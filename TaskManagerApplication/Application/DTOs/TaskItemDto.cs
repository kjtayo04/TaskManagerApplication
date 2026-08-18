using System;

namespace TaskManagerApplication.Application.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Low";
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "todo";
        public DateTime EnteredOn { get; set; }
    }
}
