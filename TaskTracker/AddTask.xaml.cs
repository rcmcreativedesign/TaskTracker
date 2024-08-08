using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    public partial class AddTask : Window, INotifyPropertyChanged
    {
        public AddTask()
        {
            InitializeComponent();
            PopulateCategories();
            taskId.Focus();
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskItem.TaskId) || string.IsNullOrEmpty(TaskItem.Description))
                return;

            var existingRecord = DataProcessor.GetAllTaskItems().FirstOrDefault(t => t.TaskId == TaskItem.TaskId);
            if (existingRecord is not null)
            {
                if (existingRecord.IsCompleted)
                {
                    if (MessageBox.Show("Task ID has been completed. Do you want to revive it?", "Revive Task ID", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        existingRecord.IsCompleted = false;
                        SaveAndClose(existingRecord);
                    }
                }
                else
                    MessageBox.Show("Task ID already exists", "Task ID Exists", MessageBoxButton.OK);
                return;
            }

            SaveAndClose(TaskItem);
        }

        private void CancelButton_Click(object obj, RoutedEventArgs e)
        {
            Close();
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

        private void SaveAndClose(TaskItem itemToSave)
        {
            DataProcessor.SaveTaskItem(itemToSave);
            WindowClosed?.Invoke(this, itemToSave);
            Close();
        }
    }
}