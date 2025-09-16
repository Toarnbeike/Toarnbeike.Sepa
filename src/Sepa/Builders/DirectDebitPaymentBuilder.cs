using Toarnbeike.Sepa.Core;
using Toarnbeike.Sepa.Domain;

namespace Toarnbeike.Sepa.Builders;

/// <summary>
/// Builder to simplify the creation of a <see cref="DirectDebitPayment"/>
/// </summary>
/// <param name="creditor">The initiating party of the payment.</param>
public class DirectDebitPaymentBuilder(Creditor creditor)
{
    private readonly List<DirectDebitTransaction> _transactions = [];

    /// <summary>
    /// Add a transaction to the direct debit payment.
    /// </summary>
    /// <param name="debtor">The debtor of this specific transaction.</param>
    /// <param name="amount">The amount of the transaction.</param>
    /// <param name="mandate">The mandate that allows for a transaction.</param>
    /// <param name="remittanceInformation">Description of the transaction for the debtor.</param>
    /// <param name="endToEndId">Optional identification number for the complete transaction.</param>
    /// <returns>The same builder for fluent configuration.</returns>
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

    /// <summary>
    /// Build the direct debit payment.
    /// </summary>
    /// <param name="messageId">The unique identifier of this collection. Defaults to "MSG-{new GuidV7}"</param>
    /// <param name="collectionDelayInDays">Collection is delayed by this amount of days. Defaults to 10.</param>
    public DirectDebitPayment Build(string? messageId = null, int collectionDelayInDays = 10)
    {
        messageId ??= $"MSG-{Guid.CreateVersion7():N)}";
        
        return DirectDebitPayment.Create(messageId, collectionDelayInDays, creditor,  _transactions);
    }
}