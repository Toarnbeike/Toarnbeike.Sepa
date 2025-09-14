namespace Toarnbeike.Sepa.Core;

public sealed record Mandate
{
    /// <summary>
    /// Unique identification of the provided mandate.
    /// </summary>
    public string MandateId { get; }
    
    /// <summary>
    /// Date on which the mandate was signed.
    /// </summary>
    public DateOnly DateOfSignature { get; }
    
    /// <summary>
    /// Optionally additional information about the mandate.
    /// Can be used for information about the signer or the signing method.
    /// </summary>
    public string? Remarks { get; }

    public static Mandate Create(string mandateId, DateOnly dateOfSignature, string? remarks = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mandateId);
        return new Mandate(mandateId, dateOfSignature, remarks);
    }
    
    private Mandate(string mandateId, DateOnly dateOfSignature, string? remarks = null) =>
        (MandateId, DateOfSignature, Remarks) = (mandateId,  dateOfSignature, remarks);
}