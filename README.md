# TaskTracker CLI

TaskTracker is a simple command-line tool to manage tasks.  
You can add tasks, update them, mark them as in progress or done, delete them, and list or search through them.

This project is intentionally small but structured like a real backend:

- **CLI (Program.cs)** acts like a controller.
- **TaskService** contains the business logic.
- **TaskRepository** persists data to a JSON file.
- **TaskItem** is the domain model.

---

## Features

- Add new tasks with optional descriptions
- Update task title and/or description
- Delete tasks by ID
- Mark tasks as:
  - `todo`
  - `in-progress`
  - `done`
- List tasks with filters:
  - all tasks
  - only todo
  - only in-progress
  - only done
- Search tasks by text in title or description
- Persist tasks to a `tasks.json` file in the current directory

---

## Tech Stack

- .NET (C#)
- Console application (top-level statements)
- Built-in JSON support (`System.Text.Json`)
- File-based storage (`tasks.json`)

No external libraries or frameworks are used.

---

## Project Structure

```text
TaskTracker/
  TaskTracker.csproj
  Program.cs          # CLI entry point (argument parsing, calling service, printing)
  TaskItem.cs        # Task model (Id, Title, Description, Status, timestamps)
  TaskRepository.cs  # Data access (load/save tasks.json, compute next Id)
  TaskService.cs     # Business logic (add/update/delete/status/search)
  tasks.json         # JSON storage file (auto-created at runtime)
```

Getting Started
Prerequisites
.NET SDK installed
(You can check with dotnet --version.)

Clone the repository
bash
Copy code
git clone https://github.com/<your-username>/<your-repo-name>.git
cd <your-repo-name>
Replace <your-username> and <your-repo-name> with your actual GitHub info.

Build
bash
Copy code
dotnet build
Run
From the project directory:

bash
Copy code
dotnet run -- <command> [arguments]
Note: -- is used so dotnet run doesn’t try to parse your CLI args.

Usage
General syntax
bash
Copy code
dotnet run -- <command> [arguments]
Commands
1. Add a task
bash
Copy code
dotnet run -- add <title> [description]
Examples:

bash
Copy code
dotnet run -- add "Buy milk"
dotnet run -- add "Buy milk" "2L low-fat"
2. List tasks
bash
Copy code
dotnet run -- list [all|todo|in-progress|done]
Examples:

bash
Copy code
dotnet run -- list
dotnet run -- list all
dotnet run -- list todo
dotnet run -- list in-progress
dotnet run -- list done
If no filter is provided, all is used by default.

3. Mark task status
Mark as in-progress:

bash
Copy code
dotnet run -- mark-in-progress <id>
Mark as done:

bash
Copy code
dotnet run -- mark-done <id>
Examples:

bash
Copy code
dotnet run -- mark-in-progress 1
dotnet run -- mark-done 2
4. Update a task
bash
Copy code
dotnet run -- update <id> <new title> [new description]
Examples:

bash
Copy code
dotnet run -- update 1 "Buy almond milk"
dotnet run -- update 2 "Refactor code" "Clean up TaskTracker project"
If you only pass a new title, the description stays unchanged.

If you pass a new description as well, both are updated.

5. Delete a task
bash
Copy code
dotnet run -- delete <id>
Example:

bash
Copy code
dotnet run -- delete 3
6. Search tasks
bash
Copy code
dotnet run -- search <text>
The search is case-insensitive and checks both title and description.

Examples:

bash
Copy code
dotnet run -- search milk
dotnet run -- search "clean"
Data Storage
Tasks are stored in a file named tasks.json in the current working directory.

The file is created automatically the first time you add a task.

The format is a simple JSON array of objects, e.g.:

json
Copy code
[
  {
    "Id": 1,
    "Title": "Buy milk",
    "Description": "2L low-fat",
    "Status": "todo",
    "CreatedAt": "2025-11-18T12:34:56Z",
    "UpdatedAt": "2025-11-18T12:34:56Z"
  }
]
Design Overview
This project is structured to mimic a simple backend architecture:

Program (CLI)

Parses command-line arguments.

Validates basic input (e.g. required arguments, integer IDs).

Calls methods on TaskService.

Prints user-friendly output to the console.

TaskService

Contains the core business logic.

Exposes methods like:

AddTask

TryUpdateTask

TryChangeStatus

TryDeleteTask

GetAllTasks

SearchTasks

Uses TaskRepository to load/save data.

TaskRepository

Responsible for persistence.

Reads and writes the list of TaskItem instances from/to tasks.json.

Computes the next available task ID.

TaskItem

Represents a single task in the system.

Contains Id, Title, Description, Status, CreatedAt, UpdatedAt.

This separation makes it easier to later:

Unit test the service without touching the file system.

Replace the JSON file with a real database.

Expose the same logic via a web API instead of a CLI.

Possible Future Improvements
Use an enum for task status instead of raw strings.

Add due dates and priority levels.

Add unit tests for TaskService.

Add export/import commands.

Create an ASP.NET Core Web API using the same service/repository.
