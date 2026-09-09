using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class LeadValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_ConvertedLeadWithoutContactId_FailsVr066()
    {
        var canon = CreateCanon(
        [
            Lead("lead-1", status: "Converted", convertedContactId: null)
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-066" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_ConvertedLeadWithUnknownContactId_FailsVr066()
    {
        var canon = CreateCanon(
        [
            Lead("lead-1", status: "Converted", convertedContactId: "missing-persona")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-066" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_NonConvertedLeadWithConvertedContactId_FailsVr066()
    {
        var canon = CreateCanon(
        [
            Lead("lead-1", status: "Qualified", convertedContactId: "persona-1")
        ],
        personas: [Persona("persona-1")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-066" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_LeadWithUnknownAccountId_FailsVr067()
    {
        var canon = CreateCanon(
        [
            Lead("lead-1", accountId: "missing-org")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-067" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_DuplicateLeadId_FailsVr065()
    {
        var canon = CreateCanon(
        [
            Lead("lead-1"),
            Lead("lead-1")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-065" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_FewerThanTenLeads_FailsVr065()
    {
        var canon = CreateCanon([Lead("lead-1")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-065" && v.EntityId == "leads");
    }

    [Fact]
    public void Validate_InvalidLeadStatus_FailsVr065()
    {
        var canon = CreateCanon([Lead("lead-1", status: "Pending")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-065" && v.EntityId == "lead-1");
    }

    [Fact]
    public void Validate_ConvertedLeadWithExistingPersona_PassesConversionRule()
    {
        var leads = Enumerable.Range(1, 10)
            .Select(i => Lead($"lead-{i:D3}", status: i == 1 ? "Converted" : "New", convertedContactId: i == 1 ? "persona-1" : null))
            .ToList();

        var canon = CreateCanon(leads, personas: [Persona("persona-1")]);

        var result = _validator.Validate(canon);

        Assert.DoesNotContain(result.Violations, v => v.Rule == "VR-066");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Lead> leads,
        IReadOnlyList<Persona>? personas = null,
        IReadOnlyList<Organisation>? organisations = null) =>
        new()
        {
            Version = "1.5.0",
            Personas = personas ?? [],
            Organisations = organisations ?? [],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            },
            TaxRates = StandardTaxRates(),
            Products = StandardProducts(),
            Leads = leads
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
                ProductId = $"prod-{i}",
                Name = $"Product {i}",
                Description = "Description",
                UnitPrice = 100,
                TaxRateId = "tax-standard",
                UnitOfMeasure = "each",
                Status = "active"
            })
            .ToList();

    private static Lead Lead(
        string leadId,
        string status = "New",
        string source = "Web",
        string? accountId = null,
        string? convertedContactId = null) =>
        new()
        {
            LeadId = leadId,
            CompanyName = "Example Company",
            ContactName = "Example Contact",
            Status = status,
            Source = source,
            Description = "Example lead description for validation tests.",
            AccountId = accountId,
            ConvertedContactId = convertedContactId
        };

    private static Persona Persona(string id) =>
        new()
        {
            Id = id,
            DisplayName = "Example Person",
            HistoricalName = "Example Person",
            Title = "Title",
            Status = "active",
            Biography = "Bio",
            HistoricalAnchor = "Legend",
            IsFictionalExtension = false,
            OrganisationIds = ["turpin-enterprises"],
            Email = "example@turpinverse.uk"
        };
}
