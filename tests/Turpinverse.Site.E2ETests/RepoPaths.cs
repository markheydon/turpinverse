namespace Turpinverse.Site.E2ETests;

internal static class RepoPaths
{
    public static string? FindRepoRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "invoices.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    public static string? FindBuiltSitePublicDirectory()
    {
        var repoRoot = FindRepoRoot();
        if (repoRoot is null)
        {
            return null;
        }

        var publicDirectory = Path.Combine(repoRoot, "site", "public");
        var indexPath = Path.Combine(publicDirectory, "index.html");
        return File.Exists(indexPath) ? publicDirectory : null;
    }
}
