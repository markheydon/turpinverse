using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public static class CanonSchemaValidator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Lazy<JsonSchema> Schema = new(LoadSchema);

    public static IReadOnlyList<ValidationViolation> Validate(Canon canon)
    {
        var element = JsonSerializer.SerializeToElement(canon, JsonOptions);

        var result = Schema.Value.Evaluate(element, new EvaluationOptions
        {
            OutputFormat = OutputFormat.List
        });

        return FlattenViolations(result);
    }

    public static bool TryValidateJson(string json, out IReadOnlyList<ValidationViolation> violations)
    {
        violations = [];
        try
        {
            using var document = JsonDocument.Parse(json);
            var result = Schema.Value.Evaluate(document.RootElement, new EvaluationOptions
            {
                OutputFormat = OutputFormat.List
            });

            violations = FlattenViolations(result);
            return result.IsValid;
        }
        catch (JsonException ex)
        {
            violations = [new ValidationViolation("SCHEMA-000", ex.Message, "Canon", "json")];
            return false;
        }
    }

    private static JsonSchema LoadSchema()
    {
        var assembly = typeof(CanonSchemaValidator).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .First(name => name.EndsWith("canon-schema.json", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Embedded canon schema resource not found.");
        using var reader = new StreamReader(stream);
        return JsonSchema.FromText(reader.ReadToEnd());
    }

    private static List<ValidationViolation> FlattenViolations(EvaluationResults result)
    {
        var violations = new List<ValidationViolation>();
        CollectViolations(result, violations);
        return violations;
    }

    private static void CollectViolations(EvaluationResults result, List<ValidationViolation> violations)
    {
        if (result.IsValid)
        {
            return;
        }

        if (result.Errors is not null)
        {
            foreach (var (keyword, message) in result.Errors)
            {
                var path = result.InstanceLocation.ToString();
                var entityId = string.IsNullOrWhiteSpace(path) ? "canon" : path.TrimStart('/');
                violations.Add(new ValidationViolation(
                    $"SCHEMA-{keyword.ToUpperInvariant()}",
                    message,
                    "Canon",
                    entityId));
            }
        }

        if (result.Details is not null)
        {
            foreach (var detail in result.Details)
            {
                CollectViolations(detail, violations);
            }
        }
    }
}
