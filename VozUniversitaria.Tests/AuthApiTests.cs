using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VozUniversitaria.Data;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using Xunit;

public class AuthApiTests
{
    [Fact]
    public async Task Login_preserves_cookie_session_and_enforces_role_authorization()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        await factory.SeedUserAsync("student@ues.mx", UserRole.Student, isVerified: true);
        await factory.SeedUserAsync("admin@ues.mx", UserRole.Admin, isVerified: true);

        var csrfToken = await GetCsrfTokenAsync(client);
        var studentLogin = await PostWithCsrfAsync(client, "/api/auth/login", csrfToken,
            new { email = "student@ues.mx", password = "Password123" });

        Assert.Equal(HttpStatusCode.NoContent, studentLogin.StatusCode);

        var studentSession = await client.GetFromJsonAsync<JsonElement>("/api/session");
        Assert.True(studentSession.GetProperty("authenticated").GetBoolean());
        Assert.Contains("Student", studentSession.GetProperty("roles").EnumerateArray()
            .Select(role => role.GetString()));
        Assert.Equal(HttpStatusCode.Forbidden,
            (await client.GetAsync("/api/admin/access")).StatusCode);

        csrfToken = await GetCsrfTokenAsync(client);
        var logout = await PostWithCsrfAsync(client, "/api/auth/logout", csrfToken, new { });
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        Assert.False((await client.GetFromJsonAsync<JsonElement>("/api/session"))
            .GetProperty("authenticated").GetBoolean());

        csrfToken = await GetCsrfTokenAsync(client);
        var adminLogin = await PostWithCsrfAsync(client, "/api/auth/login", csrfToken,
            new { email = "admin@ues.mx", password = "Password123" });
        Assert.Equal(HttpStatusCode.NoContent, adminLogin.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/admin/access")).StatusCode);
    }

    [Fact]
    public async Task Unverified_account_can_verify_email_through_the_existing_otp_service()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        await factory.SeedUserAsync("pending@ues.mx", UserRole.Student, isVerified: false);
        var csrfToken = await GetCsrfTokenAsync(client);

        var resend = await PostWithCsrfAsync(client, "/api/auth/verification/resend", csrfToken,
            new { email = "pending@ues.mx" });
        Assert.Equal(HttpStatusCode.Accepted, resend.StatusCode);

        var code = TestEmailSender.GetLastCode();
        var verify = await PostWithCsrfAsync(client, "/api/auth/verification/verify", csrfToken,
            new { email = "pending@ues.mx", code });
        Assert.Equal(HttpStatusCode.NoContent, verify.StatusCode);

