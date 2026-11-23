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
- **TaskManager.UI** — `MainWindow` shell with sidebar, one `Views/` + `ViewModels/` folder per module,
  shared styles/templates in `App.axaml`, and dependency injection in `App.axaml.cs`.

Views contain no business logic. ViewModels call services; services call repositories;
only the Data layer touches EF Core.

## Modules

- **Dashboard** — task totals, recent notes with their summaries, and active study goals.
- **Tasks** — create, edit, delete, complete, priority, due date, search and filters. Selecting a task
  shows its attached notes; **Open task notes** jumps to Notes with that task already selected.
- **Notes** — title, short summary, long content, pinning, search by title/summary/content, and links
  to a task, category and study goal. Opening Notes from a task filters to that task's notes.
- **Study goals (Learning)** — subject, description, status and visual progress. Each goal shows its
  linked tasks, completed-task count and study notes, so progress is tied to real work.
- **Categories** — create, rename, delete; shared by tasks and notes (unique, case-insensitive names).
- **Settings** — database file information and stored record counts.

Selecting a list item loads it into the form for editing; Save creates a new item or updates
the selected one. Deleting a task, category or learning activity clears that link from notes
instead of deleting the notes.

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
