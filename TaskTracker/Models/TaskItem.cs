using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TaskTracker.Enums;

namespace TaskTracker.Models
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
        [Required]
        public string TaskId { get; set; }
        public ServiceNowType ServiceNowType { get; set; }
        [Required]
        public string Description { get; set; }
        public Category Category { get; set; }
        public DateTime? DueDate { get; set; }

        [JsonIgnore]
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
