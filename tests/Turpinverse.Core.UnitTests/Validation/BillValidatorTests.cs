using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class BillValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_BillSupplierWithoutSupplierRole_FailsVr091()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1", supplierAccountId: "highway-commission")
        ],
        organisations:
        [
            Organisation("highway-commission", roles: ["customer"])
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-091" && v.EntityId == "bill-1");
    }

    [Fact]
    public void Validate_BillLineProjectWithoutMatchingSupplierOrg_Allowed()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1", lines:
            [
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1, projectId: "black-bess-route-optimiser"),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)
            ])
        ],
        projects:
        [
            new Project
            {
                Id = "black-bess-route-optimiser",
                Title = "Black Bess Route Optimiser",
                Summary = "Summary",
                Image = "/img.png",
                Tags = ["logistics"],
                Links = [],
                OrganisationId = "turpin-enterprises"
            }
        ]);

        var result = _validator.Validate(canon);

        Assert.DoesNotContain(result.Violations, v => v.Rule.StartsWith("VR-09", StringComparison.Ordinal) && v.EntityId == "bill-1");
    }

    [Fact]
    public void Validate_BillAmountDueMismatch_FailsVr094()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1", amountDue: 999)
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-094" && v.EntityId == "bill-1");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Bill> bills,
        IReadOnlyList<Organisation>? organisations = null,
        IReadOnlyList<Project>? projects = null) =>
        new()
        {
            Version = "1.7.0",
            Personas = [],
            Organisations = organisations ??
            [
                Organisation("king-equine-trading", roles: ["supplier", "customer"])
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
            Projects = projects ?? [],
            Bills = bills
        };

    private static Bill Bill(
        string billId,
        string supplierAccountId = "king-equine-trading",
        decimal amountDue = 2400,
        IReadOnlyList<BillLine>? lines = null) =>
        new()
        {
            BillId = billId,
            BillNumber = "BILL-2026-0001",
            SupplierAccountId = supplierAccountId,
            Status = "Authorised",
            IssueDate = "2026-01-01",
            DueDate = "2026-02-01",
            Currency = "GBP",
            Subtotal = 2000,
            TaxTotal = 400,
            Total = 2400,
            AmountDue = amountDue,
            Lines = lines ??
            [
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)
            ]
        };

    private static BillLine Line(
        decimal lineTotal,
        decimal unitPrice,
        decimal quantity,
        string? projectId = null) =>
        new()
        {
            Description = "Service",
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRateId = "tax-standard",
            LineTotal = lineTotal,
            ProductId = "highway-risk-day-rate",
            ProjectId = projectId
        };

    private static Organisation Organisation(string id, IReadOnlyList<string> roles) =>
        new()
        {
            Id = id,
            TradingName = id,
            Description = "Test organisation",
            Industry = "Test",
            HistoricalAnchor = "Legend",
            Status = "active",
            Roles = roles,
            MemberPersonaIds = [],
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
