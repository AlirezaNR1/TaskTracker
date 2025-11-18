# TaskTracker CLI

TaskTracker is a simple command-line application for managing tasks. It lets you add, update, delete, search, and change the status of tasks, storing everything in a local JSON file.

## Features

- Add new tasks with optional description
- Update task title and description
- Delete tasks by ID
- Mark tasks as:
  - `todo`
  - `in-progress`
  - `done`
- List tasks with filters:
  - all, todo, in-progress, done
- Search tasks by text in title or description
- Data stored in `tasks.json` in the current directory

## Tech Stack

- C# / .NET
- Console application (CLI)
- JSON file persistence (no external database)
- Simple layered architecture:
  - CLI (Program)
  - Service layer (`TaskService`)
  - Repository layer (`TaskRepository`)
  - Model (`TaskItem`)

## Commands

```bash
dotnet run -- add <title> [description]
dotnet run -- update <id> <new title> [new description]
dotnet run -- delete <id>
dotnet run -- mark-in-progress <id>
dotnet run -- mark-done <id>
dotnet run -- list [all|todo|in-progress|done]
dotnet run -- search <text>
```

## Usage

From the project directory:

```bash
# Build
dotnet build

# Run (examples)
dotnet run -- add "Buy milk" "2L low-fat"
dotnet run -- list
dotnet run -- list todo
dotnet run -- mark-in-progress 1
dotnet run -- mark-done 1
dotnet run -- update 1 "Buy almond milk" "1L"
dotnet run -- delete 1
dotnet run -- search milk
```

## Notes

- tasks.json is automatically created in the current directory if it doesn’t exist.
- Errors and edge cases (missing/invalid IDs, empty titles, unknown filters) are handled with clear CLI messages.
