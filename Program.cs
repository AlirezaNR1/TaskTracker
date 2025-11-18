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

    var now = DateTime.UtcNow;

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
