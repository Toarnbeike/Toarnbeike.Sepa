namespace Toarnbeike.Sepa.Core;

/// <summary>
/// Bank identification code, also called Swift code.
/// Is directly coupled to the <see cref="Iban"/> of the bank account.
/// </summary>
public readonly record struct Bic
{
    /// <summary>
    /// String representation of the Bic.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Create a new Bic, and checks for validity
    /// </summary>
    /// <param name="value">The string representation.</param>
    /// <returns>A new instance of the Bic code.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="value"/> is null,
    /// whitespace, or has an incorrect length.</exception>
    public static Bic Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        var normalized = value.Replace(" ", "").ToUpperInvariant();
        
        // verify the length of the provided value is between 8 and 11 characters.
        return normalized.Length is >= 8 and <= 11
            ? new Bic(normalized)
            : throw new ArgumentException($"Invalid BIC length: {value}", nameof(value));
    }
    
    /// <inheritdoc />
    public override string ToString() => Value;
    private Bic(string value) => Value = value;
}