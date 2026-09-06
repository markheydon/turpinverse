using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class JoinGraphValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_AccountPrimaryNotMember_FailsVr052()
    {
        var canon = CreateCanon(
            organisations:
            [
                Org("epping-forest-authority", ["william-hargreaves"], primaryContactId: "henry-clayton")
            ],
            personas: [Persona("william-hargreaves"), Persona("henry-clayton")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-052" && v.EntityId == "epping-forest-authority");
    }

    [Fact]
    public void Validate_DealMainNotAccountMember_FailsVr054()
    {
        var canon = CreateCanon(
            organisations: [Org("epping-forest-authority", ["william-hargreaves"])],
            personas: [Persona("william-hargreaves"), Persona("henry-clayton")],
            deals:
            [
                Deal("deal-test", "epping-forest-authority", contactId: "henry-clayton")
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-054" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_StakeholderOnlyNonMember_PassesVr054()
    {
        var canon = CreateCanon(
            organisations: [Org("epping-forest-authority", ["william-hargreaves"])],
            personas: [Persona("william-hargreaves"), Persona("henry-clayton")],
            deals:
            [
                Deal("deal-test", "epping-forest-authority", stakeholders: ["henry-clayton"])
            ]);

        var result = _validator.Validate(canon);

        Assert.DoesNotContain(result.Violations, v => v.Rule == "VR-054" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_UnknownStakeholder_FailsVr055()
    {
        var canon = CreateCanon(
            organisations: [Org("turpin-enterprises", ["dick-turpin"])],
            personas: [Persona("dick-turpin")],
            deals:
            [
                Deal("deal-test", "turpin-enterprises", contactId: "dick-turpin", stakeholders: ["missing-person"])
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-055" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_DuplicateStakeholder_FailsVr055()
    {
        var canon = CreateCanon(
            organisations: [Org("turpin-enterprises", ["dick-turpin", "ned-palmer"])],
            personas: [Persona("dick-turpin"), Persona("ned-palmer")],
            deals:
            [
                Deal("deal-test", "turpin-enterprises", contactId: "dick-turpin", stakeholders: ["ned-palmer", "ned-palmer"])
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-055" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_MainEqualsStakeholder_FailsVr055()
    {
        var canon = CreateCanon(
            organisations: [Org("turpin-enterprises", ["dick-turpin"])],
            personas: [Persona("dick-turpin")],
            deals:
            [
                Deal("deal-test", "turpin-enterprises", contactId: "dick-turpin", stakeholders: ["dick-turpin"])
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-055" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_UnknownProjectDealAndCase_FailsVr056()
    {
        var canon = CreateCanon(
            organisations: [Org("turpin-enterprises", ["dick-turpin"])],
            personas: [Persona("dick-turpin")],
            projects:
            [
                Project("project-test", "turpin-enterprises", dealId: "missing-deal", caseIds: ["missing-case"])
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-056" && v.EntityId == "project-test");
    }

    [Fact]
    public void Validate_UnknownEventPipelineIds_FailsVr057()
    {
        var canon = CreateCanon(
            events:
            [
                new CanonEvent
                {
                    Id = "event-test",
                    Date = "1739",
                    Title = "Event",
                    Description = "Description",
                    Category = "historical",
                    DealIds = ["missing-deal"],
                    CaseIds = ["missing-case"]
                }
            ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-057" && v.EntityId == "event-test");
    }

    [Fact]
    public void Validate_MissingAccountOnPipelineRecord_FailsVr053()
    {
        var canon = CreateCanon(
            organisations: [Org("turpin-enterprises", ["dick-turpin"])],
            personas: [Persona("dick-turpin")],
            deals: [Deal("deal-test", "missing-account", contactId: "dick-turpin")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-053" && v.EntityId == "deal-test");
    }

    [Fact]
    public void Validate_OrganisationWithZeroMembersAndNoPrimary_PassesJoinRules()
    {
        var canon = CreateCanon(
            organisations: [Org("prospect-account", [])]);

        var result = _validator.Validate(canon);

        Assert.DoesNotContain(
            result.Violations,
            v => v.EntityId == "prospect-account" && v.Rule is "VR-052" or "VR-003");
    }

    [Fact]
    public async Task Validate_LoadedCanon_NamedRowsMatchVr058()
    {
        var repository = new Turpinverse.Data.Repositories.JsonCanonRepository();
        var canon = await repository.LoadAsync(TestContext.Current.CancellationToken);
        var result = _validator.Validate(canon);

        Assert.DoesNotContain(result.Violations, v => v.Rule == "VR-058");
    }

    [Fact]
    public async Task Validate_LoadedCanon_HasAuthoredOrganisationPrimaries()
    {
        var repository = new Turpinverse.Data.Repositories.JsonCanonRepository();
        var canon = await repository.LoadAsync(TestContext.Current.CancellationToken);

        Assert.Equal("dick-turpin", canon.Organisations.Single(o => o.Id == "turpin-enterprises").PrimaryContactId);
        Assert.Equal("samuel-gregory", canon.Organisations.Single(o => o.Id == "essex-gang").PrimaryContactId);
        Assert.Equal(10, canon.Organisations.Count(o => !string.IsNullOrWhiteSpace(o.PrimaryContactId)));
    }

    [Fact]
    public async Task Validate_LoadedCanon_ProjectsUseJoinGraphPeopleFields()
    {
        var repository = new Turpinverse.Data.Repositories.JsonCanonRepository();
        var canon = await repository.LoadAsync(TestContext.Current.CancellationToken);

        foreach (var project in canon.Projects)
        {
            Assert.True(project.LinkedPersonaIds.Count > 0, $"Project '{project.Id}' should have linked people");
        }
    }

    private static Canon CreateCanon(
        IReadOnlyList<Persona>? personas = null,
        IReadOnlyList<Organisation>? organisations = null,
        IReadOnlyList<Deal>? deals = null,
        IReadOnlyList<Project>? projects = null,
        IReadOnlyList<CanonEvent>? events = null) =>
        new()
        {
            Version = "1.1.0",
            Personas = personas ?? [],
            Organisations = organisations ?? [],
            Events = events ?? [],
            Aliases = [],
            ToneGuidelines = new ToneGuidelines
            {
                Version = "1.0.0",
                Principles = ["A", "B", "C"],
                Examples = [],
                ForbiddenPatterns = []
            },
            Deals = deals ?? [],
            Cases = [],
            Projects = projects ?? []
        };

    private static Persona Persona(string id) =>
        new()
        {
            Id = id,
            DisplayName = id,
            HistoricalName = id,
            Title = "Title",
            Biography = "Bio",
            HistoricalAnchor = "Legend",
            IsFictionalExtension = false,
            OrganisationIds = ["turpin-enterprises"],
            Status = "active",
            Email = $"{id}@turpinverse.uk"
        };

    private static Organisation Org(string id, string[] members, string? primaryContactId = null) =>
        new()
        {
            Id = id,
            TradingName = id,
            Description = "Description",
            Industry = "Industry",
            HistoricalAnchor = "Legend",
            MemberPersonaIds = members,
            PrimaryContactId = primaryContactId,
            Status = "active",
            RegisteredOffice = TestAddresses.SampleOffice
        };

    private static Deal Deal(
        string dealId,
        string accountId,
        string? contactId = null,
        IReadOnlyList<string>? stakeholders = null) =>
        new()
        {
            DealId = dealId,
            DealName = dealId,
            AccountId = accountId,
            ContactId = contactId,
            StakeholderContactIds = stakeholders ?? [],
            Stage = "Prospecting",
            Amount = 100,
            CloseDate = "2026-01-01",
            Description = "Description"
        };

    private static Project Project(
        string id,
        string organisationId,
        string? dealId = null,
        IReadOnlyList<string>? caseIds = null) =>
        new()
        {
            Id = id,
            Title = id,
            Summary = "Summary",
            Image = "/img.png",
            Tags = ["tag"],
            Links = [new FeaturedLink { Url = "https://example.com", Label = "Link" }],
            OrganisationId = organisationId,
            DealId = dealId,
            CaseIds = caseIds ?? []
        };
}
