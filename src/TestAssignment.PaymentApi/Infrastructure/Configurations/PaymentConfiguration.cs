using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;
using TestAssignment.PaymentApi.Domain.Shared;

namespace TestAssignment.PaymentApi.Infrastructure.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.Id)
            .ValueGeneratedNever()
            .HasConversion(
                paymentId => paymentId.Value,
                value => new PaymentId(value));

        builder.Property(payment => payment.AccountId)
            .IsRequired()
            .HasConversion(
                accountId => accountId.Value,
                value => new AccountId(value));

        builder.Property(payment => payment.OwnerId)
            .IsRequired()
            .HasConversion(
                ownerId => ownerId.Value,
                value => AccountOwnerId.Create(value));

        builder.ComplexProperty(payment => payment.Amount, amountBuilder =>
        {
            amountBuilder.Property(amount => amount.MinorUnits)
                .HasColumnName("amount_minor_units")
                .IsRequired();

            amountBuilder.Property(amount => amount.Currency)
                .HasColumnName("amount_currency_code")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    value => new Currency(value));
        });

        builder.Property(payment => payment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(payment => payment.OwnerId);
        builder.HasIndex(payment => payment.AccountId);
        builder.HasIndex(payment => payment.CreatedAtUtc);
    }
}