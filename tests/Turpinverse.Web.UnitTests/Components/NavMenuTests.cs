using Bunit;
using Turpinverse.Core.Export;
using Turpinverse.Web.Components.Layout;

namespace Turpinverse.Web.UnitTests.Components;

public class NavMenuTests : BunitContext
{
    public NavMenuTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void NavMenu_RendersDirectoryItemsBeforeGroupedSections()
    {
        var cut = Render<NavMenu>();
        var markup = cut.Markup;

        var contactsIndex = markup.IndexOf("Contacts", StringComparison.Ordinal);
        var accountsIndex = markup.IndexOf("Accounts", StringComparison.Ordinal);
        var crmIndex = markup.IndexOf("CRM", StringComparison.Ordinal);
        var financeIndex = markup.IndexOf("Finance", StringComparison.Ordinal);
        var leadsIndex = markup.IndexOf("Leads", StringComparison.Ordinal);
        var productsIndex = markup.IndexOf("Products", StringComparison.Ordinal);

        Assert.True(contactsIndex >= 0);
        Assert.True(accountsIndex > contactsIndex);
        Assert.True(crmIndex > accountsIndex);
        Assert.True(leadsIndex > crmIndex);
        Assert.True(financeIndex > leadsIndex);
        Assert.True(productsIndex > financeIndex);
    }

    [Fact]
    public void NavMenu_GroupedItemsFollowExportDatasetOrder()
    {
        var expectedOrder = ExportDatasets.All
            .Where(dataset => dataset.Group is not null)
            .Select(dataset => dataset.Title)
            .ToArray();

        var cut = Render<NavMenu>();
        var positions = expectedOrder
            .Select(title => cut.Markup.IndexOf(title, StringComparison.Ordinal))
            .ToArray();

        Assert.All(positions, position => Assert.True(position >= 0));

        for (var index = 1; index < positions.Length; index++)
        {
            Assert.True(positions[index] > positions[index - 1]);
        }
    }
}
