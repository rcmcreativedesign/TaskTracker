using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            EditTaskCommand = new SimpleDelegateCommand(EditTask_Clicked);

            foreach (TaskItem item in DataProcessor.GetAllTaskItems())
            {
                item.TaskCompleted += TaskCompletedHandler;
                TaskItems.Add(item);
            }
        }

        public ICommand TaskIdClickedCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }

        public ObservableCollection<TaskItem> TaskItems { get; set; } = [];
        public ObservableCollection<string> Filters { get; set; } = ["All"];

        public string SelectedFilter { get; set; } = "All";

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask task = new();
            task.WindowClosed += AddTask_WindowClosed;
            task.Show();
        }

        private void TaskId_Clicked(object taskId)
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
                        ServiceNowType.C5Task => "x_ofost_c5_task_table",
                        ServiceNowType.Change => "change_request",
                        ServiceNowType.ChangeTask => "u_it_change_task",
                        _ => "sc_task"
                    };
                    var url = $"{SERVICENOWURL}/{type}.do?sysparm_query=number={taskItem.TaskId}";
                    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                }
            }
        }

        private void EditTask_Clicked(object taskId)
        {
            if (taskId is string id)
            {
                var taskItem = TaskItems.FirstOrDefault(x => x.TaskId == id);
                if (taskItem != null)
                {
                    EditTask task = new();
                    task.WindowClosed += EditTask_WindowClosed;
                    task.Show();
                    task.TaskItem.TaskId = taskItem.TaskId;
                    task.TaskItem.Description = taskItem.Description;
                    task.TaskItem.Category = taskItem.Category;
                    task.TaskItem.DueDate = taskItem.DueDate;
                    task.TaskItem.LastChecked = taskItem.LastChecked;
                    task.TaskItem.ServiceNowType = taskItem.ServiceNowType;
                    task.TaskItem.Requestor = taskItem.Requestor;
                    task.TaskItem.AssignedTo = taskItem.AssignedTo;
                }
            }
        }

        private void AddTask_WindowClosed(object sender, TaskItem addedItem)
        {
            addedItem.TaskCompleted += TaskCompletedHandler;
            TaskItems.Add(addedItem);
        }

        private void EditTask_WindowClosed(object sender, TaskItem updatedItem)
        {
            var taskItem = TaskItems.First(x => x.TaskId == updatedItem.TaskId);
            taskItem.Update(updatedItem);
            listBox.Items.Refresh();
        }

        private void TaskCompletedHandler(object sender, EventArgs args)
        {
            if (sender is TaskItem task)
            {
                task.TaskCompleted -= TaskCompletedHandler;
                _ = TaskItems.Remove(task);
            }
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var test = e.AddedItems;
        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            var test = e.RoutedEvent;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            TrackerSettings view = new();
            view.WindowClosed += Settings_WindowClosed;
            view.Show();
        }

        private void Settings_WindowClosed(object sender, object e)
        {
            listBox.Items.Refresh();
        }

        //private static List<TaskItem> GenerateTestData()
        //{
        //    List<TaskItem> testData =
        //    [
        //        new TaskItem { IsCompleted = false, TaskId = "PRJ001", Description = "First project", Category = Category.TeamLead, DueDate = DateTime.Now.AddDays(2) },
        //        new TaskItem { IsCompleted = false, TaskId = "TASK10933", Description = "Fix the issue with the program that people use", Category = Category.DotNet },
        //        new TaskItem { IsCompleted = true, TaskId = "PRJ002", Description = "Second Project", Category = Category.VPMS, DueDate = DateTime.Now.AddDays(-2) }
        //    ];

        //    return testData;
        //}
    }
}
