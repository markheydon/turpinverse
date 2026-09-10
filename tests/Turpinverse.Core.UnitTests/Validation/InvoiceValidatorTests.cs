using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class InvoiceValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_InvoiceWithCase011_FailsVr077()
    {
        var canon = CreateCanon(
        [
            Invoice("inv-1", caseId: "case-011")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-077" && v.EntityId == "inv-1");
    }

    [Fact]
    public void Validate_PaymentWithBothTargets_FailsVr081()
    {
        var canon = CreateCanon(
            [Invoice("inv-1")],
            payments: [Payment("pay-1", invoiceId: "inv-1", billId: "bill-1")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-081" && v.EntityId == "pay-1");
    }

    [Fact]
    public void Validate_PaymentExceedsInvoiceTotal_FailsVr081()
    {
        var canon = CreateCanon(
            [Invoice("inv-1", total: 1200, amountDue: 0)],
            payments: [Payment("pay-1", invoiceId: "inv-1", amount: 1500)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-081" && v.EntityId == "inv-1");
    }

    [Fact]
    public void Validate_InvoiceIncorrectAmountDue_FailsVr079()
    {
        var canon = CreateCanon(
            [Invoice("inv-1", total: 1200, amountDue: 500)],
            payments: [Payment("pay-1", invoiceId: "inv-1", amount: 1200)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-079" && v.EntityId == "inv-1");
    }

    [Fact]
    public void Validate_FewerThanTwelveInvoices_FailsVr075()
    {
        var canon = CreateCanon(
            Enumerable.Range(1, 11)
                .Select(i => Invoice($"inv-{i}", invoiceNumber: $"INV-2026-{1000 + i:D4}"))
                .ToList());

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-075" && v.EntityId == "invoices");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Invoice> invoices,
        IReadOnlyList<Payment>? payments = null) =>
        new()
        {
            Version = "1.6.0",
            Personas = [Persona("henry-clayton", ["highway-commission"])],
            Organisations =
            [
                Organisation("highway-commission", roles: ["customer"], members: ["henry-clayton"])
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
                new() { TaxRateId = "tax-standard", Name = "Standard", Code = "S", Percentage = 20m, Description = "Standard VAT" }
            ],
            Invoices = invoices,
            Payments = payments ?? [],
            CreditNotes = []
        };

    private static Invoice Invoice(
        string invoiceId,
        string? caseId = null,
        decimal total = 1200,
        decimal amountDue = 1200,
        string invoiceNumber = "INV-2026-0001") =>
        new()
        {
            InvoiceId = invoiceId,
            InvoiceNumber = invoiceNumber,
            AccountId = "highway-commission",
            ContactId = "henry-clayton",
            CaseId = caseId,
            Status = amountDue == 0 ? "Paid" : "Authorised",
            IssueDate = "2026-01-01",
            DueDate = "2026-02-01",
            Currency = "GBP",
            Subtotal = 1000,
            TaxTotal = 200,
            Total = total,
            AmountDue = amountDue,
            Lines =
            [
                Line(500),
                Line(500)
            ]
        };

    private static InvoiceLine Line(decimal lineTotal) =>
        new()
        {
            Description = "Service",
            Quantity = 1,
            UnitPrice = lineTotal,
            TaxRateId = "tax-standard",
            LineTotal = lineTotal,
            ProductId = "highway-risk-day-rate"
        };

    private static Payment Payment(
        string paymentId,
        string? invoiceId = null,
        string? billId = null,
        decimal amount = 1000) =>
        new()
        {
            PaymentId = paymentId,
            PaymentDate = "2026-01-15",
            Amount = amount,
            Method = "Bank transfer",
            InvoiceId = invoiceId,
            BillId = billId
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

    private static Organisation Organisation(
        string id,
        IReadOnlyList<string> roles,
        IReadOnlyList<string>? members = null) =>
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
