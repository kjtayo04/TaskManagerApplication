using System;

namespace TaskManagerApplication.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Low"; // Low, Medium, High
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "todo"; // todo, inprogress, done
        public DateTime EnteredOn { get; set; } = DateTime.UtcNow;
    }
}
