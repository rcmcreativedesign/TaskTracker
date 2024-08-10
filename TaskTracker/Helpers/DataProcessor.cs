using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Helpers
{
    public static class DataProcessor
    {
        private const string filePathBase = "C:\\Temp";
        private const string itemsFile = "TaskItems.json";
        private const string settingsFile = "Settings.json";

        public static List<TaskItem> GetAllTaskItems()
        {
            string itemsFilePath = Path.Combine(filePathBase, itemsFile);
            if (!File.Exists(itemsFilePath))
                return [];
            var dataFile = File.ReadAllText(itemsFilePath);
            if (string.IsNullOrEmpty(dataFile))
                return [];
            var taskItems = JsonSerializer.Deserialize<List<TaskItem>>(dataFile);
            return taskItems ?? [];
        }

        public static List<TaskItem> GetAllTaskItems(string taskId)
        {
            var taskItem = GetAllTaskItems().FirstOrDefault(x => x.TaskId == taskId);
            return [.. taskItem.SubTasks];
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
            string itemsFilePath = Path.Combine(filePathBase, itemsFile);
            var dataFile = JsonSerializer.Serialize(taskItems);
            if (!File.Exists(itemsFilePath))
                File.CreateText(itemsFilePath).Close();
            File.WriteAllText(itemsFilePath, dataFile);
        }

        public static Settings GetSettings()
        {
            string setingsFilePath = Path.Combine(filePathBase, settingsFile);
            if (!File.Exists(setingsFilePath))
                return new();
            var dataFile = File.ReadAllText(setingsFilePath);
            if (string.IsNullOrEmpty(dataFile))
                return new();
            var settings = JsonSerializer.Deserialize<Settings>(dataFile);
            return settings ?? new();
        }

        public static void SaveSettings(Settings settings)
        {
            string setingsFilePath = Path.Combine(filePathBase, settingsFile);
            var dataFile = JsonSerializer.Serialize(settings);
            if (!File.Exists(setingsFilePath))
                File.CreateText(setingsFilePath).Close();
            File.WriteAllText(setingsFilePath, dataFile);
        }
    }
}
