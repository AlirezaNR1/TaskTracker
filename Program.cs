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
    case "add":
        HandleArgs(args);
        break;
    case "update":
    case "delete":
    case "mark-in-progress":
    case "mark-done":
    case "list":
        Console.WriteLine($"Command '{command}' not implemented yet.");
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
}

void HandleArgs(string[] args)
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

    var now = DateTime.UtcNow;

    // For now, Id is just a placeholder.
    // Later we’ll generate proper Ids when we load tasks from JSON.
    //TODO: change Id indexing
    var task = new TaskItem
    {
        Id = 1,
        Title = title,
        Description = description,
        Status = "todo",
        CreatedAt = now,
        UpdatedAt = now
    };

    Console.WriteLine("Adding task:");
    Console.WriteLine($"  Id: {task.Id}");
    Console.WriteLine($"  Title: {task.Title}");
    Console.WriteLine($"  Description: {(task.Description ?? "(none)")}");
    Console.WriteLine($"  Status: {task.Status}");
    Console.WriteLine($"  CreatedAt: {task.CreatedAt:o}");
    Console.WriteLine($"  UpdatedAt: {task.UpdatedAt:o}");
}

class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "todo"; // "todo" | "in-progress" | "done"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
