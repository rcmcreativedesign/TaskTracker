using System;
using TaskTracker.Models;

namespace TaskTracker.Interfaces
{
    public interface IAddTask
    {
        event EventHandler<TaskItem> WindowClosed;
        TaskItem TaskItem { get; set; }
        void Show();
    }
}
