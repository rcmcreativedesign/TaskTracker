using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : Window, INotifyPropertyChanged
    {
        private readonly Dictionary<string, int> sortArray = [];

        public EditTask()
        {
            InitializeComponent();
            PopulateCategories();
            PopulateSortArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TaskItem> WindowClosed;

        public TaskItem TaskItem { get; set; } = new();

        public ObservableCollection<string> Categories { get; set; }

        private void PopulateCategories()
        {
            var settings = DataProcessor.GetSettings();
            Categories = new ObservableCollection<string>(settings.Categories);
            RaisePropertyChanged(nameof(Categories));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskItem.TaskId) || string.IsNullOrEmpty(TaskItem.Description))
                return;

            DataProcessor.SaveTaskItem(TaskItem);
            WindowClosed?.Invoke(this, TaskItem);
            Close();
        }

        private void CancelButton_Click(object obj, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnActivated(EventArgs e)
        {
            RefreshList();
            base.OnActivated(e);
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
