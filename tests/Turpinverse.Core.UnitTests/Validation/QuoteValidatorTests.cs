using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class QuoteValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_QuoteAccountWithoutCustomerRole_FailsVr069()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", accountId: "turpin-enterprises")
        ],
        organisations:
        [
            Organisation("turpin-enterprises", roles: [])
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-069" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_QuoteContactNotAccountMember_FailsVr070()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", accountId: "highway-commission", contactId: "dick-turpin")
        ],
        organisations:
        [
            Organisation("highway-commission", roles: ["customer"], members: ["henry-clayton"])
        ],
        personas: [Persona("dick-turpin", ["turpin-enterprises"]), Persona("henry-clayton", ["highway-commission"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-070" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_QuoteDealOnDifferentAccount_FailsVr071()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", accountId: "highway-commission", dealId: "deal-1")
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])],
        deals: [Deal("deal-1", accountId: "millington-inn")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-071" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_QuoteLineProjectWrongAccount_FailsVr072()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", lines:
            [
                Line(projectId: "project-1", lineTotal: 1000, unitPrice: 1000, quantity: 1),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)
            ])
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])],
        projects: [Project("project-1", organisationId: "brazier-legal")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-072" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_QuoteIncorrectTotals_FailsVr073()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", subtotal: 999, taxTotal: 0, total: 999)
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-073" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_FewerThanEightQuotes_FailsVr068()
    {
        var canon = CreateCanon([Quote("quote-1")], organisations: [Organisation("highway-commission", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-068" && v.EntityId == "quotes");
    }

    [Fact]
    public void Validate_QuoteInvalidStatus_FailsVr074()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", status: "Pending")
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-074" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_QuoteSingleLine_FailsVr074()
    {
        var canon = CreateCanon(
        [
            Quote("quote-1", lines: [Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)])
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-074" && v.EntityId == "quote-1");
    }

    [Fact]
    public void Validate_NoSharedDealId_FailsVr071()
    {
        var canon = CreateCanon(
            Enumerable.Range(1, 8)
                .Select(i => Quote(
                    $"quote-{i}",
                    dealId: $"deal-{i}",
                    quoteNumber: $"QUO-2026-{1000 + i:D4}"))
                .ToList(),
            organisations: [Organisation("highway-commission", roles: ["customer"])],
            deals: Enumerable.Range(1, 8)
                .Select(i => Deal($"deal-{i}", accountId: "highway-commission"))
                .ToList());

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-071" && v.EntityId == "quotes");
    }

    [Fact]
    public void Validate_QuoteTaxUsesPerLineRounding_FailsVr073WhenAuthoredTaxUnrounded()
    {
        var canon = CreateCanon(
        [
            Quote(
                "quote-1",
                subtotal: 66.66m,
                taxTotal: 13.32m,
                total: 79.98m,
                lines:
                [
                    Line(lineTotal: 33.33m, unitPrice: 33.33m, quantity: 1),
                    Line(lineTotal: 33.33m, unitPrice: 33.33m, quantity: 1, productId: "prod-2")
                ])
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(
            result.Violations,
            v => v.Rule == "VR-073" && v.EntityId == "quote-1" && v.Message.Contains("taxTotal"));
    }

    private static Canon CreateCanon(
        IReadOnlyList<Quote> quotes,
        IReadOnlyList<Organisation>? organisations = null,
        IReadOnlyList<Persona>? personas = null,
        IReadOnlyList<Deal>? deals = null,
        IReadOnlyList<Project>? projects = null) =>
        new()
        {
            Version = "1.6.0",
            Personas = personas ?? [Persona("henry-clayton", ["highway-commission"])],
            Organisations = organisations ?? [Organisation("highway-commission", roles: ["customer"], members: ["henry-clayton"])],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            },
            Deals = deals ?? [],
            Projects = projects ?? [],
            Products = StandardProducts(),
            TaxRates = StandardTaxRates(),
            Quotes = quotes
        };

    private static IReadOnlyList<TaxRate> StandardTaxRates() =>
    [
        new() { TaxRateId = "tax-standard", Name = "Standard", Code = "S", Percentage = 20m, Description = "Standard VAT" },
        new() { TaxRateId = "tax-reduced", Name = "Reduced", Code = "R", Percentage = 5m, Description = "Reduced VAT" },
        new() { TaxRateId = "tax-zero", Name = "Zero", Code = "Z", Percentage = 0m, Description = "Zero VAT" },
        new() { TaxRateId = "tax-exempt", Name = "Exempt", Code = "E", Percentage = 0m, Description = "Exempt VAT" }
    ];

    private static IReadOnlyList<Product> StandardProducts() =>
        Enumerable.Range(1, 10)
            .Select(i => new Product
            {
                ProductId = i == 1 ? "highway-risk-day-rate" : $"prod-{i}",
                Name = $"Product {i}",
                Description = "Description",
                UnitPrice = 100,
                TaxRateId = "tax-standard",
                UnitOfMeasure = "each",
                Status = "active"
            })
            .ToList();

    private static Quote Quote(
        string quoteId,
        string accountId = "highway-commission",
        string? contactId = null,
        string? dealId = null,
        string status = "Draft",
        string quoteNumber = "QUO-2026-0100",
        decimal subtotal = 2000,
        decimal taxTotal = 400,
        decimal total = 2400,
        IReadOnlyList<QuoteLine>? lines = null) =>
        new()
        {
            QuoteId = quoteId,
            QuoteNumber = quoteNumber,
            AccountId = accountId,
            ContactId = contactId,
            DealId = dealId,
            Status = status,
            IssueDate = "2026-01-01",
            ExpiryDate = "2026-02-01",
            Currency = "GBP",
            Subtotal = subtotal,
            TaxTotal = taxTotal,
            Total = total,
            Lines = lines ??
            [
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)
            ]
        };

    private static QuoteLine Line(
        decimal lineTotal,
        decimal unitPrice,
        decimal quantity,
        string? productId = "highway-risk-day-rate",
        string? projectId = null) =>
        new()
        {
            Description = "Test line",
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRateId = "tax-standard",
            LineTotal = lineTotal,
            ProductId = productId,
            ProjectId = projectId
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

    private static Deal Deal(string dealId, string accountId) =>
        new()
        {
            DealId = dealId,
            DealName = "Test deal",
            AccountId = accountId,
            Stage = "Proposal",
            Amount = 1000,
            CloseDate = "2026-12-31",
            Description = "Test deal"
        };

    private static Project Project(string id, string organisationId) =>
        new()
        {
            Id = id,
            Title = "Test project",
            Summary = "Test summary",
            Image = "/images/projects/test.svg",
            Tags = ["test"],
            Links = [],
            OrganisationId = organisationId
        };
}
