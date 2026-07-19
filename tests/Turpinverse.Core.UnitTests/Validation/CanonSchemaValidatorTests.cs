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
}
