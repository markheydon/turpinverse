using System.Text.Json;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public static class CanonSchemaValidator
{
    public static IReadOnlyList<ValidationViolation> Validate(Canon canon)
    {
        var violations = new List<ValidationViolation>();

        if (canon.Personas.Count < 15)
        {
            violations.Add(new ValidationViolation(
                "SCHEMA-001", "Personas array requires minimum 15 items", "Canon", "personas"));
        }

        if (canon.Organisations.Count < 8)
        {
            violations.Add(new ValidationViolation(
                "SCHEMA-002", "Organisations array requires minimum 8 items", "Canon", "organisations"));
        }

        if (canon.Events.Count < 10)
        {
            violations.Add(new ValidationViolation(
                "SCHEMA-003", "Events array requires minimum 10 items", "Canon", "events"));
        }

        if (canon.ToneGuidelines.Principles.Count < 3)
        {
            violations.Add(new ValidationViolation(
                "SCHEMA-004", "Tone guidelines require minimum 3 principles", "Canon", "toneGuidelines"));
        }

        foreach (var persona in canon.Personas)
        {
            if (!persona.Email.EndsWith("@turpinverse.uk", StringComparison.Ordinal))
            {
                violations.Add(new ValidationViolation(
                    "SCHEMA-005",
                    $"Persona '{persona.Id}' email does not match required pattern",
                    "Persona",
                    persona.Id));
            }
        }

        return violations;
    }

    public static bool TryValidateJson(string json, out IReadOnlyList<ValidationViolation> violations)
    {
        violations = [];
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException ex)
        {
            violations = [new ValidationViolation("SCHEMA-000", ex.Message, "Canon", "json")];
            return false;
        }
    }
}
