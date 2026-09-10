using System.Text.Json;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Hugo;

public class CommittedSiteCreditNotesDataTests
{
    [Fact]
    public async Task CommittedSiteCreditNotesData_ContainsEveryEmbeddedCanonCreditNoteId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new JsonCanonRepository();
        var canon = await repository.LoadAsync(cancellationToken);
        var repoRoot = FindRepoRoot();
        var siteCreditNotesPath = Path.Combine(repoRoot, "site", "data", "credit-notes.json");

        Assert.True(File.Exists(siteCreditNotesPath), $"Missing committed Hugo data file: {siteCreditNotesPath}");

        await using var stream = File.OpenRead(siteCreditNotesPath);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var siteCreditNoteIds = document.RootElement
            .EnumerateArray()
            .Select(creditNote => creditNote.GetProperty("creditNoteId").GetString())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var canonCreditNoteIds = canon.CreditNotes
            .Select(creditNote => creditNote.CreditNoteId)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(canonCreditNoteIds.Length, siteCreditNoteIds.Length);
        Assert.Equal(canonCreditNoteIds, siteCreditNoteIds);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "canon", "credit-notes.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found from test output directory.");
    }
}
