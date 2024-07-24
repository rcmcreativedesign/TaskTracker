using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TaskTracker
{
    public class TaskItem : INotifyPropertyChanged
    {
        private bool isCompleted;

        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (SetProperty(ref isCompleted, value))
                {
                    if (isCompleted)
                        TaskCompleted?.Invoke(this, new EventArgs());
                }
            }
        }
        public string TaskId { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime? DueDate { get; set; }

        public string DueDateDisplay => DueDate?.ToString("MM/dd/yyyy", new CultureInfo("en-US")) ?? string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler TaskCompleted;

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
    }
}
