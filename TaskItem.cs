
internal class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "todo"; // "todo" | "in-progress" | "done"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

