using System.Collections.Generic;
using TaskTracker.Models;

namespace TaskTracker.Extensions
{
    public static class ListExtensions
    {
        public static List<TaskItem> Flatten(this List<TaskItem> list)
        {
            var results = new List<TaskItem>();
            foreach (var task in list)
            {
                results.Add(task);
                foreach (var subTask in task.SubTasks)
                {
                    results.Add(subTask);
                }
            }
            return results;
        }
    }
}
