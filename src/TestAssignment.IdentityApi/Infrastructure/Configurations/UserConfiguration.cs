using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Infrastructure.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever()
            .HasConversion(
                userId => userId.Value,
                value => new UserId(value));

        builder.Property(user => user.Login)
            .HasColumnName("login")
            .HasMaxLength(64)
            .IsRequired()
            .HasConversion(
                login => login.Value,
                value => new Login(value));

        builder.HasIndex(user => user.Login)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired()
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => new PasswordHash(value));

        builder.ComplexProperty(
            user => user.LockoutState,
            lockoutStateBuilder =>
            {
                lockoutStateBuilder.Property(lockoutState => lockoutState.FailedLoginCount)
                    .HasColumnName("failed_login_count")
                    .IsRequired();

                lockoutStateBuilder.Property(lockoutState => lockoutState.LockoutEndUtc)
                    .HasColumnName("lockout_end_utc")
                    .IsRequired(false);
            });
    }
}