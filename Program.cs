
string filePath = Path.Combine(Environment.CurrentDirectory, "tasks.json");
var repository = new TaskRepository(filePath);
var service = new TaskService(repository);

// No arguments → show usage and fail
if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

// First argument = command
string command = args[0].ToLowerInvariant();

switch (command)
{
    case "list":
        HandleList(args);
        break;
    case "add":
        HandleAdd(args);
        break;
    case "mark-in-progress":
        HandleMarkStatus(args, "in-progress");
        break;
    case "mark-done":
        HandleMarkStatus(args, "done");
        break;
    case "update":
        HandleUpdate(args);
        break;
    case "delete":
        HandleDelete(args); 
        break;
    case "search":
        HandleSearch(args); 
        break;
    default:
        Console.WriteLine($"Unknown command '{command}'.");
        PrintUsage();
        return 1;
}


return 0;

void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  tasktracker add <title> [description]");
    Console.WriteLine("  tasktracker update <id> <new title> [new description]");
    Console.WriteLine("  tasktracker delete <id>");
    Console.WriteLine("  tasktracker mark-in-progress <id>");
    Console.WriteLine("  tasktracker mark-done <id>");
    Console.WriteLine("  tasktracker list [all|todo|in-progress|done]");
    Console.WriteLine("  tasktracker search <text>");
}

void HandleAdd(string[] args)
{
    //args[1] = Title
    //args[2] = Description

    if (args.Length < 2)
    {
        global::System.Console.WriteLine("Error: 'add' requires at least a <title>.");
        PrintUsage(); 
        return;
    }

    string title = args[1];
    string? description = null;

    if (args.Length == 3)
    {
        description = args[2];
    }

    try
    {
        var task = service.AddTask(title, description);
        Console.WriteLine($"Created task {task.Id}: {task.Title}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
void HandleList(string[] args)
{
    var tasks = service.GetAllTasks();

    string filter = "all";
    if (args.Length >= 2)
    {
        filter = args[1].ToLowerInvariant();
    }

    IEnumerable<TaskItem>? filtered = filter switch
    {
        "all" => tasks,
        "todo" => tasks.Where(t => t.Status == "todo"),
        "in-progress" => tasks.Where(t => t.Status == "in-progress"),
        "done" => tasks.Where(t => t.Status == "done"),
        _ => null
    };

    if (filtered is null)
    {
        Console.WriteLine($"Unknown filter '{filter}'. Use: all | todo | in-progress | done");
        PrintUsage();
        return;
    }

    var list = filtered.OrderBy(t => t.Id).ToList();

    if (list.Count == 0)
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    foreach (var t in list)
    {
        // Normalize how status is displayed (optional)
        string statusLabel = t.Status switch
        {
            "todo" => "todo",
            "in-progress" => "in-progress",
            "done" => "done",
            _ => t.Status
        };

        Console.WriteLine($"{t.Id} [{statusLabel}] {t.Title}");

        if (!string.IsNullOrWhiteSpace(t.Description))
        {
            Console.WriteLine($" => {t.Description}");
        }
    }
}
void HandleMarkStatus(string[] args, string newStatus)
{
    // args[0] = command name, args[1] should be id
    if (args.Length < 2)
    {
        Console.WriteLine($"Error: '{args[0]}' requires an <id> argument.");
        PrintUsage();
        return;
    }

    if (!int.TryParse(args[1], out int id))
    {
        Console.WriteLine("Error: <id> must be a number.");
        return;
    }

    bool ok = service.TryChangeStatus(id, newStatus);
    if (!ok)
    {
        Console.WriteLine($"Task with id {id} not found.");
        return;
    }

    Console.WriteLine($"Task {id} status updated to '{newStatus}'.");
}

void HandleDelete(string[] args)
{
    // args[0] = "delete", args[1] should be id
    if (args.Length < 2)
    {
        Console.WriteLine($"Error: '{args[0]}' requires an <id> argument.");
        PrintUsage();
        return;
    }

    if (!int.TryParse(args[1], out int id))
    {
        Console.WriteLine("Error: <id> must be a number.");
        return;
    }

    var tasks = repository.LoadAll();

    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
        Console.WriteLine($"Task with id {id} not found.");
        return;
    }

    tasks.Remove(task);
    repository.SaveAll(tasks);

    Console.WriteLine($"Deleted task {id}."); bool ok = service.TryDeleteTask(id);
    if (!ok)
    {
        Console.WriteLine($"Task with id {id} not found.");
        return;
    }

    Console.WriteLine($"Deleted task {id}.");
}

void HandleUpdate(string[] args)
{
    // args[0] = "update", args[1] = id, args[2] = new title, args[3] = optional new description
    if (args.Length < 3)
    {
        Console.WriteLine($"Error: '{args[0]}' requires <id> and <new title>.");
        PrintUsage();
        return;
    }

    if (!int.TryParse(args[1], out int id))
    {
        Console.WriteLine("Error: <id> must be a number.");
        return;
    }

    string newTitle = args[2];
    string? newDescription = args.Length >= 4 ? args[3] : null;

    bool ok = service.TryUpdateTask(id, newTitle, newDescription);
    if (!ok)
    {
        Console.WriteLine($"Task with id {id} not found.");
        return;
    }

    Console.WriteLine($"Updated task {id}.");
}
void HandleSearch(string[] args)
{
 // args[0] = "search", args[1] should be search text
    if (args.Length < 2)
    {
        Console.WriteLine("Error: 'search' requires a <text> argument.");
        PrintUsage();
        return;
    }

    string query = args[1];
    if (string.IsNullOrWhiteSpace(query))
    {
        Console.WriteLine("Error: search text cannot be empty.");
        return;
    }

    var matches = service.SearchTasks(query);

    if (matches.Count == 0)
    {
        Console.WriteLine("No matching tasks found.");
        return;
    }

    foreach (var t in matches)
    {
        Console.WriteLine($"{t.Id} [{t.Status}] {t.Title}");
        if (!string.IsNullOrWhiteSpace(t.Description))
        {
            Console.WriteLine($"    {t.Description}");
        }
    }
}


