using Microsoft.Extensions.DependencyInjection;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;

var services = new ServiceCollection();
services.AddTurpinverseCore();
services.AddTurpinverseData();
var provider = services.BuildServiceProvider();

var repoRoot = FindRepoRoot();
var siteDir = Path.Combine(repoRoot, "site");

var generator = provider.GetRequiredService<IHugoContentGenerator>();
await generator.GenerateAsync(siteDir);

var canon = await provider.GetRequiredService<ICanonRepository>().LoadAsync();
Console.WriteLine($"Generated {canon.Personas.Count} personas, {canon.Organisations.Count} organisations, {canon.Events.Count} events.");

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Turpinverse.slnx")))
        {
            return dir.FullName;
        }
        dir = dir.Parent;
    }
    throw new InvalidOperationException("Could not find repository root.");
}
