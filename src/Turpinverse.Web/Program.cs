using Microsoft.Extensions.Options;
using Turpinverse.Core.DependencyInjection;
using Turpinverse.Data.DependencyInjection;
using Turpinverse.Web.Components;
using Turpinverse.Web.Configuration;
using Turpinverse.Web.DependencyInjection;
using Turpinverse.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddTurpinverseCore();
builder.Services.AddTurpinverseData();
builder.Services.AddTurpinverseWeb(builder.Configuration);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var exportOptions = app.Services.GetRequiredService<IOptions<ExportApiOptions>>().Value;
if (exportOptions.PublicApiEnabled)
{
    app.MapExportEndpoints();
    app.MapCanonEndpoints();
}

app.Run();

public partial class Program;
