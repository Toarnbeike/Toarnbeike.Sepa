using Toarnbeike.Sepa.Builders;
using Toarnbeike.Sepa.Core;
using Toarnbeike.Sepa.Domain;

namespace Toarnbeike.Sepa.Tests.TestData;

internal static class Pain008
{
    public static Creditor CreateCreditor(Iban? iban = null)
    {
        return Creditor.Create(
            name: "Test Creditor BV",
            iban: iban ?? Iban.Create("GB33BUKB20201555555555"),
            bic: Bic.Create("ABNANL2A"),
            creditorId: "NL98ZZZ123456780001");
    }

    public static Debtor CreateDebtor(Iban? iban = null)
    {
        return Debtor.Create(
            name: "Debtor",
            iban: iban ?? Iban.Create("GB94BARC10201530093459"),
            bic: Bic.Create("ABNANL2A"));
    }

    public static Mandate CreateMandate()
    {
        return Mandate.Create(
            mandateId: "Mandate-001",
            dateOfSignature: new DateOnly(2025,1,1),
            remarks: "This field should not turn up in the document");
    }

    public static DirectDebitPayment CreatePayment(Creditor? creditor = null, Debtor? debtor = null, decimal? amount = null, string? endToEndId = null)
    {
        creditor ??= CreateCreditor();
        var builder = new DirectDebitPaymentBuilder(creditor);
        
        builder.AddTransaction(
            debtor: debtor ?? CreateDebtor(),
            amount: Money.Euro(amount ?? 125.75m),
            mandate: CreateMandate(),
            remittanceInformation: "Factuur-001",
            endToEndId: endToEndId);
        
        return builder.Build(messageId: "MSG-001");
    }
}