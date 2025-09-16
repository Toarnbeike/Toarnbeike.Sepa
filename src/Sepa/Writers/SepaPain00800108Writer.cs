using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Sepa.Contract.Generated;
using Toarnbeike.Sepa.Domain;

namespace Toarnbeike.Sepa.Writers;

internal sealed class SepaPain00800108Writer : ISepaPain00800108Writer
{
    public Document GenerateDocument(DirectDebitPayment payment)
    {
        return new Document
        {
            CstmrDrctDbtInitn = MapCustomerDirectDebitInitiation(payment)
        };
    }

    public XDocument GenerateXDocument(DirectDebitPayment payment)
    {
        var document = GenerateDocument(payment);
        
        var serializer = new XmlSerializer(typeof(Document));
        using var memoryStream = new MemoryStream();
        serializer.Serialize(memoryStream, document);
        
        memoryStream.Position = 0;
        return XDocument.Load(memoryStream);
    }

    public async Task WriteXmlAsync(DirectDebitPayment payment, string filePath, bool validate = true)
    {
        var xdocument = GenerateXDocument(payment);

        if (validate)
        {
            ValidateAgainstSchema(xdocument);
        }
        
        await Task.Run(() => xdocument.Save(filePath));
    }

    private CustomerDirectDebitInitiationV08 MapCustomerDirectDebitInitiation(DirectDebitPayment payment)
    {
        var paymentInstruction = MapPaymentInfo(payment);
        var customerDirectDebitInitiation = new CustomerDirectDebitInitiationV08
        {
            GrpHdr = MapGroupHeader(payment)
        };
        customerDirectDebitInitiation.PmtInf.Add(paymentInstruction);
        return customerDirectDebitInitiation;
    }

    private GroupHeader83 MapGroupHeader(DirectDebitPayment payment)
    {
        return new GroupHeader83
        {
            MsgId = payment.MessageId,
            CreDtTm = payment.CreationDateTime,
            NbOfTxs = payment.Transactions.Count.ToString(),
            CtrlSum = payment.Transactions.Sum(t => t.Amount.Amount),
            InitgPty = new PartyIdentification135
            {
                Nm = payment.Creditor.Name
            }
        };
    }

    private PaymentInstruction29 MapPaymentInfo(DirectDebitPayment payment)
    {
        var paymentTypeInformation = new PaymentTypeInformation29()
        {
            LclInstrm = new LocalInstrument2Choice { Cd = "CORE" },
            SeqTp = SequenceType3Code.Rcur
        };
        paymentTypeInformation.SvcLvl.Add(new ServiceLevel8Choice { Cd = "SEPA" });

        var personIdentification = new PersonIdentification13();
        personIdentification.Othr.Add(new GenericPersonIdentification1
        {
            Id = payment.Creditor.CreditorId,
            SchmeNm = new PersonIdentificationSchemeName1Choice
            {
                Cd = "SEPA"
            }
        });
        
        var paymentInstruction = new PaymentInstruction29
        {
            PmtInfId = payment.MessageId,
            PmtMtd = PaymentMethod2Code.Dd,
            BtchBookg = true,
            NbOfTxs = payment.Transactions.Count.ToString(),
            CtrlSum = payment.Transactions.Sum(t => t.Amount.Amount),
            PmtTpInf = paymentTypeInformation,
            
            ReqdColltnDt = payment.RequestedCollectionDate.ToDateTime(TimeOnly.MinValue),
            Cdtr = new PartyIdentification135
            {
                Nm = payment.Creditor.Name
            },
            CdtrAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Iban = payment.Creditor.Iban.ToString()
                },
                Ccy = "EUR"
            },
            CdtrAgt = new BranchAndFinancialInstitutionIdentification6
            {
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    Bicfi = payment.Creditor.Bic.ToString()
                }
            },
            ChrgBr = ChargeBearerType1Code.Slev,
            CdtrSchmeId = new PartyIdentification135
            {
                Id = new Party38Choice
                {
                    PrvtId = personIdentification,
                }
            },
        };

        foreach (var transaction in payment.Transactions.Select(MapTransaction))
        {
            paymentInstruction.DrctDbtTxInf.Add(transaction);
        }
        
        return paymentInstruction;
    }

    private DirectDebitTransactionInformation23 MapTransaction(DirectDebitTransaction tx)
    {
        var remittenceInformation = string.IsNullOrWhiteSpace(tx.RemittanceInformation)
            ? null
            : new RemittanceInformation16();
        remittenceInformation?.Ustrd.Add(tx.RemittanceInformation);
        
        return new DirectDebitTransactionInformation23
        {
            PmtId = new PaymentIdentification6
            {
                EndToEndId = tx.EndToEndId
            },
            InstdAmt = new ActiveOrHistoricCurrencyAndAmount
            {
                Ccy = tx.Amount.Currency,
                Value = tx.Amount.Amount
            },
            DrctDbtTx = new DirectDebitTransaction10
            {
                MndtRltdInf = new MandateRelatedInformation14
                {
                    MndtId = tx.Mandate.MandateId,
                    DtOfSgntr = tx.Mandate.DateOfSignature.ToDateTime(TimeOnly.MinValue)
                }
            },
            DbtrAgt = new BranchAndFinancialInstitutionIdentification6
            {
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    Bicfi = tx.Debtor.Bic?.ToString()
                }
            },
            Dbtr = new PartyIdentification135
            {
                Nm = tx.Debtor.Name
            },
            DbtrAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Iban = tx.Debtor.Iban.ToString()
                }
            },
            RmtInf = remittenceInformation
        };
    }
    
    private void ValidateAgainstSchema(XDocument document)
    {
        var schemaSet = new XmlSchemaSet();
        using var schemaStream = GetPain008SchemaStream();
        using var reader = XmlReader.Create(schemaStream);

        schemaSet.Add(null, reader);

        document.Validate(schemaSet, (sender, args) =>
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                throw new XmlSchemaValidationException($"XML validation error: {args.Message}", args.Exception);
            }
        });
    }

    private static Stream GetPain008SchemaStream()
    {
        var assembly = typeof(SepaPain00800108Writer).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .First(n => n.EndsWith("pain.008.001.08.xsd", StringComparison.OrdinalIgnoreCase));

        return assembly.GetManifestResourceStream(resourceName)
               ?? throw new InvalidOperationException($"SEPA schema resource '{resourceName}' not found.");
    }
}