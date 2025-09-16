using System.Xml.Linq;
using System.Xml.Schema;
using Shouldly;
using Toarnbeike.Sepa.Core;
using Toarnbeike.Sepa.Writers;

namespace Toarnbeike.Sepa.Tests.Writers;

public class SepaPain00800108WriterTests
    {
        private readonly ISepaPain00800108Writer _writer = new SepaPain00800108Writer();

        private static string GetEmbeddedXsdPath()
        {
            var assembly = typeof(SepaPain00800108Writer).Assembly;

            var resourceName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("pain.008.001.08.xsd", StringComparison.OrdinalIgnoreCase));

            var tempPath = Path.GetTempFileName();
            using var resource = assembly.GetManifestResourceStream(resourceName)!;
            using var file = File.OpenWrite(tempPath);
            resource.CopyTo(file);

            return tempPath;
        }

        private static void ValidateXml(string xmlFilePath, string xsdPath)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add("urn:iso:std:iso:20022:tech:xsd:pain.008.001.08", xsdPath);

            var doc = XDocument.Load(xmlFilePath, LoadOptions.PreserveWhitespace);
            string? validationErrors = null;

            doc.Validate(schemas, (o, e) =>
            {
                validationErrors += e.Message + "\n";
            });

            validationErrors.ShouldBeNullOrEmpty();
        }
        
        [Fact]
        public void GenerateDocument_ShouldContainExpectedValues()
        {
            var payment = TestData.Pain008.CreatePayment();

            var document = _writer.GenerateDocument(payment);

            document.CstmrDrctDbtInitn.GrpHdr.MsgId.ShouldBe(payment.MessageId);
            document.CstmrDrctDbtInitn.PmtInf.First().Cdtr.Nm.ShouldBe(payment.Creditor.Name);
            document.CstmrDrctDbtInitn.PmtInf.First().CdtrAcct.Id.Iban.ShouldBe(payment.Creditor.Iban.Value);
        }
        
        [Fact]
        public async Task WriteXmlAsync_ShouldGenerateXsdValidFile()
        {
            var payment = TestData.Pain008.CreatePayment();
            var tempFile = Path.GetTempFileName();
            var xsdPath = GetEmbeddedXsdPath();

            await _writer.WriteXmlAsync(payment, tempFile);

            ValidateXml(tempFile, xsdPath);
        }

        [Fact]
        public async Task InvalidPayment_ShouldFailValidation()
        {
            var invalidCreditor = TestData.Pain008.CreateCreditor(new Iban());
            
            var payment = TestData.Pain008.CreatePayment(invalidCreditor);
            var tempFile = Path.GetTempFileName();
            var xsdPath = GetEmbeddedXsdPath();

            await _writer.WriteXmlAsync(payment, tempFile);

            Should.Throw<ShouldAssertException>(() =>
            {
                ValidateXml(tempFile, xsdPath);
            });
        }
    }