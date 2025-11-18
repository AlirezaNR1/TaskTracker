using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

string filePath = Path.Combine(Environment.CurrentDirectory, "tasks.json");

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
        HandleAdd(args);
        break;
    case "update":
    case "delete":
    case "mark-in-progress":
        HandleMarkStatus(args, "in-progress");
        break;
    case "mark-done":
        HandleMarkStatus(args, "done");
        break;  
    case "list":
        HandleList(args);
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

    var tasks = LoadTasks();

    int nextId;
    if (tasks.Count == 0)
    {
        nextId = 1;
    }
    else
    {
        nextId = tasks.Max(t => t.Id) + 1;
    }

    var now = DateTime.Now;

    var task = new TaskItem
    {
        Id = nextId,
        Title = title,
        Description = description,
        Status = "todo",
        CreatedAt = now,
        UpdatedAt = now
    };

    tasks.Add(task);
    SaveTasks(tasks);

    Console.WriteLine($"Created task {task.Id}: {task.Title}");
}
void HandleList(string[] args)
{
    var tasks = LoadTasks();

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

    var tasks = LoadTasks();

    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
        Console.WriteLine($"Task with id {id} not found.");
        return;
    }

    task.Status = newStatus;
    task.UpdatedAt = DateTime.Now;

    SaveTasks(tasks);

    Console.WriteLine($"Task {task.Id} status updated to '{task.Status}'.");
}

List<TaskItem> LoadTasks()
{
    if (!File.Exists(filePath))
    {
        // No file yet → no tasks
        return new List<TaskItem>();
    }

    try
    {
        string json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            // Empty file → treat as no tasks
            return new List<TaskItem>();
        }

        var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
        return tasks ?? new List<TaskItem>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Failed to read or parse '{filePath}': {ex.Message}");
        Console.WriteLine("Starting with an empty task list.");
        return new List<TaskItem>();
    }
}

void SaveTasks(List<TaskItem> tasks)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    string json = JsonSerializer.Serialize(tasks, options);
    File.WriteAllText(filePath, json);
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
