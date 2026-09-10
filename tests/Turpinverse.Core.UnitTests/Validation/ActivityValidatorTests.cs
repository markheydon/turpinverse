using Turpinverse.Core.Models;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class ActivityValidatorTests
{
    private readonly CanonValidator _validator = new();

    [Fact]
    public void Validate_DuplicateActivityId_FailsVr096()
    {
        var canon = CreateCanon(
        [
            Activity("activity-001"),
            Activity("activity-001")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-096" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_FewerThanTwentyActivities_FailsVr096()
    {
        var canon = CreateCanon([Activity("activity-001")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-096" && v.EntityId == "activities");
    }

    [Fact]
    public void Validate_InvalidActivityType_FailsVr096()
    {
        var canon = CreateCanon([Activity("activity-001", type: "Phone call")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-096" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_UnknownOwnerContactId_FailsVr097()
    {
        var canon = CreateCanon([Activity("activity-001", ownerContactId: "missing-persona")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_OpenTaskWithoutDueDate_FailsVr097()
    {
        var canon = CreateCanon(
        [
            Activity("activity-001", type: "Task", status: "Open", dueDate: null)
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_DurationOnNote_FailsVr097()
    {
        var canon = CreateCanon([Activity("activity-001", type: "Note", durationMinutes: 10)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_UnknownRegardingDeal_FailsVr098()
    {
        var canon = CreateCanon(
        [
            Activity("activity-001", regardingType: "deal", regardingId: "deal-missing")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-098" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_MissingFlagshipStories_FailsVr099()
    {
        var activities = Enumerable.Range(1, 20)
            .Select(i => Activity($"activity-{i:D3}", regardingType: "contact", regardingId: "persona-1"))
            .ToList();

        var canon = CreateCanon(activities, personas: [Persona("persona-1")]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-099" && v.EntityId == "activities");
    }

    [Fact]
    public void Validate_DueDateBeforeActivityDate_FailsVr097()
    {
        var canon = CreateCanon(
        [
            Activity("activity-001", activityDate: "2026-03-10", dueDate: "2026-03-01")
        ]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_SubjectTooLong_FailsVr097()
    {
        var canon = CreateCanon([Activity("activity-001", subject: new string('x', 121))]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    [Fact]
    public void Validate_DurationAboveMaximum_FailsVr097()
    {
        var canon = CreateCanon([Activity("activity-001", type: "Call", durationMinutes: 481)]);

        var result = _validator.Validate(canon);

        Assert.Contains(result.Violations, v => v.Rule == "VR-097" && v.EntityId == "activity-001");
    }

    private static Canon CreateCanon(
        IReadOnlyList<Activity> activities,
        IReadOnlyList<Persona>? personas = null) =>
        new()
        {
            Version = "1.8.0",
            Personas = personas ?? [Persona("mary-brazier")],
            Organisations = [],
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
            Activities = activities
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

    private static Activity Activity(
        string activityId,
        string type = "Note",
        string status = "Completed",
        string regardingType = "contact",
        string regardingId = "mary-brazier",
        string ownerContactId = "mary-brazier",
        string activityDate = "2026-01-01",
        string? dueDate = null,
        int? durationMinutes = null,
        string subject = "Example activity subject") =>
        new()
        {
            ActivityId = activityId,
            Type = type,
            Subject = subject,
            Description = "Example activity description for validation tests.",
            ActivityDate = activityDate,
            DueDate = dueDate,
            Status = status,
            RegardingType = regardingType,
            RegardingId = regardingId,
            OwnerContactId = ownerContactId,
            DurationMinutes = durationMinutes
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