        var user = await factory.FindUserAsync("pending@ues.mx");
        Assert.True(user.IsEmailVerified);
        Assert.True(user.EmailConfirmed);
    }

    [Fact]
    public async Task Api_authorization_returns_status_codes_instead_of_mvc_redirects()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/admin/access");
        var loginPage = await client.GetAsync("/Account/Login");
        var registrationPage = await client.GetAsync("/Account/Register?role=Student");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, loginPage.StatusCode);
        Assert.Equal("/app/index.html", loginPage.Headers.Location?.OriginalString);
        Assert.Equal(HttpStatusCode.Redirect, registrationPage.StatusCode);
        Assert.Equal("/app/index.html#registro", registrationPage.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task State_changing_api_requests_require_an_antiforgery_token()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email = "student@ues.mx", password = "Password123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_creates_student_and_starts_existing_otp_verification()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        var csrfToken = await GetCsrfTokenAsync(client);

        var response = await PostWithCsrfAsync(client, "/api/auth/register", csrfToken,
            RegistrationPayload("new-student@ues.mx"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("requiresEmailVerification").GetBoolean());
        Assert.Equal("new-student@ues.mx", body.GetProperty("email").GetString());
        Assert.True(body.TryGetProperty("message", out _));

        var user = await factory.FindUserAsync("new-student@ues.mx");
        Assert.Equal(UserRole.Student, user.Role);
        Assert.False(user.IsEmailVerified);
        Assert.NotNull(user.OtpHash);
        Assert.Equal("new-student@ues.mx", TestEmailSender.GetLastRecipient());

        var verify = await PostWithCsrfAsync(client, "/api/auth/verification/verify", csrfToken,
            new { email = "new-student@ues.mx", code = TestEmailSender.GetLastCode() });
        Assert.Equal(HttpStatusCode.NoContent, verify.StatusCode);
    }

    [Fact]
    public async Task Register_returns_a_field_error_for_duplicate_email()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        var csrfToken = await GetCsrfTokenAsync(client);
        var payload = RegistrationPayload("duplicate@ues.mx");

        var firstResponse = await PostWithCsrfAsync(client, "/api/auth/register", csrfToken, payload);
        var duplicateResponse = await PostWithCsrfAsync(client, "/api/auth/register", csrfToken, payload);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        var body = await duplicateResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Revisa los datos del formulario.", body.GetProperty("error").GetString());
        Assert.True(body.GetProperty("errors").TryGetProperty("email", out _));
    }

    [Fact]
    public async Task Register_returns_a_field_error_when_passwords_do_not_match()
    {
        await using var factory = new ApiApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        var csrfToken = await GetCsrfTokenAsync(client);
        var response = await PostWithCsrfAsync(client, "/api/auth/register", csrfToken,
            RegistrationPayload("mismatch@ues.mx", "OtherPassword456"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("errors").TryGetProperty("confirmPassword", out _));
    }

    private static async Task<string> GetCsrfTokenAsync(HttpClient client)
    {
        var result = await client.GetFromJsonAsync<JsonElement>("/api/security/csrf");
        return result.GetProperty("requestToken").GetString()
            ?? throw new InvalidOperationException("The CSRF endpoint returned no request token.");
    }

    private static Task<HttpResponseMessage> PostWithCsrfAsync(
        HttpClient client,
        string path,
        string csrfToken,
        object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add("X-CSRF-TOKEN", csrfToken);
        return client.SendAsync(request);
    }

    private static object RegistrationPayload(string email, string confirmPassword = "Password123") => new
    {
        name = "Test",
        lastName = "Student",
        caseNumber = $"case-{Guid.NewGuid():N}",
        email,
        password = "Password123",
        confirmPassword
    };
}

public sealed class ApiApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"VozUniversitariaTests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender, TestEmailSender>();
        });
    }

    public async Task SeedUserAsync(string email, UserRole role, bool isVerified)
    {
        using var scope = Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleName = role.ToString();
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            Assert.True(roleResult.Succeeded);
        }

        var user = new Users
        {
            Name = "Test",
            LastName = "User",
            CaseNumber = $"case-{Guid.NewGuid():N}",
            UserName = email,
            Email = email,
            EmailConfirmed = isVerified,
            IsEmailVerified = isVerified,
            Role = role
        };
        var createResult = await userManager.CreateAsync(user, "Password123");
        Assert.True(createResult.Succeeded, string.Join("; ", createResult.Errors.Select(error => error.Description)));
        var assignRoleResult = await userManager.AddToRoleAsync(user, roleName);
        Assert.True(assignRoleResult.Succeeded);
    }

    public async Task<Users> FindUserAsync(string email)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
        return await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException("The seeded test user was not found.");
    }
}

public sealed class TestEmailSender : IEmailSender
{
    private static string? _lastBody;
    private static string? _lastRecipient;

    public Task SendAsync(string to, string subject, string body)
    {
        _lastBody = body;
        _lastRecipient = to;
        return Task.CompletedTask;
    }

    public static string GetLastRecipient() =>
        _lastRecipient ?? throw new InvalidOperationException("No test email was sent.");

    public static string GetLastCode()
    {
        var match = Regex.Match(_lastBody ?? string.Empty, @"code is (?<code>\d{6})");
        return match.Success
            ? match.Groups["code"].Value
            : throw new InvalidOperationException("The OTP email did not contain a verification code.");
    }
}
