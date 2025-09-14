namespace Toarnbeike.Sepa.Core;

/// <summary>
/// Representation of a amount of money in a specified currency.
/// </summary>
public readonly record struct Money
{
    private readonly long _amount = 0;
    
    /// <summary>
    /// The amount of money in the money object.
    /// </summary>
    public decimal Amount => _amount / 100m;
    
    /// <summary>
    /// The currency of the money object.
    /// </summary>
    public string Currency { get; } = "EUR";
    
    /// <summary>
    /// Create a new money object for a custom currency.
    /// </summary>
    /// <param name="amount">The amount of money.</param>
    /// <param name="currency">The currency.</param>
    /// <returns>A new instance of the money object.</returns>
    /// <exception cref="ArgumentException">Thrown if the currency is null or whitespace</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the amount is negative or has to many decimal places</exception>
    public static Money Create(decimal amount, string currency) => new (amount, currency);

    /// <summary>
    /// Create a new money object for the Euro currency.
    /// </summary>
    /// <param name="amount">The amount of money.</param>
    /// <returns>A new instance of the money object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the amount is negative or has to many decimal places</exception>
    public static Money Euro(decimal amount) => new(amount, "EUR");

    /// <summary>
    /// The zero money object.
    /// </summary>
    public static Money Zero = new();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Currency} {Amount:F2}";
    }
    
    private Money(decimal amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero");
        }

        if (decimal.Round(amount, 2) != amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must have no more than 2 decimal places");
        }
        
        _amount = (long)(amount * 100);
        Currency = currency;
    }
}