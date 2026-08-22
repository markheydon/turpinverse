using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class ArticleGalleryHugoTests
{
    [Fact]
    public async Task GenerateAsync_WritesPublishedArticlesOnly()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-articles-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var articlesDir = Path.Combine(siteRoot, "content", "articles");
            var published = canon.Articles.Where(a => !a.Draft).ToList();

            Assert.Equal(published.Count + 1, Directory.GetFiles(articlesDir, "*.md").Length);

            foreach (var article in published)
            {
                var content = await File.ReadAllTextAsync(
                    Path.Combine(articlesDir, $"{article.Id}.md"),
                    cancellationToken);
                Assert.Contains($"title: \"{article.Title}\"", content);
                Assert.Contains($"collection: \"{article.Collection}\"", content);
                Assert.Contains($"authorPersonaId: \"{article.AuthorPersonaId}\"", content);
            }
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task GenerateAsync_WritesArticlesDataJsonWithPublishedOnly()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-articles-data-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var articlesJson = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "data", "articles.json"),
                cancellationToken);
            var articles = JsonSerializer.Deserialize<JsonElement>(articlesJson);
            Assert.Equal(canon.Articles.Count(a => !a.Draft), articles.GetArrayLength());
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task GenerateAsync_MapsShowTableOfContentsToShowToc()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var articleWithToc = canon.Articles.First(a => a.ShowTableOfContents == true);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-toc-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var content = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "content", "articles", $"{articleWithToc.Id}.md"),
                cancellationToken);
            Assert.Contains("showToc: true", content);
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task GenerateAsync_WritesGalleryContentAndDataJson()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-galleries-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var galleriesDir = Path.Combine(siteRoot, "content", "galleries");
            Assert.Equal(canon.Galleries.Count + 1, Directory.GetFiles(galleriesDir, "*.md").Length);

            var galleriesJson = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "data", "galleries.json"),
                cancellationToken);
            var galleries = JsonSerializer.Deserialize<JsonElement>(galleriesJson);
            Assert.Equal(canon.Galleries.Count, galleries.GetArrayLength());

            var gallery = canon.Galleries[0];
            var content = await File.ReadAllTextAsync(
                Path.Combine(galleriesDir, $"{gallery.Id}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{gallery.Title}\"", content);
            Assert.Contains("viewerEnabled: true", content);
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task GenerateAsync_PersonaArticleDataSupportsPublishedLists()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-persona-articles-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var articlesJson = await File.ReadAllTextAsync(
                Path.Combine(siteRoot, "data", "articles.json"),
                cancellationToken);
            var articles = JsonSerializer.Deserialize<JsonElement>(articlesJson);

            var authorWithArticles = canon.Articles
                .Where(a => !a.Draft)
                .GroupBy(a => a.AuthorPersonaId)
                .First(g => g.Count() > 0);

            var authored = articles.EnumerateArray()
                .Count(element => element.GetProperty("authorPersonaId").GetString() == authorWithArticles.Key);
            Assert.Equal(authorWithArticles.Count(), authored);

            var personaWithoutArticles = canon.Personas
                .Select(p => p.Id)
                .Except(canon.Articles.Where(a => !a.Draft).Select(a => a.AuthorPersonaId))
                .First();
            var noneAuthored = articles.EnumerateArray()
                .Count(element => element.GetProperty("authorPersonaId").GetString() == personaWithoutArticles);
            Assert.Equal(0, noneAuthored);
        }
        finally
        {
            if (Directory.Exists(siteRoot))
            {
                Directory.Delete(siteRoot, recursive: true);
            }
        }
    }
}
