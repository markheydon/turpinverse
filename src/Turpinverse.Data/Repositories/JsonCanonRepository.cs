using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Models;

namespace Turpinverse.Data.Repositories;

public sealed class JsonCanonRepository : ICanonRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly Assembly _assembly = typeof(JsonCanonRepository).Assembly;

    public async Task<Canon> LoadAsync(CancellationToken cancellationToken = default)
    {
        var version = await ReadStringPropertyAsync("canon.json", "version", cancellationToken);
        var personas = await ReadArrayAsync<Persona>("personas.json", cancellationToken);
        var organisations = await ReadArrayAsync<Organisation>("organisations.json", cancellationToken);
        var events = await ReadArrayAsync<CanonEvent>("events.json", cancellationToken);
        var aliases = await ReadArrayAsync<AliasMap>("aliases.json", cancellationToken);
        var toneGuidelines = await ReadObjectAsync<ToneGuidelines>("tone-guidelines.json", cancellationToken)
            ?? throw new InvalidOperationException("tone-guidelines.json is required.");
        var deals = await ReadArrayAsync<Deal>("deals.json", cancellationToken);
        var cases = await ReadArrayAsync<Case>("cases.json", cancellationToken);
        var experience = await ReadArrayAsync<Experience>("experience.json", cancellationToken);
        var education = await ReadArrayAsync<Education>("education.json", cancellationToken);
        var projects = await ReadArrayAsync<Project>("projects.json", cancellationToken);
        var achievements = await ReadArrayAsync<Achievement>("achievements.json", cancellationToken);

        return new Canon
        {
            Version = version ?? "1.0.0",
            Personas = personas,
            Organisations = organisations,
            Events = events,
            Aliases = aliases,
            ToneGuidelines = toneGuidelines,
            Deals = deals,
            Cases = cases,
            Experience = experience,
            Education = education,
            Projects = projects,
            Achievements = achievements
        };
    }

    private async Task<string?> ReadStringPropertyAsync(
        string fileName,
        string propertyName,
        CancellationToken cancellationToken)
    {
        await using var stream = OpenResource(fileName);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        return document.RootElement.TryGetProperty(propertyName, out var property)
            ? property.GetString()
            : null;
    }

    private async Task<IReadOnlyList<T>> ReadArrayAsync<T>(string fileName, CancellationToken cancellationToken)
    {
        await using var stream = OpenResource(fileName);
        var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions, cancellationToken);
        return items ?? [];
    }

    private async Task<T?> ReadObjectAsync<T>(string fileName, CancellationToken cancellationToken)
    {
        await using var stream = OpenResource(fileName);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
    }

    private Stream OpenResource(string fileName)
    {
        var resourceName = _assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith($".canon.{fileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            throw new FileNotFoundException($"Embedded canon resource not found: {fileName}");
        }

        return _assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Unable to open embedded canon resource: {fileName}");
    }
}
