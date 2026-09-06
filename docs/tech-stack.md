# Tech stack

Short reference for Turpinverse technology choices. The demo datasets live in [`canon/`](../canon/). Channel intent (Hugo vs Blazor) lives in [product-surfaces.md](./product-surfaces.md). Rationale for major choices: [decision-log.md](./decision-log.md).

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 / C# |
| Web app | ASP.NET Core Blazor Server |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) |
| Public site | Hugo Extended (PaperMod theme) |
| Styling (Blazor) | Tailwind CSS 4.x |
| Charts (Blazor) | Chart.js |
| CSV export | CsvHelper |
| Data | File-based JSON canon in [`canon/`](../canon/) |
| Tests | xUnit v3, NSubstitute, bUnit |

See [README.md](../README.md) for quickstart commands and testing standards.
