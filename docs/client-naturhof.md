# Client Case Study — Naturhof Bocholt

> A live, production website (German-language) I host and maintain for a
> local organic-vegetable farm in Bocholt, Germany. This is the first real
> (paying) client running on the lab — the point where the homelab stopped
> being a learning project and started serving a business.

## Overview

| | |
|---|---|
| **Client** | Naturhof Bocholt — organic vegetable-box farm (Germany) |
| **Site language** | German |
| **Role** | Sole developer & operator (hosting, backend, frontend, deployment, maintenance) |
| **Hosting** | Runs on the Realm Architect Lab server as an isolated client stack |
| **Status** | Live in production |

## What the client needed

A small farm running a seasonal vegetable-box subscription. Their old WordPress site
had gone offline and they needed a new, self-maintained website — most importantly a
recipe collection, a blog, and a seasonal vegetable calendar that the (non-technical)
owner could update herself.

## What I built

### Self-service content management
- **Blog** with a German-language admin panel and a WYSIWYG editor (headings, formatting,
  inline image upload). The owner publishes posts herself without touching code.
- **Seasonal vegetable calendar** — an editable grid (vegetables × months) where the owner
  toggles a cell to mark a crop in season. Saves instantly. Mobile view uses a
  horizontally-scrollable table with a sticky first column.
- **Recipe system** — recipes with images, categories, and a clean card-grid layout.

### Backend
- **ASP.NET Core Web API (.NET 8, C#)** exposing public read endpoints and JWT-protected
  admin endpoints for the blog and calendar.
- **MariaDB** for all content (recipes, blog posts, calendar data), with secure image
  upload handling (type/size validation, safe filenames).

### Data migration
- Recovered the client's full recipe collection — including images — from their
  **offline legacy WordPress site via the Internet Archive**, then imported it into the
  new database.

### GDPR / German legal compliance
- **Self-hosted web fonts** — removed all Google Fonts requests so no visitor IP is sent
  to Google.
- **Two-click consent for the embedded Google Map** — the map only loads after the visitor
  actively clicks, so no data leaves the page beforehand.
- **Impressum (§ 5 DDG)** and an updated privacy policy reflecting the above.

## Deployment & operations

- Deployed behind the lab's Nginx reverse proxy with HTTPS via Certbot, on its own subdomain.
- Runs as an isolated stack alongside the lab's other services (separate database scope).
- Managed and updated over the WireGuard-only admin path — same security model as the rest
  of the lab.

## Problems solved in production

Real debugging on a live service, not in a sandbox:

- **`.NET publish` failing in production** because of a Windows-only `web.config` transform
  step — fixed by disabling the IIS transform for the Linux/Kestrel target
  (`IsTransformWebConfigDisabled`).
- **API serving stale code after deploy** — traced it to the build never reaching the
  `publish/` directory rather than a restart problem, then corrected the publish step.
- **Recovering content from an already-offline website** using archived copies.

## What this demonstrates

End-to-end ownership of a real client deliverable: provisioning, backend development,
database design and migration, secure deployment, legal compliance, and ongoing
maintenance — the day-to-day of a small IT service provider.

---

*Client work is shown here with the client's permission. No credentials, keys, or
private data are included in this repository.*
