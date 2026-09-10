using System.Text.Json;
using Turpinverse.Core.Hugo;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class HugoCreditNoteGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesOneMarkdownFilePerCanonCreditNote()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var generator = new HugoContentGenerator(repository);
        var siteRoot = Path.Combine(Path.GetTempPath(), $"turpinverse-hugo-credit-notes-{Guid.NewGuid():N}");

        try
        {
            await generator.GenerateAsync(siteRoot, cancellationToken);

            var creditNotesDir = Path.Combine(siteRoot, "content", "credit-notes");
            var creditNotesDataPath = Path.Combine(siteRoot, "data", "credit-notes.json");

            Assert.Equal(canon.CreditNotes.Count + 1, Directory.GetFiles(creditNotesDir, "*.md").Length);
            Assert.True(File.Exists(creditNotesDataPath));

            var creditNotesJson = await File.ReadAllTextAsync(creditNotesDataPath, cancellationToken);
            var creditNotes = JsonSerializer.Deserialize<JsonElement>(creditNotesJson);
            Assert.Equal(canon.CreditNotes.Count, creditNotes.GetArrayLength());

            var firstCreditNote = canon.CreditNotes[0];
            var creditNoteContent = await File.ReadAllTextAsync(
                Path.Combine(creditNotesDir, $"{firstCreditNote.CreditNoteId}.md"),
                cancellationToken);
            Assert.Contains($"title: \"{firstCreditNote.CreditNoteNumber}\"", creditNoteContent);
            Assert.Contains($"creditNoteId: \"{firstCreditNote.CreditNoteId}\"", creditNoteContent);
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
