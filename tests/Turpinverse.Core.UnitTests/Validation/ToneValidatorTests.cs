using FluentAssertions;
using Turpinverse.Core.Validation;

namespace Turpinverse.Core.UnitTests.Validation;

public class ToneValidatorTests
{
    private readonly ToneValidator _validator = new();

    [Fact]
    public void ValidateText_RejectsForbiddenPattern()
    {
        var violations = _validator.ValidateText(
            "This is an idiot move",
            ["\\bidiot\\b"],
            "Persona",
            "test");

        violations.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidateText_AllowsCleanText()
    {
        var violations = _validator.ValidateText(
            "Seasoned highway operations consultant.",
            ["\\bidiot\\b"],
            "Persona",
            "test");

        violations.Should().BeEmpty();
    }
}
