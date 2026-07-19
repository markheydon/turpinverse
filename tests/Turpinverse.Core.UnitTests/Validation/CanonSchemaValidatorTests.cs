using FluentAssertions;
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
        var canon = await _repository.LoadAsync();
        var violations = CanonSchemaValidator.Validate(canon);

        violations.Should().BeEmpty(
            because: string.Join("; ", violations.Select(v => v.Message)));
    }

    [Fact]
    public void TryValidateJson_RejectsInvalidJson()
    {
        var valid = CanonSchemaValidator.TryValidateJson("{not-json", out var violations);

        valid.Should().BeFalse();
        violations.Should().NotBeEmpty();
    }
}
