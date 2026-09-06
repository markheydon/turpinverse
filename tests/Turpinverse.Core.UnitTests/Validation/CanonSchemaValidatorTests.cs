using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class CanonSchemaValidatorTests
{
    private readonly ICanonRepository _repository = new Turpinverse.Data.Repositories.JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_PassesJsonSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var violations = CanonSchemaValidator.Validate(canon);

        Assert.Empty(violations);
    }

    [Fact]
    public void TryValidateJson_RejectsInvalidJson()
    {
        var valid = CanonSchemaValidator.TryValidateJson("{not-json", out var violations);

        Assert.False(valid);
        Assert.NotEmpty(violations);
    }

    [Fact]
    public async Task Validate_DealWithoutContactId_PassesJsonSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var deals = canon.Deals.ToList();
        deals[0] = deals[0] with { ContactId = null };

        var modifiedCanon = canon with { Deals = deals };
        var violations = CanonSchemaValidator.Validate(modifiedCanon);

        Assert.Empty(violations);
    }

    [Fact]
    public async Task Validate_CaseWithoutContactId_PassesJsonSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var cases = canon.Cases.ToList();
        cases[0] = cases[0] with { ContactId = null };

        var modifiedCanon = canon with { Cases = cases };
        var violations = CanonSchemaValidator.Validate(modifiedCanon);

        Assert.Empty(violations);
    }

    [Fact]
    public async Task Validate_OrganisationWithEmptyMembers_PassesJsonSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var organisations = canon.Organisations.ToList();
        organisations[0] = organisations[0] with
        {
            MemberPersonaIds = [],
            PrimaryContactId = null
        };

        var modifiedCanon = canon with { Organisations = organisations };
        var violations = CanonSchemaValidator.Validate(modifiedCanon);

        Assert.Empty(violations);
    }
}
