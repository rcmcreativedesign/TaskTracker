using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TaskTracker.Commands;
using TaskTracker.Enums;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    public partial class MainWindow : Window
    {
        private const string SERVICENOWURL = "https://ostprod.servicenowservices.com";

        public MainWindow()
        {
            InitializeComponent();

            TaskIdClickedCommand = new SimpleDelegateCommand(TaskId_Clicked);

            foreach (TaskItem item in DataProcessor.GetAllTaskItems())
            {
                item.TaskCompleted += TaskCompletedHandler;
                TaskItems.Add(item);
            }
        }

        public ICommand TaskIdClickedCommand { get; set; }
        public ObservableCollection<TaskItem> TaskItems { get; set; } = new();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask task = new();
            task.WindowClosed += AddTask_WindowClosed;
            task.Show();
        }

        public void TaskId_Clicked(object taskId)
        {
            if (taskId is string id)
            {
                var taskItem = TaskItems.FirstOrDefault(x => x.TaskId == id);
                if (taskItem != null && taskItem.ServiceNowType != ServiceNowType.None)
                {
                    var type = taskItem.ServiceNowType switch
                    {
                        ServiceNowType.Incident => "sc_incident",
                        ServiceNowType.Request => "sc_request",
                        _ => "sc_task"
                    };
                    var url = $"{SERVICENOWURL}/{type}?sysparm_query=number={taskItem.TaskId}";
                    //Process.Start($"{SERVICENOWURL}/{type}?sys_parm=number={taskItem.TaskId}");
                }
            }
        }

        private void AddTask_WindowClosed(object sender, TaskItem addedItem)
        {
            addedItem.TaskCompleted += TaskCompletedHandler;
            TaskItems.Add(addedItem);
        }

        private static List<TaskItem> GenerateTestData()
        {
            List<TaskItem> testData = new()
            {
                new TaskItem { IsCompleted = false, TaskId = "PRJ001", Description = "First project", Category = Category.TeamLead, DueDate = DateTime.Now.AddDays(2) },
                new TaskItem { IsCompleted = false, TaskId = "TASK10933", Description = "Fix the issue with the program that people use", Category = Category.DotNet },
                new TaskItem { IsCompleted = true, TaskId = "PRJ002", Description = "Second Project", Category = Category.VPMS, DueDate = DateTime.Now.AddDays(-2) }
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
