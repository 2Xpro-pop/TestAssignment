using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAssignment.IdentityApi.Domain.Sesssions;
using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Infrastructure.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(userSession => userSession.Id);

        builder.Property(userSession => userSession.Id)
            .ValueGeneratedNever()
            .HasConversion(
                sessionId => sessionId.Value,
                value => new SessionId(value));

        builder.Property(userSession => userSession.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasConversion(
                userId => userId.Value,
                value => new UserId(value));

        builder.Property(userSession => userSession.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(64)
            .IsRequired()
            .HasConversion(
                tokenHash => tokenHash.Value,
                value => new TokenHash(value));

        builder.Property(userSession => userSession.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(userSession => userSession.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .IsRequired();

        builder.Property(userSession => userSession.RevokedAtUtc)
            .HasColumnName("revoked_at_utc")
            .IsRequired(false);

        builder.HasIndex(userSession => userSession.TokenHash)
            .IsUnique();

        builder.HasIndex(userSession => userSession.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(userSession => userSession.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}