using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using VozUniversitaria.Models;
using VozUniversitaria.Repositories;
using VozUniversitaria.Services;
using Xunit;

public class OtpServiceTests
{
    [Fact]
    public async Task SendAsync_returns_failed_when_smtp_transport_throws()
    {
        var repository = new InMemoryOtpRepository();
        var emailSender = new StubEmailSender(new IOException("SMTP transport unavailable."));
        var service = new OtpService(repository, emailSender, NullLogger<OtpService>.Instance);
        var user = CreateUser();

        var result = await service.SendAsync(user);

        Assert.Equal(OtpSendResult.Failed, result);
        Assert.NotNull(user.OtpHash);
        Assert.Equal(1, repository.SaveCount);
        Assert.Equal(1, emailSender.SendCount);
    }

    [Fact]
    public async Task SendAsync_does_not_send_another_code_during_cooldown()
    {
        var repository = new InMemoryOtpRepository();
        var emailSender = new StubEmailSender();
        var service = new OtpService(repository, emailSender, NullLogger<OtpService>.Instance);
        var user = CreateUser();

        var firstResult = await service.SendAsync(user);
        var secondResult = await service.SendAsync(user);

        Assert.Equal(OtpSendResult.Sent, firstResult);
        Assert.Equal(OtpSendResult.TooSoon, secondResult);
        Assert.Equal(1, repository.SaveCount);
        Assert.Equal(1, emailSender.SendCount);
    }

    private static Users CreateUser() =>
        new()
        {
            Id = 1,
            Name = "Test",
            LastName = "User",
            CaseNumber = "123",
            Email = "test@ues.mx"
        };

    private sealed class InMemoryOtpRepository : IOtpRepository
    {
        public int SaveCount { get; private set; }

        public Task SaveAsync(Users user, CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class StubEmailSender(Exception? exception = null) : IEmailSender
    {
        public int SendCount { get; private set; }

        public Task SendAsync(string to, string subject, string body)
        {
            SendCount++;
            return exception is null ? Task.CompletedTask : Task.FromException(exception);
        }
    }
}
