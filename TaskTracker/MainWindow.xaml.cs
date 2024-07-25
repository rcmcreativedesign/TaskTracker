using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TaskTracker.Helpers;

namespace TaskTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //foreach (TaskItem item in GenerateTestData().Where(x => !x.IsCompleted))
            //{
            //    item.TaskCompleted += TaskCompletedHandler;
            //    TaskItems.Add(item);
            //}
            foreach (TaskItem item in DataProcessor.GetAllTaskItems())
            {
                item.TaskCompleted += TaskCompletedHandler;
                TaskItems.Add(item);
            }
        }

        public ObservableCollection<TaskItem> TaskItems { get; set; } = new();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask task = new AddTask();
            task.Show();
        }

        private static List<TaskItem> GenerateTestData()
        {
            List<TaskItem> testData = new()
            {
                new TaskItem { IsCompleted = false, TaskId = "PRJ001", Description = "First project", Category = "VPMS", DueDate = DateTime.Now.AddDays(2) },
                new TaskItem { IsCompleted = false, TaskId = "TASK10933", Description = "Fix the issue with the program that people use", Category = "TCN" },
                new TaskItem { IsCompleted = true, TaskId = "PRJ002", Description = "Second Project", Category = "VPMS", DueDate = DateTime.Now.AddDays(-2) }
            };

            return testData;
        }

        private void TaskCompletedHandler(object sender, EventArgs args)
        {
            if (sender is TaskItem task)
            {
                task.TaskCompleted -= TaskCompletedHandler;
                _ = TaskItems.Remove(task);
            }
        }
    }
}
