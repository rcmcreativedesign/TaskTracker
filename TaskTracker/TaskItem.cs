using System;
using System.Globalization;

namespace TaskTracker
{
    public class TaskItem
    {
        public bool IsCompleted { get; set; }
        public string TaskId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }

        public string DueDateDisplay => DueDate?.ToString("MM/dd/yyyy", new CultureInfo("en-US")) ?? string.Empty;
    }
}
