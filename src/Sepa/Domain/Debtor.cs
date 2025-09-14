using Toarnbeike.Sepa.Core;

namespace Toarnbeike.Sepa.Domain;

/// <summary>
/// Description of the debtor in a <see cref="DirectDebitTransaction" />
/// The debtor is the payer of the transaction.
/// </summary>
public sealed record Debtor
{
    /// <summary>
    /// Name of the Debtor
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Full Iban of the debtor
    /// </summary>
    public Iban Iban { get; }
    
    /// <summary>
    /// Optionally the Bic of debtor
    /// Can be omitted for domestic transactions.
    /// </summary>
    public Bic? Bic { get; }

    /// <summary>
    /// Create a new instance of a Debtor for a <see cref="DirectDebitTransaction" />
    /// </summary>
    /// <param name="name">The name of the debtor.</param>
    /// <param name="iban">The iban of the debtor.</param>
    /// <param name="bic">Optionally the bic of the debtor. Can be omitted for domestic transactions.</param>
    /// <returns>A new valid instance of the debtor or an exception is thrown</returns>
    /// <exception cref="ArgumentException">Thrown when the provided name is null or whitespace.</exception>
    public static Debtor Create(string name, Iban iban, Bic? bic = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Debtor(name, iban, bic);
    }
    
    private Debtor(string name, Iban iban, Bic? bic = null) =>
        (Name, Iban, Bic) = (name, iban, bic);
}