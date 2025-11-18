

class TaskService
{
    private readonly TaskRepository _repository;

    public TaskService(TaskRepository repository)
    {
        _repository = repository;
    }

    public List<TaskItem> GetAllTasks()
    {
        return _repository.LoadAll();
    }

    public TaskItem AddTask(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        var tasks = _repository.LoadAll();
        int nextId = _repository.GetNextId(tasks);
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
        _repository.SaveAll(tasks);

        return task;
    }

    public bool TryUpdateTask(int id, string newTitle, string? newDescription)
    {
        var tasks = _repository.LoadAll();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            task.Title = newTitle;
        }

        if (newDescription != null)
        {
            task.Description = newDescription;
        }

        task.UpdatedAt = DateTime.UtcNow;
        _repository.SaveAll(tasks);
        return true;
    }

    public bool TryChangeStatus(int id, string newStatus)
    {
        var tasks = _repository.LoadAll();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
        {
            return false;
        }

        task.Status = newStatus;
        task.UpdatedAt = DateTime.Now;
        _repository.SaveAll(tasks);
        return true;
    }

    public bool TryDeleteTask(int id)
    {
        var tasks = _repository.LoadAll();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
        {
            return false;
        }

        tasks.Remove(task);
        _repository.SaveAll(tasks);
        return true;
    }

    public List<TaskItem> SearchTasks(string query)
    {
        var tasks = _repository.LoadAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<TaskItem>();
        }

        string q = query.Trim();

        return tasks
            .Where(t =>
                (t.Title?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.Description?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
            )
            .OrderBy(t => t.Id)
            .ToList();
    }
}