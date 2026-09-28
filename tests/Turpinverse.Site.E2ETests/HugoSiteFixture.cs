using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Playwright;

namespace Turpinverse.Site.E2ETests;

public sealed class HugoSiteFixture : IAsyncLifetime, IAsyncDisposable
{
    public bool IsReady { get; private set; }

    public string BaseUrl { get; private set; } = string.Empty;

    public IPlaywright Playwright { get; private set; } = null!;

    public IBrowser Browser { get; private set; } = null!;

    private WebApplication? _app;

    public async ValueTask InitializeAsync()
    {
        var publicDirectory = RepoPaths.FindBuiltSitePublicDirectory();
        if (publicDirectory is null)
        {
            return;
        }

        var fileProvider = new PhysicalFileProvider(publicDirectory);
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        _app = builder.Build();
        _app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
        _app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });

        await _app.StartAsync(TestContext.Current.CancellationToken);

        var server = _app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var address = addresses?.FirstOrDefault();
        if (string.IsNullOrEmpty(address))
        {
            return;
        }

        BaseUrl = address.TrimEnd('/') + "/";
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync();
        IsReady = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        Playwright?.Dispose();

        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}

[CollectionDefinition(nameof(HugoSiteCollection))]
public sealed class HugoSiteCollection : ICollectionFixture<HugoSiteFixture>;
