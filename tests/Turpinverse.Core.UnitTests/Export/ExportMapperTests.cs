using Turpinverse.Core.Export;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.UnitTests.Export;

[Trait("Category", "CsvExport")]
public class ExportMapperTests
{
    [Fact]
    public void MapContacts_EmitsOneRowPerMembership()
    {
        var canon = new Canon
        {
            Version = "1.1.0",
            Personas =
            [
                new Persona
                {
                    Id = "dick-turpin",
                    DisplayName = "Richard Turpin",
                    HistoricalName = "Richard Turpin",
                    Title = "CEO",
                    Biography = "Bio",
                    HistoricalAnchor = "Legend",
                    IsFictionalExtension = false,
                    OrganisationIds = ["essex-gang", "turpin-enterprises"],
                    Status = "deceased",
                    Email = "dick-turpin@turpinverse.uk"
                }
            ],
            Organisations = [],
            Events = [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            }
        };

        var rows = ExportMapper.MapContacts(canon);

        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Equal("dick-turpin", row.ContactId));
        Assert.Equal(["essex-gang", "turpin-enterprises"], rows.Select(r => r.AccountId).OrderBy(id => id).ToArray());
        Assert.All(rows, row => Assert.Equal("dick-turpin@turpinverse.uk", row.Email));
    }

    [Fact]
    public void MapDeal_IncludesOptionalMainAndStakeholders()
    {
        var export = ExportMapper.MapDeal(new Deal
        {
            DealId = "deal-008",
            DealName = "Deal",
            AccountId = "epping-forest-authority",
            ContactId = "william-hargreaves",
            StakeholderContactIds = ["henry-clayton"],
            Stage = "Qualification",
            Amount = 100,
            CloseDate = "2026-01-01",
            Description = "Description"
        });

        Assert.Equal("william-hargreaves", export.ContactId);
        Assert.Equal("henry-clayton", export.StakeholderContactIds);
    }

    [Fact]
    public void MapProject_UsesMainUnionStakeholdersColumns()
    {
        var export = ExportMapper.MapProject(new Project
        {
            Id = "palmer-identity-vault",
            Title = "Palmer Identity Vault",
            Summary = "Summary",
            Image = "/img.png",
            Tags = ["identity"],
            Links = [new FeaturedLink { Url = "https://example.com", Label = "Link" }],
            OrganisationId = "brazier-legal",
            ContactId = "mary-brazier",
            StakeholderContactIds = ["dick-turpin"]
        });

        Assert.Equal("mary-brazier", export.ContactId);
        Assert.Equal("dick-turpin", export.StakeholderContactIds);
        Assert.Equal(string.Empty, export.DealId);
        Assert.Equal(string.Empty, export.CaseIds);
    }

    [Fact]
    public void MapLead_IncludesOptionalFieldsAsEmptyStrings()
    {
        var export = ExportMapper.MapLead(new Lead
        {
            LeadId = "lead-004",
            CompanyName = "Palmer & Associates",
            ContactName = "John Palmer",
            Status = "Converted",
            Source = "Web",
            Description = "Alias confusion inbound lead.",
            ConvertedContactId = "dick-turpin"
        });

        Assert.Equal("lead-004", export.LeadId);
        Assert.Equal("John Palmer", export.ContactName);
        Assert.Equal("dick-turpin", export.ConvertedContactId);
        Assert.Equal(string.Empty, export.AccountId);
        Assert.Equal(string.Empty, export.Rating);
    }

    [Fact]
    public void MapQuote_IncludesOptionalFieldsAsEmptyStrings()
    {
        var export = ExportMapper.MapQuote(new Quote
        {
            QuoteId = "quote-001",
            QuoteNumber = "QUO-2026-0035",
            AccountId = "highway-commission",
            Status = "Draft",
            IssueDate = "2026-06-01",
            ExpiryDate = "2026-09-15",
            Currency = "GBP",
            Subtotal = 10240,
            TaxTotal = 2048,
            Total = 12288,
            Lines =
            [
                new QuoteLine
                {
                    Description = "Day rate",
                    Quantity = 1,
                    UnitPrice = 1200,
                    TaxRateId = "tax-standard",
                    LineTotal = 1200,
                    ProductId = "highway-risk-day-rate"
                },
                new QuoteLine
                {
                    Description = "Surcharge",
                    Quantity = 1,
                    UnitPrice = 320,
                    TaxRateId = "tax-standard",
                    LineTotal = 320,
                    ProductId = "overnight-logistics-surcharge"
                }
            ]
        });

        Assert.Equal("quote-001", export.QuoteId);
        Assert.Equal("QUO-2026-0035", export.QuoteNumber);
        Assert.Equal(string.Empty, export.ContactId);
        Assert.Equal(string.Empty, export.DealId);
        Assert.Equal(string.Empty, export.Notes);
    }
}
