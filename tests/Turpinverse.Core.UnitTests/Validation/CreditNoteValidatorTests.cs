using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CreditNoteValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_CreditNoteWithMismatchedInvoiceAccount_FailsVr082()
    {
        var canon = CreateCanon(
            invoices: [Invoice("inv-1", accountId: "highway-commission")],
            creditNotes: [CreditNote("crn-1", accountId: "millington-inn", invoiceId: "inv-1")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-082" && v.EntityId == "crn-1");
    }

    [Fact]
    public void Validate_CreditNoteIncorrectSubtotal_FailsVr083()
    {
        var creditNote = CreditNote("crn-1") with { Subtotal = 999 };
        var canon = CreateCanon(creditNotes: [creditNote]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-083" && v.EntityId == "crn-1");
    }

    [Fact]
    public void Validate_FewerThanThreeCreditNotes_FailsVr082()
    {
        var canon = CreateCanon(
            creditNotes:
            [
                CreditNote("crn-1"),
                CreditNote("crn-2", creditNoteNumber: "CRN-2026-0002")
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-082" && v.EntityId == "creditNotes");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Invoice>? invoices = null,
        IReadOnlyList<CreditNote>? creditNotes = null) =>
        new()
        {
            Version = "1.6.0",
            Personas = [Persona("henry-clayton", ["highway-commission"])],
            Organisations =
            [
                Organisation("highway-commission", roles: ["customer"], members: ["henry-clayton"]),
                Organisation("millington-inn", roles: ["customer"])
            ],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            },
            Products = StandardProducts(),
            TaxRates =
            [
                new() { TaxRateId = "tax-standard", Name = "Standard", Code = "S", Percentage = 20m, Description = "Standard VAT" },
                new() { TaxRateId = "tax-zero", Name = "Zero", Code = "Z", Percentage = 0m, Description = "Zero VAT" }
            ],
            Invoices = invoices ?? [],
            Payments = [],
            CreditNotes = creditNotes ?? []
        };

    private static CreditNote CreditNote(
        string creditNoteId,
        string creditNoteNumber = "CRN-2026-0001",
        string accountId = "highway-commission",
        string? invoiceId = null) =>
        new()
        {
            CreditNoteId = creditNoteId,
            CreditNoteNumber = creditNoteNumber,
            AccountId = accountId,
            ContactId = "henry-clayton",
            InvoiceId = invoiceId,
            IssueDate = "2026-01-01",
            Currency = "GBP",
            Subtotal = 185,
            TaxTotal = 37,
            Total = 222,
            Lines =
            [
                new CreditNoteLine
                {
                    Description = "Advisory hour credit",
                    Quantity = 1,
                    UnitPrice = 185,
                    TaxRateId = "tax-standard",
                    LineTotal = 185,
                    ProductId = "highway-risk-day-rate"
                },
                new CreditNoteLine
                {
                    Description = "Rounding adjustment",
                    Quantity = 1,
                    UnitPrice = 0,
                    TaxRateId = "tax-zero",
                    LineTotal = 0
                }
            ]
        };

    private static Invoice Invoice(string invoiceId, string accountId) =>
        new()
        {
            InvoiceId = invoiceId,
            InvoiceNumber = "INV-2026-0001",
            AccountId = accountId,
            ContactId = "henry-clayton",
            Status = "Paid",
            IssueDate = "2026-01-01",
            DueDate = "2026-02-01",
            Currency = "GBP",
            Subtotal = 1000,
            TaxTotal = 200,
            Total = 1200,
            AmountDue = 0,
            Lines =
            [
                new InvoiceLine
                {
                    Description = "Service",
                    Quantity = 1,
                    UnitPrice = 500,
                    TaxRateId = "tax-standard",
                    LineTotal = 500,
                    ProductId = "highway-risk-day-rate"
                },
                new InvoiceLine
                {
                    Description = "Service",
                    Quantity = 1,
                    UnitPrice = 500,
                    TaxRateId = "tax-standard",
                    LineTotal = 500,
                    ProductId = "highway-risk-day-rate"
                }
            ]
        };

    private static Persona Persona(string id, IReadOnlyList<string> organisationIds) =>
        new()
        {
            Id = id,
            DisplayName = "Example Person",
            HistoricalName = "Example Person",
            Title = "Tester",
            Biography = "Bio",
            HistoricalAnchor = "Legend",
            IsFictionalExtension = false,
            Status = "active",
            OrganisationIds = organisationIds,
            Email = $"{id}@turpinverse.uk"
        };

    private static Organisation Organisation(string id, IReadOnlyList<string> roles, IReadOnlyList<string>? members = null) =>
        new()
        {
            Id = id,
            TradingName = id,
            Description = "Test organisation",
            Industry = "Test",
            HistoricalAnchor = "Legend",
            Status = "active",
            Roles = roles,
            MemberPersonaIds = members ?? [],
            RegisteredOffice = TestAddresses.SampleOffice
        };

    private static IReadOnlyList<Product> StandardProducts() =>
    [
        new()
        {
            ProductId = "highway-risk-day-rate",
            Name = "Day rate",
            Description = "Desc",
            UnitPrice = 1200,
            TaxRateId = "tax-standard",
            UnitOfMeasure = "day",
            Status = "active"
        }
    ];
}
