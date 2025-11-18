using System.Text.Json;

class TaskRepository
{
    private readonly string _filePath;

    public TaskRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<TaskItem> LoadAll()
    {
        if (!File.Exists(_filePath))
        {
            // No file yet → no tasks
            return new List<TaskItem>();
        }

        try
        {
            string json = File.ReadAllText(_filePath);

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
            Console.WriteLine($"Warning: Failed to read or parse '{_filePath}': {ex.Message}");
            Console.WriteLine("Starting with an empty task list.");
            return new List<TaskItem>();
        }
    }

    public void SaveAll(List<TaskItem> tasks)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(tasks, options);
        File.WriteAllText(_filePath, json);
    }

    public int GetNextId(List<TaskItem> tasks)
    {
        if (tasks == null || tasks.Count == 0)
        {
            return 1;
        }

        return tasks.Max(t => t.Id) + 1;
    }
}

