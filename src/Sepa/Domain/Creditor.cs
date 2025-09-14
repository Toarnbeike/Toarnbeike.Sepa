using Toarnbeike.Sepa.Core;

namespace Toarnbeike.Sepa.Domain;

/// <summary>
/// Description of the creditor in a <see cref="DirectDebitPayment"/>
/// The creditor is the initiating party and the receiver of the money.
/// </summary>
public sealed record Creditor
{
    /// <summary>
    /// Name of the creditor
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Full Iban of the creditor
    /// </summary>
    public Iban Iban { get; }
    
    /// <summary>
    /// The Bic of bank of the creditor
    /// Contrary to the <see cref="Debtor"/>, the Bic is required for the creditor.
    /// </summary>
    public Bic Bic { get; }

    /// <summary>
    /// Identification number that allows the creditor to initiate a direct debit payment.
    /// </summary>
    public string CreditorId { get; }

    /// <summary>
    /// Create a new instance of a Creditor for a <see cref="DirectDebitPayment" />
    /// </summary>
    /// <param name="name">The name of the creditor.</param>
    /// <param name="iban">The iban of the creditor.</param>
    /// <param name="bic">The bic of the creditor.</param>
    /// <param name="creditorId">Identification of the creditor that allows initiating direct debit payments </param>
    /// <returns>A validated instance of a creditor or an exception is thrown.</returns>
    /// <exception cref="ArgumentException">Throw if either the provided name or the provided creditorId is null or whitespace. </exception>
    public static Creditor Create(string name, Iban iban, Bic bic, string creditorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(creditorId);
        return new Creditor(name, iban, bic, creditorId);
    }
    
    private Creditor(string name, Iban iban, Bic bic, string creditorId) =>
        (Name, Iban, Bic, CreditorId) = (name, iban, bic, creditorId);

}