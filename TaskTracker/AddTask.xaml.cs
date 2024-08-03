using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TaskTracker.Commands;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    public partial class AddTask : Window
    {
        public AddTask()
        {
            InitializeComponent();
            PopulateCategories();
            taskId.Focus();
        }

        public event EventHandler<TaskItem> WindowClosed;

        public TaskItem TaskItem { get; set; } = new();

        public ObservableCollection<string> Categories { get; set; }

        private void PopulateCategories()
        {
            var settings = DataProcessor.GetSettings();
            Categories = new ObservableCollection<string>(settings.Categories);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
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
    }
}