namespace Toarnbeike.Sepa.Domain;

/// <summary>
/// Description of a sepa payment according to pain 008.001.0x.
/// </summary>
public sealed record DirectDebitPayment
{
    /// <summary>
    /// Unique identifier for this payment request.
    /// </summary>
    public string MessageId { get; }
    
    /// <summary>
    /// DateTime this sepa payment request was created.
    /// </summary>
    public DateTime CreationDateTime { get; }
    
    /// <summary>
    /// The requested date on which the direct debit should be collectable.
    /// </summary>
    public DateOnly RequestedCollectionDate { get; }
    
    /// <summary>
    /// Description of the creditor that initiates and collects the payment
    /// </summary>
    public Creditor Creditor { get; }

    /// <summary>
    /// Collection of transactions that are collected in this payment.
    /// </summary>
    public IReadOnlyCollection<DirectDebitTransaction> Transactions { get; }

    /// <summary>
    /// Create a new Direct debit Sepa payment.
    /// </summary>
    /// <param name="messageId">Unique identifier of the payment message.</param>
    /// <param name="collectionDelayInDays">Amount of days before payment is collectable.</param>
    /// <param name="creditor">Creditor of the payment.</param>
    /// <param name="transactions">Collection of transactions to collect in this payment.</param>
    /// <returns>A validated instance of the sepa payment.</returns>
    /// <exception cref="ArgumentException">Thrown if the <see cref="MessageId"/> is null or whitespace</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown is the <see cref="transactions"/> collection is empty.</exception>
    public static DirectDebitPayment Create(string messageId, int collectionDelayInDays, Creditor creditor,
        IEnumerable<DirectDebitTransaction> transactions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        var ddts = transactions.ToList() ?? throw new ArgumentNullException(nameof(transactions));
        if (ddts.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transactions));
        }
        
        return new DirectDebitPayment(
            messageId,
            DateTime.UtcNow,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(collectionDelayInDays)),
            creditor,
            ddts);
    }
    
    private DirectDebitPayment(string messageId, DateTime creationDateTime, DateOnly requestedCollectionDate,
        Creditor creditor, IReadOnlyCollection<DirectDebitTransaction> transactions) =>
        (MessageId,  CreationDateTime, RequestedCollectionDate, Creditor, Transactions) =
        (messageId, creationDateTime, requestedCollectionDate, creditor, transactions);
}