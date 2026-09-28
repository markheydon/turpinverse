using Microsoft.Playwright;

namespace Turpinverse.Site.E2ETests;

[Collection(nameof(HugoSiteCollection))]
[Trait("Category", "HugoSite")]
public sealed class PrimaryNavDropdownTests(HugoSiteFixture fixture)
{
    private const string SkipMessage =
        "Hugo site not built; run ./scripts/invoke-hugo-site.sh build (or hugo build in site/) first.";

    [Fact]
    public async Task CrmDropdown_OnTabletPortrait_UsesAbsolutePanelAndSingleNavRow()
    {
        if (!fixture.IsReady)
        {
            Assert.Skip(SkipMessage);
        }

        await using var context = await fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = fixture.BaseUrl,
            ViewportSize = new ViewportSize { Width = 768, Height = 1024 },
        });
        var page = await context.NewPageAsync();

        await page.GotoAsync("/");
        await page.Locator("summary.menu-dropdown-trigger", new PageLocatorOptions { HasText = "CRM" })
            .ClickAsync();

        var panel = page.Locator(".menu-dropdown-details[open] .menu-dropdown-panel");
        await panel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var position = await panel.EvaluateAsync<string>("el => getComputedStyle(el).position");
        Assert.Equal("absolute", position);

        var offsetTops = await page.Locator("#menu > li")
            .EvaluateAllAsync<int[]>("elements => elements.map(element => element.offsetTop)");
        Assert.NotEmpty(offsetTops);
        var minTop = offsetTops.Min();
        var maxTop = offsetTops.Max();
        Assert.True(
            maxTop - minTop <= 3,
            $"Expected nav items on one row; offsetTop spread was {maxTop - minTop}px (issue #54 showed a large split).");
    }

    [Fact]
    public async Task CrmDropdown_OnPhone_UsesStaticPanel()
    {
        if (!fixture.IsReady)
        {
            Assert.Skip(SkipMessage);
        }

        await using var context = await fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = fixture.BaseUrl,
            ViewportSize = new ViewportSize { Width = 375, Height = 667 },
        });
        var page = await context.NewPageAsync();

        await page.GotoAsync("/");
        await page.Locator("summary.menu-dropdown-trigger", new PageLocatorOptions { HasText = "CRM" })
            .ClickAsync();

        var panel = page.Locator(".menu-dropdown-details[open] .menu-dropdown-panel");
        await panel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var position = await panel.EvaluateAsync<string>("el => getComputedStyle(el).position");
        Assert.Equal("static", position);
    }
}
