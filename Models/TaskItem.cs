namespace TaskTrackerCLIApp.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
    }

    public class TaskItemStatus
    {
        public const string ToDo = "To-do";
        public const string InProgress = "In-progress";
        public const string Done = "Done";
    }
}
