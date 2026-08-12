# OncoTarget Explorer

A biological target explorer that helps researchers evaluate whether a given protein or gene is a plausible tumour-specific, membrane-bound cancer target — built on a .NET API and a React frontend.

---

## Overview

Scientists researching cancer targets often need to cross-reference several public databases just to answer one question: *is this protein a plausible tumour-specific, membrane-bound target?*

OncoTarget Explorer brings that lookup into one place — search a gene or protein, see its function, location, and disease associations at a glance, and keep a running shortlist of candidates worth a closer look.

## Features

- **Search** by gene symbol or protein name (e.g. *HER2 / ERBB2*, *TROP2 / TACSTD2*, *CEACAM5*), backed live by the [UniProt](https://www.uniprot.org/) REST API.
- **Detail view** for each result: function summary, subcellular location, sequence length, disease/cancer associations, and cross-reference IDs (RefSeq, PDB, DrugBank, ChEMBL, HGNC, and more).
- **Shortlist** — save proteins of interest to a list that persists between sessions, backed by a SQLite database.
- **Always current** — every search and detail lookup hits UniProt live; nothing is cached from a static snapshot.

**Not included by design:** user accounts/authentication, multiple simultaneous data sources, custom-trained ML models, and structure viewers or other advanced visualisations — this is a focused, single-purpose tool rather than a general bioinformatics platform.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET 10), C# |
| External integration | `HttpClient` + typed client, `System.Text.Json` |
| Data access | EF Core + SQLite |
| Frontend | React + TypeScript (Vite) |
| Data fetching (UI) | TanStack Query |
| Backend tests | xUnit |
| Frontend tests | Vitest + React Testing Library |

## Architecture

The React/TypeScript frontend calls the ASP.NET Core Web API, which either proxies and reshapes live data from the UniProt REST API or reads/writes the shortlist via EF Core and SQLite. The API owns all external communication, keeping the frontend thin. Responsibilities are split into three layers: **Controllers** (HTTP endpoints) → **Services** (business logic and external calls) → **Data** (EF Core repository).

```
backend/
  src/OncoTargetExplorer.Api/   ASP.NET Core Web API
    Controllers/                 ProteinsController, ShortlistController
    Services/                    UniProt client + protein mapping
    Data/                        EF Core DbContext, entity, migrations
    Models/                      API DTOs
  tests/OncoTargetExplorer.Api.Tests/   xUnit tests

frontend/
  src/
    api/                         API client, TanStack Query hooks
    components/                  SearchBox, ResultsTable, DetailPanel, ShortlistPanel
```

---

## Getting Started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (npm included)

### Run the backend

```bash
cd backend/src/OncoTargetExplorer.Api
dotnet run
```

The API starts at `http://localhost:5080` and creates its SQLite database (`oncotarget.db`) on first run. Swagger/OpenAPI is available at `/openapi/v1.json` in development.

`dotnet run` must be invoked from the project's own folder (`backend/src/OncoTargetExplorer.Api`) — running it from `backend/` or the repo root won't find a project. To run it from `backend/` instead, use `dotnet run --project src/OncoTargetExplorer.Api`.

### Run the frontend

In a separate terminal:

```bash
cd frontend
npm install
npm run dev
```

The app starts at `http://localhost:5173` and talks to the API at `http://localhost:5080` by default (override with a `VITE_API_BASE_URL` environment variable). Both the backend and frontend need to be running for search, detail, and shortlist features to work.

### Run the tests

```bash
# Backend (xUnit)
cd backend
dotnet test

# Frontend (Vitest + React Testing Library)
cd frontend
npm run test
```

### Troubleshooting (Windows)

- **`npm`/`node` not recognized**, right after installing Node.js: your terminal's PATH was cached before the install. Open a new terminal window, or refresh the current PowerShell session with:
  ```powershell
  $env:PATH = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
  ```
- **"running scripts is disabled on this system"** when running `npm`: PowerShell's default execution policy blocks npm's `.ps1` launcher. Allow local scripts for your user account:
  ```powershell
  Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
  ```
  Or sidestep it for a single command with `npm.cmd install` / `npm.cmd run dev`.

---

## License

See [LICENSE](LICENSE).
