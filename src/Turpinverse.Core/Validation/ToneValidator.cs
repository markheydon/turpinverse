using System.Text.RegularExpressions;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public sealed partial class ToneValidator
{
    public IReadOnlyList<ValidationViolation> ValidateCanon(Canon canon)
    {
        var violations = new List<ValidationViolation>();
        var patterns = canon.ToneGuidelines.ForbiddenPatterns;

        foreach (var persona in canon.Personas)
        {
            violations.AddRange(ValidateText(persona.Notes, patterns, "Persona", persona.Id));
        }

        foreach (var org in canon.Organisations)
        {
            violations.AddRange(ValidateText(org.Description, patterns, "Organisation", org.Id));
        }

        return violations;
    }

    public IReadOnlyList<ValidationViolation> ValidateText(
        string? text,
        IReadOnlyList<string> forbiddenPatterns,
        string entityType,
        string entityId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var violations = new List<ValidationViolation>();
        foreach (var pattern in forbiddenPatterns)
        {
            if (ForbiddenPatternRegex(pattern).IsMatch(text))
            {
                violations.Add(new ValidationViolation(
                    "TONE-001",
                    $"Text contains forbidden pattern '{pattern}'",
                    entityType,
                    entityId));
            }
        }

        return violations;
    }

    private static Regex ForbiddenPatternRegex(string pattern) =>
        new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
