# Tech stack

Short reference for Turpinverse technology choices. Channel intent (Hugo vs Blazor) lives in [product-surfaces.md](./product-surfaces.md).

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 / C# |
| Web app | ASP.NET Core Blazor Server |
| Orchestration | .NET Aspire (AppHost + ServiceDefaults) |
| Public site | Hugo Extended (PaperMod theme) |
| Styling (Blazor) | Tailwind CSS 4.x |
| Charts (Blazor) | Chart.js |
| CSV export | CsvHelper |
| Data | File-based JSON canon in `src/Turpinverse.Data/canon/` |
| Tests | xUnit v3, NSubstitute, bUnit |

See [README.md](../README.md) for quickstart commands and testing standards.
