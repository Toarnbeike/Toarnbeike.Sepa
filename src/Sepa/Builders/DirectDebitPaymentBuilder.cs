using Toarnbeike.Sepa.Core;
using Toarnbeike.Sepa.Domain;

namespace Toarnbeike.Sepa.Builders;

public class DirectDebitPaymentBuilder(Creditor creditor)
{
    private readonly List<DirectDebitTransaction> _transactions = [];

    public DirectDebitPaymentBuilder AddTransaction(
        Debtor debtor,
        Money amount,
        Mandate mandate,
        string? remittanceInformation = null,
        string? endToEndId = null)
    {
        endToEndId ??= $"{Guid.NewGuid():N}";
        var transaction = DirectDebitTransaction.Create(
            debtor, amount, mandate, endToEndId, remittanceInformation);
        _transactions.Add(transaction);
        return this;
    }

    public DirectDebitPayment Build(int collectionDelayInDays, string? messageId = null)
    {
        messageId ??= $"MSG-{Guid.NewGuid():N)}";
        
        return DirectDebitPayment.Create(messageId, collectionDelayInDays, creditor,  _transactions);
    }
}