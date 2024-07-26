using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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
