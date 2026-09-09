using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CatalogueValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_ProductWithUnknownTaxRate_FailsVr062()
    {
        var canon = CreateCanon(
            taxRates: [TaxRate("tax-standard", 20m)],
            products: [Product("prod-1", "tax-missing")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-062" && v.EntityId == "prod-1");
    }

    [Fact]
    public void Validate_DuplicateProductId_FailsVr061()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products:
            [
                Product("prod-1", "tax-standard"),
                Product("prod-1", "tax-standard")
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-061" && v.EntityId == "prod-1");
    }

    [Fact]
    public void Validate_InvalidOrganisationRole_FailsVr063()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")],
            organisations: [Org("test-org", roles: ["wholesale"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-063" && v.EntityId == "test-org");
    }

    [Fact]
    public void Validate_TurpinEnterprisesWithRoles_FailsVr063()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")],
            organisations: [Org("turpin-enterprises", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-063" && v.EntityId == "turpin-enterprises");
    }

    [Fact]
    public void Validate_KingEquineWithoutSupplier_FailsVr064()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")],
            organisations: [Org("king-equine-trading", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-064" && v.EntityId == "king-equine-trading");
    }

    [Fact]
    public void Validate_MissingTaxRateRow_FailsVr060()
    {
        var canon = CreateCanon(
            taxRates: [TaxRate("tax-standard", 20m)],
            products: [Product("prod-1", "tax-standard")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-060");
    }

    [Fact]
    public void Validate_FewerThanTenProducts_FailsVr061()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-061" && v.EntityId == "products");
    }

    [Fact]
    public void Validate_ProductWithInvalidStatus_FailsVr061()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard", status: "archived")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-061" && v.EntityId == "prod-1");
    }

    [Fact]
    public void Validate_ProductWithNegativeUnitPrice_FailsVr061()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard", unitPrice: -1)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-061" && v.EntityId == "prod-1");
    }

    [Fact]
    public void Validate_ProductWithInvalidUnitOfMeasure_FailsVr061()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard", unitOfMeasure: "crate")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-061" && v.EntityId == "prod-1");
    }

    [Fact]
    public void Validate_BrazierLegalWithoutPartner_FailsVr064()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")],
            organisations: [Org("brazier-legal", roles: ["customer", "supplier"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-064" && v.EntityId == "brazier-legal");
    }

    [Fact]
    public void Validate_YorkAssizeWithoutPartner_FailsVr064()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")],
            organisations: [Org("york-assize-court", roles: ["customer"])]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-064" && v.EntityId == "york-assize-court");
    }

    [Fact]
    public void Validate_MissingNamedOrganisation_FailsVr064()
    {
        var canon = CreateCanon(
            taxRates: StandardTaxRates(),
            products: [Product("prod-1", "tax-standard")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-064" && v.EntityId == "king-equine-trading");
    }

    private static Canon CreateCanon(
        IReadOnlyList<TaxRate>? taxRates = null,
        IReadOnlyList<Product>? products = null,
        IReadOnlyList<Organisation>? organisations = null) =>
        new()
        {
            Version = "1.4.0",
            Personas = [],
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
            TaxRates = taxRates ?? StandardTaxRates(),
            Products = products ?? []
        };

    private static IReadOnlyList<TaxRate> StandardTaxRates() =>
    [
        TaxRate("tax-standard", 20m),
        TaxRate("tax-reduced", 5m),
        TaxRate("tax-zero", 0m),
        TaxRate("tax-exempt", 0m)
    ];

    private static TaxRate TaxRate(string id, decimal percentage) =>
        new()
        {
            TaxRateId = id,
            Name = id,
            Code = "X",
            Percentage = percentage,
            Description = "Test tax rate"
        };

    private static Product Product(
        string productId,
        string taxRateId,
        string status = "active",
        decimal unitPrice = 100,
        string unitOfMeasure = "each") =>
        new()
        {
            ProductId = productId,
            Name = productId,
            Description = "Description",
            UnitPrice = unitPrice,
            TaxRateId = taxRateId,
            UnitOfMeasure = unitOfMeasure,
            Status = status
        };

    private static Organisation Org(string id, IReadOnlyList<string>? roles = null) =>
        new()
        {
            Id = id,
            TradingName = id,
            Description = "Description",
            Industry = "Industry",
            HistoricalAnchor = "Legend",
            MemberPersonaIds = [],
            Status = "active",
            RegisteredOffice = TestAddresses.SampleOffice,
            Roles = roles ?? []
        };
}
