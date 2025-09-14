using Toarnbeike.Sepa.Core;

namespace Toarnbeike.Sepa.Domain;

/// <summary>
/// Transaction following the SEPA 008.001.0x schema for a direct debit
/// from a <see cref="Debtor"/> to the initiating party.
/// </summary>
public sealed record DirectDebitTransaction
{
    /// <summary>
    /// The person to pay the direct debit.
    /// </summary>
    public Debtor Debtor { get; }
    
    /// <summary>
    /// The amount of money to pay.
    /// </summary>
    public Money Amount { get; }
    
    /// <summary>
    /// The mandate which allows for a direct debit transaction
    /// </summary>
    public Mandate Mandate { get; }
    
    /// <summary>
    /// Unique identifier for this transaction
    /// </summary>
    public string EndToEndId { get; }
    
    /// <summary>
    /// Optionally additional information regarding the transaction.
    /// Goal is clarification for the debtor what this transaction is for. 
    /// </summary>
    public string? RemittanceInformation { get; }

    /// <summary>
    /// Create a new valid Direct debit transaction. 
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="debtor"/> or <paramref name="mandate"/> are null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is zero or negative.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="endToEndId"/> is null or whitespace.</exception>
    public static DirectDebitTransaction Create(
        Debtor debtor, Money amount, Mandate mandate,
        string endToEndId, string? remittanceInformation = null)
    {
        ArgumentNullException.ThrowIfNull(debtor);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount.Amount);
        ArgumentNullException.ThrowIfNull(mandate);
        ArgumentException.ThrowIfNullOrWhiteSpace(endToEndId);
        return new DirectDebitTransaction(debtor, amount, mandate, endToEndId, remittanceInformation);
    }

    private DirectDebitTransaction(Debtor debtor, Money amount, Mandate mandate, string endToEndId,
        string? remittanceInformation = null) =>
        (Debtor, Amount, Mandate, EndToEndId, RemittanceInformation) = (debtor, amount, mandate, endToEndId,  remittanceInformation);
}