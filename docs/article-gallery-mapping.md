# Article and gallery mapping (repo-only)

This note is **not** Hugo content. It documents how generic article and gallery canon records project to the public reference site without naming a consumer theme as the source of truth.

## Canon → Hugo

| Canon field | Hugo output |
|-------------|-------------|
| `Article.title` | Page title and list card heading |
| `Article.publishedAt` | Front matter `publishedAt` |
| `Article.body` | Markdown body |
| `Article.authorPersonaId` | Front matter + persona article lists; display name resolved at generation |
| `Article.collection` | Collection label chip via `article-collection` partial |
| `Article.showTableOfContents` | Front matter `showToc` when true |
| `Article.relatedProjectId` / `relatedCaseId` | Front matter + named links partial |
| `Gallery.images[]` | `data/galleries.json` consumed by gallery layouts |
| `Gallery.viewer` | `viewerEnabled` / `viewerMode` front matter |

Draft articles (`draft: true`) are omitted from generated markdown and `data/articles.json`.

## Out of scope

- Blazor routes, CSV columns, and `ExportDatasets` entries for articles/galleries
- Hugo collection taxonomy or `/collections/` index pages
