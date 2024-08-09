using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string SERVICENOWURL = "https://ostprod.servicenowservices.com";
        private string selectedFilter = "All";
        private readonly Dictionary<string, int> sortArray = [];

        public MainWindow()
        {
            InitializeComponent();

            TaskIdClickedCommand = new SimpleDelegateCommand(TaskId_Clicked);
            EditTaskCommand = new SimpleDelegateCommand(EditTask_Clicked);

            PopulateFilters();
            PopulateSortArray();
            RefreshList("All");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand TaskIdClickedCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }

        public ObservableCollection<TaskItem> TaskItems { get; set; } = [];
        public ObservableCollection<string> Filters { get; set; } = ["All"];

        public string SelectedFilter
        {
            get => selectedFilter;
            set
            {
                if (SetProperty(ref selectedFilter, value))
                    RefreshList(selectedFilter);
            }
        }

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
                    task.TaskItem.CreatedDate = taskItem.CreatedDate;
                    task.TaskItem.CompletedDate = taskItem.CompletedDate;
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
                var inputBox = new InputBox("Enter completion date", task.CompletedDate.ToString());
                inputBox.ShowDialog();
                if (!string.IsNullOrEmpty(inputBox.Text) && DateTime.TryParse(inputBox.Text, out var date))
                    task.CompletedDate = date;
                DataProcessor.SaveTaskItem(task);
                _ = TaskItems.Remove(task);
            }
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

                RefreshList(SelectedFilter, sort, direction);
            }
        }

        private void PopulateFilters()
        {
            var filters = DataProcessor.GetSettings().Categories;
            foreach (var filter in filters)
                Filters.Add(filter);
            RaisePropertyChanged(nameof(Filters));
        }

        private void PopulateSortArray()
        {
            sortArray.Add("Task ID", -1);
            sortArray.Add("Desc", -1);
            sortArray.Add("Due", -1);
        }

        private void RefreshList(string selectedFilter)
        {
            RefreshList(selectedFilter, null, -1);
        }

        private void RefreshList(string filter, Expression<Func<TaskItem, object>> sort, int direction)
        {
            TaskItems.Clear();

            var allTasks = DataProcessor.GetAllTaskItems().AsQueryable();
            if (sort is not null)
            {
                if (direction == 0)
                    allTasks = allTasks.OrderBy(sort);
                else if (direction == 1)
                    allTasks = allTasks.OrderByDescending(sort);
            }

            foreach (TaskItem item in allTasks.Where(t => !t.IsCompleted).Where(t => filter == "All" || (t.Category != null && filter.Contains(t.Category))))
            {
                item.TaskCompleted += TaskCompletedHandler;
                TaskItems.Add(item);
            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
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
