using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TaskTracker.Extensions;
using TaskTracker.Helpers;
using TaskTracker.Interfaces;
using TaskTracker.Models;

namespace TaskTracker
{
    public partial class AddTask : BindableWindow, IAddTask
    {
        private readonly string parentTaskId;

        public AddTask() : this(string.Empty) { }

        public AddTask(string parentTaskId)
        {
            InitializeComponent();

            PopulateCategories();
            taskId.Focus();
            this.parentTaskId = parentTaskId;
        }

        public event EventHandler<TaskItem> WindowClosed;

        public TaskItem TaskItem { get; set; } = new();

        public ObservableCollection<string> Categories { get; set; }

        private void PopulateCategories()
        {
            var settings = DataProcessor.GetSettings();
            Categories = new ObservableCollection<string>(settings.Categories);
            RaisePropertyChanged(nameof(Categories));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskItem.TaskId) || string.IsNullOrEmpty(TaskItem.Description))
                return;

            var existingRecord = DataProcessor.GetAllTaskItems().Flatten().FirstOrDefault(t => t.TaskId == TaskItem.TaskId);
            if (existingRecord is not null)
            {
                if (existingRecord.IsCompleted)
                {
                    if (MessageBox.Show("Task ID has been completed. Do you want to revive it?", "Revive Task ID", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        existingRecord.IsCompleted = false;
                        existingRecord.CompletedDate = null;
                        SaveAndClose(existingRecord);
                    }
                }
                else
                    MessageBox.Show("Task ID already exists", "Task ID Exists", MessageBoxButton.OK);
                return;
            }

            if (parentTaskId != null) 
                TaskItem.ParentTaskId = parentTaskId;
            SaveAndClose(TaskItem);
        }

        private void CancelButton_Click(object obj, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveAndClose(TaskItem itemToSave)
        {
            if (!string.IsNullOrEmpty(itemToSave.ParentTaskId))
            {
                var parent = DataProcessor.GetAllTaskItems().Where(t => t.TaskId == itemToSave.ParentTaskId).FirstOrDefault();
                if (parent != null)
                {
                    parent.SubTasks.Add(itemToSave);
                    DataProcessor.SaveTaskItem(parent);
                }
                else
                    Close();
            }
            else
                DataProcessor.SaveTaskItem(itemToSave);
            WindowClosed?.Invoke(this, itemToSave);
            Close();
        }
    }
}