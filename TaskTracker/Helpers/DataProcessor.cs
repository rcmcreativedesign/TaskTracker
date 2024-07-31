using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Helpers
{
    public static class DataProcessor
    {
        private const string filePath = "C:\\Temp\\TaskItems.json";
        public static List<TaskItem> GetAllTaskItems()
        {
            if (!File.Exists(filePath))
                return [];
            var dataFile = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(dataFile))
                return [];
            var taskItems = JsonSerializer.Deserialize<List<TaskItem>>(dataFile);
            return taskItems ?? [];
        }

        public static void SaveTaskItem(TaskItem item)
        {
            var allItems = GetAllTaskItems();
            if (allItems.Any(x => x.TaskId == item.TaskId))
            {
                var taskItem = allItems.First(x => x.TaskId == item.TaskId);
                taskItem.Update(item);
            }
            else
                allItems.Add(item);
            SaveAllTaskItems(allItems);
        }

        public static void SaveAllTaskItems(List<TaskItem> taskItems)
        {
            var dataFile = JsonSerializer.Serialize(taskItems);
            if (!File.Exists(filePath))
                File.CreateText(filePath).Close();
            File.WriteAllText(filePath, dataFile);
        }

    }
}
