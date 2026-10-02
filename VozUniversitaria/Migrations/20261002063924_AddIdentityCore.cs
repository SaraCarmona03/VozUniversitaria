using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VozUniversitaria.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL
                BEGIN
                    IF OBJECT_ID(N'dbo.LegacyAspNetUsers', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetRoles', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetUserRoles', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetUserClaims', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetRoleClaims', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetUserLogins', N'U') IS NOT NULL
                       OR OBJECT_ID(N'dbo.LegacyAspNetUserTokens', N'U') IS NOT NULL
                    BEGIN
                        ;THROW 51000, 'Legacy Identity archive tables already exist. Migration stopped to avoid overwriting archived data.', 1;
                    END;

                    SELECT * INTO dbo.LegacyAspNetUsers FROM dbo.AspNetUsers;
                    SELECT * INTO dbo.LegacyAspNetRoles FROM dbo.AspNetRoles;
                    SELECT * INTO dbo.LegacyAspNetUserRoles FROM dbo.AspNetUserRoles;
                    SELECT * INTO dbo.LegacyAspNetUserClaims FROM dbo.AspNetUserClaims;
                    SELECT * INTO dbo.LegacyAspNetRoleClaims FROM dbo.AspNetRoleClaims;
                    SELECT * INTO dbo.LegacyAspNetUserLogins FROM dbo.AspNetUserLogins;
                    SELECT * INTO dbo.LegacyAspNetUserTokens FROM dbo.AspNetUserTokens;

                    DROP TABLE dbo.AspNetUserTokens;
                    DROP TABLE dbo.AspNetUserLogins;
                    DROP TABLE dbo.AspNetUserClaims;
                    DROP TABLE dbo.AspNetUserRoles;
                    DROP TABLE dbo.AspNetRoleClaims;
                    DROP TABLE dbo.AspNetUsers;
                    DROP TABLE dbo.AspNetRoles;
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [Users]
                SET [UserName] = [Email],
                    [NormalizedUserName] = UPPER(LTRIM(RTRIM([Email]))),
                    [NormalizedEmail] = UPPER(LTRIM(RTRIM([Email]))),
                    [EmailConfirmed] = [IsEmailVerified],
                    [LockoutEnabled] = 1,
                    [SecurityStamp] = CONVERT(nvarchar(36), NEWID()),
                    [ConcurrencyStamp] = CONVERT(nvarchar(36), NEWID())
                WHERE [Email] IS NOT NULL;
                """);

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.Sql(
                """
                INSERT INTO dbo.AspNetRoles (Name, NormalizedName, ConcurrencyStamp)
                SELECT roles.Name, UPPER(roles.Name), CONVERT(nvarchar(36), NEWID())
                FROM (VALUES (N'Student'), (N'Teacher'), (N'Admin'), (N'Staff')) AS roles(Name)
                WHERE NOT EXISTS (
                    SELECT 1 FROM dbo.AspNetRoles existing
                    WHERE existing.NormalizedName = UPPER(roles.Name));

                IF OBJECT_ID(N'dbo.LegacyAspNetRoles', N'U') IS NOT NULL
                BEGIN
                    INSERT INTO dbo.AspNetRoles (Name, NormalizedName, ConcurrencyStamp)
                    SELECT oldRole.Name, UPPER(oldRole.Name), COALESCE(oldRole.ConcurrencyStamp, CONVERT(nvarchar(36), NEWID()))
                    FROM dbo.LegacyAspNetRoles oldRole
                    WHERE oldRole.Name IS NOT NULL
                      AND NOT EXISTS (
                          SELECT 1 FROM dbo.AspNetRoles existing
                          WHERE existing.NormalizedName = UPPER(oldRole.Name));

                    INSERT INTO dbo.Users
                        (UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash,
                         SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                         LockoutEnd, LockoutEnabled, AccessFailedCount, Name, LastName, CaseNumber, Role,
                         IsActive, CreatedAt, IsEmailVerified, OtpHash, OtpExpiresAt, OtpAttempts, OtpLastSentAt)
                    SELECT oldUser.Email, UPPER(LTRIM(RTRIM(oldUser.Email))), oldUser.Email,
                           UPPER(LTRIM(RTRIM(oldUser.Email))), oldUser.EmailConfirmed, oldUser.PasswordHash,
                           COALESCE(oldUser.SecurityStamp, CONVERT(nvarchar(36), NEWID())),
                           CONVERT(nvarchar(36), NEWID()), oldUser.PhoneNumber, oldUser.PhoneNumberConfirmed,
                           oldUser.TwoFactorEnabled, oldUser.LockoutEnd, 1, oldUser.AccessFailedCount,
                           oldUser.Name, oldUser.LastName, oldUser.CaseNumber, oldUser.Role, oldUser.IsActive,
                           oldUser.CreatedAt, oldUser.EmailConfirmed, NULL, NULL, 0, NULL
                    FROM dbo.LegacyAspNetUsers oldUser
                    WHERE oldUser.Email IS NOT NULL
                      AND NOT EXISTS (
                          SELECT 1 FROM dbo.Users currentUser
                          WHERE UPPER(LTRIM(RTRIM(currentUser.Email))) = UPPER(LTRIM(RTRIM(oldUser.Email))));
                END;

                INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
                SELECT currentUser.Id, identityRole.Id
                FROM dbo.Users currentUser
                JOIN dbo.AspNetRoles identityRole
                  ON identityRole.NormalizedName =
                    CASE currentUser.Role
                        WHEN 0 THEN N'STUDENT'
                        WHEN 1 THEN N'TEACHER'
                        WHEN 2 THEN N'ADMIN'
                        WHEN 3 THEN N'STAFF'
                    END
                WHERE NOT EXISTS (
                    SELECT 1 FROM dbo.AspNetUserRoles existing
                    WHERE existing.UserId = currentUser.Id AND existing.RoleId = identityRole.Id);

                IF OBJECT_ID(N'dbo.LegacyAspNetUsers', N'U') IS NOT NULL
                BEGIN
                    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
                    SELECT currentUser.Id, identityRole.Id
                    FROM dbo.LegacyAspNetUserRoles oldAssignment
                    JOIN dbo.LegacyAspNetUsers oldUser ON oldUser.Id = oldAssignment.UserId
                    JOIN dbo.LegacyAspNetRoles oldRole ON oldRole.Id = oldAssignment.RoleId
                    JOIN dbo.Users currentUser
                      ON UPPER(LTRIM(RTRIM(currentUser.Email))) = UPPER(LTRIM(RTRIM(oldUser.Email)))
                    JOIN dbo.AspNetRoles identityRole
                      ON identityRole.NormalizedName = UPPER(oldRole.Name)
                    WHERE NOT EXISTS (
                        SELECT 1 FROM dbo.AspNetUserRoles existing
                        WHERE existing.UserId = currentUser.Id AND existing.RoleId = identityRole.Id);

                    INSERT INTO dbo.AspNetUserClaims (UserId, ClaimType, ClaimValue)
                    SELECT currentUser.Id, oldClaim.ClaimType, oldClaim.ClaimValue
                    FROM dbo.LegacyAspNetUserClaims oldClaim
                    JOIN dbo.LegacyAspNetUsers oldUser ON oldUser.Id = oldClaim.UserId
                    JOIN dbo.Users currentUser
                      ON UPPER(LTRIM(RTRIM(currentUser.Email))) = UPPER(LTRIM(RTRIM(oldUser.Email)));

                    INSERT INTO dbo.AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                    SELECT identityRole.Id, oldClaim.ClaimType, oldClaim.ClaimValue
                    FROM dbo.LegacyAspNetRoleClaims oldClaim
                    JOIN dbo.LegacyAspNetRoles oldRole ON oldRole.Id = oldClaim.RoleId
                    JOIN dbo.AspNetRoles identityRole
                      ON identityRole.NormalizedName = UPPER(oldRole.Name);

                    INSERT INTO dbo.AspNetUserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                    SELECT oldLogin.LoginProvider, oldLogin.ProviderKey, oldLogin.ProviderDisplayName, currentUser.Id
                    FROM dbo.LegacyAspNetUserLogins oldLogin
                    JOIN dbo.LegacyAspNetUsers oldUser ON oldUser.Id = oldLogin.UserId
                    JOIN dbo.Users currentUser
                      ON UPPER(LTRIM(RTRIM(currentUser.Email))) = UPPER(LTRIM(RTRIM(oldUser.Email)));

                    INSERT INTO dbo.AspNetUserTokens (UserId, LoginProvider, Name, Value)
                    SELECT currentUser.Id, oldToken.LoginProvider, oldToken.Name, oldToken.Value
                    FROM dbo.LegacyAspNetUserTokens oldToken
                    JOIN dbo.LegacyAspNetUsers oldUser ON oldUser.Id = oldToken.UserId
                    JOIN dbo.Users currentUser
                      ON UPPER(LTRIM(RTRIM(currentUser.Email))) = UPPER(LTRIM(RTRIM(oldUser.Email)));
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
