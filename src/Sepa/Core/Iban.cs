using System.Text.RegularExpressions;

namespace Toarnbeike.Sepa.Core;

/// <summary>
/// International Bank Account Number, a standardized identification code for bank accounts.
/// </summary>
public readonly partial record struct Iban
{
    /// <summary>
    /// String representation of the internal value of the Iban.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Create a new Iban, and checks for validity
    /// </summary>
    /// <param name="value">The string representation.</param>
    /// <returns>A new instance of the Iban code.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="value"/> is null,
    /// whitespace, or otherwise invalid as an Iban.</exception>
    public static Iban Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        var normalized = value.Replace(" ", "").ToUpperInvariant();

        return IsValid(normalized) 
            ? new Iban(normalized) 
            : throw new ArgumentException($"Invalid IBAN value: {value}",  nameof(value));
    }
    
    /// <inheritdoc />
    public override string ToString() => Value;

    private Iban(string value) => Value = value;
    
    [GeneratedRegex("^[A-Z]{2}$")]
    private static partial Regex IbanStartsWithLandCode();
    
    /// <summary>
    /// Helper method to verify the validity of the provided <paramref name="value"/>
    /// </summary>
    /// <param name="value">The value to verify</param>
    /// <returns>Boolean indicating if the provided value is valid as an iban.</returns>
    private static bool IsValid(string value)
    {
        if (value.Length is < 15 or > 34)
        {
            return false;
        }

        if (!IbanStartsWithLandCode().IsMatch(value))
        {
            return false;
        }

        // Rearrange IBAN: move the first four characters to the end
        var rearranged = string.Concat(value.AsSpan(4), value.AsSpan(0, 4));

        // Convert letters to numbers (A = 10, B = 11, ..., Z = 35)
        var numeric = string.Concat(rearranged.Select(c => char.IsLetter(c) ? (c - 55).ToString() : c.ToString()));

        // Perform mod-97 operation on the numeric IBAN
        // Use a sliding window to perform mod-97, as the IBAN number can be very large
        return numeric.Aggregate(0, (current, c) => (current * 10 + (c - '0')) % 97) == 1;
    }
}