# Tech stack

Short reference for Turpinverse technology choices. The demo datasets live in [`canon/`](../canon/). Channel intent (Hugo vs Blazor) lives in [product-surfaces.md](./product-surfaces.md). Rationale for major choices: [adr/](../adr/).

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 / C# |
| Web app | ASP.NET Core Blazor Server |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) |
| Public site | Hugo Extended (PaperMod theme) |
| Styling (Blazor) | BlazorBlueprint v4 |
| Charts (Blazor) | BlazorBlueprint charts (ECharts) |
| CSV export | CsvHelper |
| Data | File-based JSON canon in [`canon/`](../canon/) |
| Tests | xUnit v3, NSubstitute, bUnit |

See [README.md](../README.md) for quickstart commands and testing standards.
