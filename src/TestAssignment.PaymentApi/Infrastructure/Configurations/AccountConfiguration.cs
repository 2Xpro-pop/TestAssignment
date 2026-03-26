using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Shared;

namespace TestAssignment.PaymentApi.Infrastructure.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public const string StampPropertyName = "Stamp";

    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Id)
            .ValueGeneratedNever()
            .HasConversion(
                accountId => accountId.Value,
                value => new AccountId(value));

        builder.Property(account => account.OwnerId)
            .IsRequired()
            .HasConversion(
                ownerId => ownerId.Value,
                value => AccountOwnerId.Create(value));

        builder.HasIndex(account => account.OwnerId)
            .IsUnique();

        builder.ComplexProperty(account => account.Balance, balanceBuilder =>
        {
            balanceBuilder.Property(balance => balance.MinorUnits)
                .HasColumnName("balance_minor_units")
                .IsRequired();

            balanceBuilder.Property(balance => balance.Currency)
                .HasColumnName("balance_currency_code")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    value => new Currency(value));
        });

        builder.Property<Guid>(StampPropertyName)
            .HasColumnName("stamp")
            .IsRequired()
            .IsConcurrencyToken();
    }
}