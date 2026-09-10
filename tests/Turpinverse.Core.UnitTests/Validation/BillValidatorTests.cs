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

    [Fact]
    public void Validate_BillDealBelongsToDifferentSupplier_FailsVr093()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1", dealId: "deal-1")
        ],
        deals:
        [
            new Deal
            {
                DealId = "deal-1",
                DealName = "Mismatch",
                AccountId = "highway-commission",
                Stage = "Proposal",
                Amount = 1000,
                CloseDate = "2026-12-31",
                Description = "Deal on a different supplier account"
            }
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-093" && v.EntityId == "bill-1");
    }

    [Fact]
    public void Validate_PaymentExceedsBillTotal_FailsVr081()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1", total: 2400, amountDue: 0)
        ],
        payments: [Payment("pay-1", billId: "bill-1", amount: 3000)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-081" && v.EntityId == "bill-1");
    }

    [Fact]
    public void Validate_PaymentWithUnknownBillId_FailsVr081()
    {
        var canon = CreateCanon(
        [
            Bill("bill-1")
        ],
        payments: [Payment("pay-1", billId: "bill-missing")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-081" && v.EntityId == "pay-1");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Bill> bills,
        IReadOnlyList<Organisation>? organisations = null,
        IReadOnlyList<Project>? projects = null,
        IReadOnlyList<Deal>? deals = null,
        IReadOnlyList<Payment>? payments = null) =>
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
            Deals = deals ?? [],
            Bills = bills,
            Payments = payments ?? []
        };

    private static Bill Bill(
        string billId,
        string supplierAccountId = "king-equine-trading",
        decimal amountDue = 2400,
        decimal total = 2400,
        string? dealId = null,
        IReadOnlyList<BillLine>? lines = null) =>
        new()
        {
            BillId = billId,
            BillNumber = "BILL-2026-0001",
            SupplierAccountId = supplierAccountId,
            DealId = dealId,
            Status = amountDue == 0 ? "Paid" : "Authorised",
            IssueDate = "2026-01-01",
            DueDate = "2026-02-01",
            Currency = "GBP",
            Subtotal = 2000,
            TaxTotal = total - 2000,
            Total = total,
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

    private static Payment Payment(
        string paymentId,
        string? billId = null,
        decimal amount = 1000) =>
        new()
        {
            PaymentId = paymentId,
            PaymentDate = "2026-01-15",
            Amount = amount,
            Method = "Bank transfer",
            BillId = billId
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
