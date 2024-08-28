using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTracker.Commands;
using TaskTracker.Enums;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : BindableWindow, INotifyPropertyChanged
    {
        private const string SERVICENOWURL = "https://ostprod.servicenowservices.com";
        private readonly Dictionary<string, int> sortArray = [];
        private TaskItem taskItem = new();

        public EditTask(TaskItem taskItem)
        {
            InitializeComponent();

            TaskIdClickedCommand = new SimpleDelegateCommand(TaskId_Clicked);
            EditTaskCommand = new SimpleDelegateCommand(EditTask_Clicked);

            PopulateCategories();
            PopulateSortArray();
            TaskItem = taskItem;
            RaisePropertyChanged(nameof(IsParent));
            RaisePropertyChanged(nameof(WindowWidth));
            RefreshList();
        }

        public event EventHandler<TaskItem> WindowClosed;

        public ICommand TaskIdClickedCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }

        public TaskItem TaskItem { get => taskItem; set => SetProperty(ref taskItem, value); }
        public ObservableCollection<string> Categories { get; set; }
        public bool SubTasksModified { get; set; } = false;
        public Visibility IsParent => string.IsNullOrEmpty(TaskItem.ParentTaskId) ? Visibility.Visible : Visibility.Collapsed;
        public int WindowWidth => string.IsNullOrEmpty(TaskItem.ParentTaskId) ? 730 : 330;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskItem.TaskId) || string.IsNullOrEmpty(TaskItem.Description))
                return;

            DataProcessor.SaveTaskItem(TaskItem);
            WindowClosed?.Invoke(this, TaskItem);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void AddSubTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask addTask = new(TaskItem.TaskId);
            addTask.WindowClosed += AddSubTask_WindowClosed;
            addTask.Show();
        }

        private void AddSubTask_WindowClosed(object sender, TaskItem e)
        {
            SubTasksModified = true;
        }

        private void EditTask_Clicked(object taskId)
        {
            if (taskId is string id)
            {
                var taskItem = TaskItem.SubTasks.FirstOrDefault(x => x.TaskId == id);
                if (taskItem != null)
                {
                    EditTask task = new(taskItem);
                    task.WindowClosed += EditTask_WindowClosed;
                    task.Show();
                }
            }
        }

        private void EditTask_WindowClosed(object sender, TaskItem updatedItem)
        {
            var taskItem = TaskItem.SubTasks.First(x => x.TaskId == updatedItem.TaskId);
            taskItem.Update(updatedItem);
            RaisePropertyChanged(nameof(TaskItem.SubTasks));
        }

        private void TaskId_Clicked(object taskId)
        {
            if (taskId is string id)
            {
                var taskItem = TaskItem.SubTasks.FirstOrDefault(x => x.TaskId == id);
                if (taskItem != null && taskItem.ServiceNowType != ServiceNowType.None)
                {
                    var type = taskItem.ServiceNowType switch
                    {
                        ServiceNowType.Incident => "incident",
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

        private void TaskCompletedHandler(object sender, EventArgs args)
        {
            if (sender is TaskItem task)
            {
                task.TaskCompleted -= TaskCompletedHandler;
                var inputBox = new InputBox("Enter completion date", task.CompletedDate.ToString());
                inputBox.ShowDialog();
                if (!string.IsNullOrEmpty(inputBox.Text) && DateTime.TryParse(inputBox.Text, out var date))
                    task.CompletedDate = date;
                DataProcessor.SaveTaskItem(task);
                _ = TaskItem.SubTasks.Remove(task);
            }
        }

        private void Header_Click(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                var headerContent = header.Content.ToString();
                int direction = ++sortArray[headerContent];
                if (direction > 1)
                    direction = -1;
                sortArray[headerContent] = direction;
                Expression<Func<TaskItem, object>> sort = headerContent switch
                {
                    "Task ID" => (x) => x.TaskId,
                    "Desc" => (x) => x.Description,
                    "Due" => (x) => x.DueDate,
                    _ => (x) => null,
                };

                RefreshList(sort, direction);
            }
        }

        private void PopulateCategories()
        {
            var settings = DataProcessor.GetSettings();
            Categories = new ObservableCollection<string>(settings.Categories);
            RaisePropertyChanged(nameof(Categories));
        }

        private void PopulateSortArray()
        {
            sortArray.Add("Task ID", -1);
            sortArray.Add("Desc", -1);
            sortArray.Add("Due", -1);
        }

        private void RefreshList()
        {
            RefreshList(null, -1);
        }

        private void RefreshList(Expression<Func<TaskItem, object>> sort, int direction)
        {
            TaskItem.SubTasks.Clear();

            var allTasks = DataProcessor.GetAllTaskItems(TaskItem.TaskId).AsQueryable();
            if (sort is not null)
            {
                if (direction == 0)
                    allTasks = allTasks.OrderBy(sort);
                else if (direction == 1)
                    allTasks = allTasks.OrderByDescending(sort);
            }

            foreach (TaskItem item in allTasks.Where(t => !t.IsCompleted))
            {
                item.TaskCompleted += TaskCompletedHandler;
                TaskItem.SubTasks.Add(item);
            }
        }
    }
}
