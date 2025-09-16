using System.Xml.Linq;
using Sepa.Contract.Generated;
using Toarnbeike.Sepa.Domain;

namespace Toarnbeike.Sepa.Writers;

public interface ISepaPain00800108Writer
{
    /// <summary>
    /// Generate a strongly typed Sepa pain 008.001.08 document
    /// </summary>
    /// <param name="payment">The direct debit payment to convert.</param>
    Document GenerateDocument(DirectDebitPayment payment);
    
    /// <summary>
    /// Generate a Xml XDocument for validation and XPath checks
    /// </summary>
    /// <param name="payment">The direct debit payment to convert.</param>
    XDocument GenerateXDocument(DirectDebitPayment payment);

    /// <summary>
    /// Write the SEPA XML to a file.
    /// </summary>
    /// <param name="payment">The payment data to write.</param>
    /// <param name="filePath">The target file path.</param>
    /// <param name="validate">If true, the XML will be validated against the embedded SEPA XSD before writing.</param>
    Task WriteXmlAsync(DirectDebitPayment payment, string filePath, bool validate = true);
}