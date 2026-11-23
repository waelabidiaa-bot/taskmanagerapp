# Personal Productivity & Learning Manager

A small Avalonia desktop app for daily tasks, notes, learning progress and categories.
It uses the existing three-project solution (`taskmanager.slnx`) and SQLite/EF Core.

## Run

```bash
cd TaskManager.UI
dotnet run
```

Run from the `TaskManager.UI` folder: the SQLite connection string is the original
relative `Data Source=tasks.db`, so that is where the application database lives.
Pending EF Core migrations are applied automatically on startup; existing data is preserved.

## Structure

- **TaskManager.Core** — entities, repository interfaces and feature services (validation, search rules).
- **TaskManager.Data** — `ApplicationDbContext`, EF Core migrations and one repository per feature.
- **TaskManager.UI** — one `MainWindow` with a slim VSCode-style icon rail and resizable split panes,
  one `Views/` + `ViewModels/` folder per module, shared styles/templates in `App.axaml`, and dependency
  injection in `App.axaml.cs`.

Views contain no business logic. ViewModels call services; services call repositories;
only the Data layer touches EF Core.

The shell is a single window with a split workspace: `NavigationService` registers each module once
and clicking a rail icon opens it as a panel in that window. Several modules stay visible at once,
separated by draggable `GridSplitter`s; clicking an already-open module focuses its panel instead of
duplicating it, and each panel has a header with a close button. The narrow ~56px activity bar uses
icon-only links with tooltips on hover and a blue accent for every module with an open panel.
Cross-module commands (for example "Open task notes") use the same navigation service, so they open
or focus the target module's panel and still pass their parameter.

The UI uses a casual dark slate theme with a blue accent: a deep canvas, lighter rounded cards,
roomier spacing and larger type. `App.axaml` sets `RequestedThemeVariant="Dark"` and holds the shared
styles (soft corner radii, padded panels, rounded buttons and rounded inputs). Content is centered
to a max width, wrapping `WrapPanel` rows replace fixed multi-column grids, and `TextBlock`
wrapping/spacing lets forms and lists reflow cleanly when a window is resized. Horizontal page
scrollbars are disabled. Icons are lightweight inline vector paths defined once in
`TaskManager.UI/Icons/AppIcons.cs` and reused via `PathIcon` in the rail, page headers, section
titles and primary buttons — no extra icon package is required.

## Modules

- **Dashboard** — task totals, recent notes with their summaries, and active study goals.
- **All Tasks** — read-focused overview with every task, its completion progress, note previews and a
  master–detail panel. Clicking a task shows all its details: description, priority, due date, category,
  linked study goal (including its milestone progress), and the full note text. Search covers task titles,
  descriptions and note titles/summaries; **Edit in Tasks** and **Open notes** jump to the related module
  with that task selected.
- **Tasks** — create, edit, delete, complete, priority, due date, search and filters. Selecting a task
  shows its attached notes; **Open task notes** jumps to Notes with that task already selected.
- **Notes** — title, short summary, long content, pinning, search by title/summary/content, and links
  to a task, category and study goal. Opening Notes from a task filters to that task's notes.
- **Study goals (Learning)** — subject, description and a milestone checklist. Select a saved goal to
  add milestones, then mark them done or undo them; progress is calculated as
  `completed milestones / all milestones` and shown on a percentage bar. No manual progress slider.
  Each goal also shows its linked tasks and study notes. Completing every milestone sets status to
  Completed; undoing one reopens it as In Progress.
- **Categories** — create, rename, delete; shared by tasks and notes (unique, case-insensitive names).
- **Settings** — database file information and stored record counts.

Selecting a list item loads it into the form for editing; Save creates a new item or updates
the selected one. Study goals must be saved before milestones can be added to them. Deleting a task,
category or learning activity clears that link from notes instead of deleting the notes; deleting a
goal also removes its milestones.

## Adding a module

1. Add its model, repository interface and service in Core, plus repository/EF configuration in Data.
2. Add its ViewModel and View under a new `Views/<Feature>` / `ViewModels/<Feature>` folder.
3. Register repository, service and ViewModel in `App.axaml.cs`, add a `DataTemplate` in `App.axaml`,
   and add one `NavigationItem` in `MainWindowViewModel`.

Two small shared hooks live in `TaskManager.UI/Services`: `INavigationAware` reloads a module when
it becomes visible (and receives an optional parameter), and `INavigationService` lets a module open
another one with context — for example Tasks opens Notes with the selected task — without referencing
the other module's ViewModel directly.

## Migrations

The app applies pending migrations at startup, so no manual update step is needed.
To create a new migration:

```bash
dotnet ef migrations add <Name> --project TaskManager.Data --context ApplicationDbContext
```
