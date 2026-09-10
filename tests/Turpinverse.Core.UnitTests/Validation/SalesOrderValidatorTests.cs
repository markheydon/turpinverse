using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class SalesOrderValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_SalesOrderAccountWithoutCustomerRole_FailsVr085()
    {
        var canon = CreateCanon(
        [
            SalesOrder("so-1", accountId: "turpin-enterprises")
        ],
        organisations:
        [
            Organisation("turpin-enterprises", roles: [])
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-085" && v.EntityId == "so-1");
    }

    [Fact]
    public void Validate_SalesOrderLineQuoteOnDifferentAccount_FailsVr087()
    {
        var canon = CreateCanon(
        [
            SalesOrder("so-1", lines:
            [
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1, quoteId: "quote-1")
            ])
        ],
        organisations: [Organisation("highway-commission", roles: ["customer"])],
        quotes:
        [
            new Quote
            {
                QuoteId = "quote-1",
                QuoteNumber = "QUO-2026-0001",
                AccountId = "millington-inn",
                Status = "Sent",
                IssueDate = "2026-01-01",
                ExpiryDate = "2026-02-01",
                Currency = "GBP",
                Subtotal = 2000,
                TaxTotal = 400,
                Total = 2400,
                Lines =
                [
                    new QuoteLine
                    {
                        Description = "Service",
                        Quantity = 1,
                        UnitPrice = 1000,
                        TaxRateId = "tax-standard",
                        LineTotal = 1000,
                        ProductId = "highway-risk-day-rate"
                    },
                    new QuoteLine
                    {
                        Description = "Service B",
                        Quantity = 1,
                        UnitPrice = 1000,
                        TaxRateId = "tax-standard",
                        LineTotal = 1000,
                        ProductId = "highway-risk-day-rate"
                    }
                ]
            }
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-087" && v.EntityId == "so-1");
    }

    [Fact]
    public void Validate_SalesOrderRequestedDeliveryBeforeOrderDate_FailsVr088()
    {
        var canon = CreateCanon(
        [
            SalesOrder("so-1", orderDate: "2026-06-01", requestedDeliveryDate: "2026-05-01")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-088" && v.EntityId == "so-1");
    }

    private static Canon CreateCanon(
        IReadOnlyList<SalesOrder> salesOrders,
        IReadOnlyList<Organisation>? organisations = null,
        IReadOnlyList<Quote>? quotes = null) =>
        new()
        {
            Version = "1.7.0",
            Personas = [],
            Organisations = organisations ??
            [
                Organisation("highway-commission", roles: ["customer"])
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
            Quotes = quotes ?? [],
            SalesOrders = salesOrders
        };

    private static SalesOrder SalesOrder(
        string salesOrderId,
        string accountId = "highway-commission",
        string orderDate = "2026-01-01",
        string? requestedDeliveryDate = null,
        IReadOnlyList<SalesOrderLine>? lines = null) =>
        new()
        {
            SalesOrderId = salesOrderId,
            OrderNumber = "SO-2026-0001",
            AccountId = accountId,
            Status = "Confirmed",
            OrderDate = orderDate,
            RequestedDeliveryDate = requestedDeliveryDate,
            Currency = "GBP",
            Subtotal = 2000,
            TaxTotal = 400,
            Total = 2400,
            Lines = lines ??
            [
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1),
                Line(lineTotal: 1000, unitPrice: 1000, quantity: 1)
            ]
        };

    private static SalesOrderLine Line(
        decimal lineTotal,
        decimal unitPrice,
        decimal quantity,
        string? quoteId = null) =>
        new()
        {
            Description = "Service",
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRateId = "tax-standard",
            LineTotal = lineTotal,
            ProductId = "highway-risk-day-rate",
            QuoteId = quoteId
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
