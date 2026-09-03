using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public sealed partial class ToneValidator
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);
    private static readonly ConcurrentDictionary<string, Regex> RegexCache = new(StringComparer.Ordinal);

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
            violations.AddRange(ValidateAddressFields(org.RegisteredOffice, patterns, "Organisation", org.Id));
        }

        foreach (var persona in canon.Personas)
        {
            if (persona.Address is not null)
            {
                violations.AddRange(ValidateAddressFields(persona.Address, patterns, "Persona", persona.Id));
            }
        }

        return violations;
    }

    private IReadOnlyList<ValidationViolation> ValidateAddressFields(
        Address address,
        IReadOnlyList<string> forbiddenPatterns,
        string entityType,
        string entityId)
    {
        var violations = new List<ValidationViolation>();
        violations.AddRange(ValidateText(address.Address1, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Address2, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Address3, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Town, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Region, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Postcode, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Country, forbiddenPatterns, entityType, entityId));
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
            if (GetRegex(pattern).IsMatch(text))
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

    private static Regex GetRegex(string pattern) =>
        RegexCache.GetOrAdd(pattern, static p => new Regex(
            p,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant,
            RegexTimeout));
}
