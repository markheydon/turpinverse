using System.Text.Json;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Data;

[Trait("Category", "CanonValidation")]
public class EmbeddedCanonResourceTests
{
    private static readonly string[] ExpectedCanonFiles =
    [
        "canon.json",
        "personas.json",
        "organisations.json",
        "events.json",
        "aliases.json",
        "tone-guidelines.json",
        "deals.json",
        "cases.json",
        "experience.json",
        "education.json",
        "projects.json",
        "achievements.json",
        "articles.json",
        "galleries.json",
        "professional-extras.json",
        "products.json",
        "tax-rates.json",
        "leads.json",
        "quotes.json",
        "invoices.json",
        "payments.json",
        "credit-notes.json",
        "sales-orders.json",
        "bills.json",
        "activities.json"
    ];

    [Fact]
    public void DataAssembly_EmbedsCanonJsonFromRepoRoot()
    {
        var resources = typeof(JsonCanonRepository).Assembly.GetManifestResourceNames();

        Assert.DoesNotContain(resources, name => name.Contains("specs", StringComparison.OrdinalIgnoreCase));

        foreach (var fileName in ExpectedCanonFiles)
        {
            Assert.Contains(
                resources,
                name => name.EndsWith($".canon.{fileName}", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void CoreAssembly_EmbedsCanonSchemaFromCanonSchemaFolder()
    {
        var resources = typeof(CanonSchemaValidator).Assembly.GetManifestResourceNames();

        Assert.DoesNotContain(resources, name => name.Contains("specs", StringComparison.OrdinalIgnoreCase));

        var schemaResource = resources.Single(name =>
            name.EndsWith("canon-schema.json", StringComparison.OrdinalIgnoreCase));

        Assert.EndsWith("schemas.canon-schema.json", schemaResource, StringComparison.Ordinal);

        using var stream = typeof(CanonSchemaValidator).Assembly.GetManifestResourceStream(schemaResource);
        Assert.NotNull(stream);

        using var document = JsonDocument.Parse(stream);
        Assert.Equal("https://turpinverse.dev/schemas/canon/v1", document.RootElement.GetProperty("$id").GetString());
    }
}
