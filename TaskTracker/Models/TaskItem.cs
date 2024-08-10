using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using TaskTracker.Enums;

namespace TaskTracker.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        private bool isCompleted;
        private string taskId;
        private string description;
        private ServiceNowType serviceNowType;
        private string requestor;
        private string assignedTo;
        private string category;
        private DateTime? dueDate;
        private DateTime? lastChecked;
        private DateTime createdDate = DateTime.Now;

        public DateTime CreatedDate { get => createdDate; set => SetProperty(ref createdDate, value); }
        public DateTime? CompletedDate { get; set; }
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (SetProperty(ref isCompleted, value))
                {
                    if (isCompleted && CompletedDate is null)
                    {
                        CompletedDate = DateTime.Now;
                        TaskCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
        }
        public string TaskId { get => taskId; set { _ = SetProperty(ref taskId, value); RaisePropertyChanged(nameof(IsValid)); } }
        public ServiceNowType ServiceNowType { get => serviceNowType; set => SetProperty(ref serviceNowType, value); }
        public bool IsServiceNow => ServiceNowType != ServiceNowType.None;
        public string Description
        {
            get => description;
            set
            {
                _ = SetProperty(ref description, value); 
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Requestor { get => requestor; set => SetProperty(ref requestor, value); }
        public string AssignedTo { get => assignedTo; set => SetProperty(ref assignedTo, value); }
        public string Category { get => category; set => SetProperty(ref category, value); }
        public DateTime? DueDate { get => dueDate; set => SetProperty(ref dueDate, value); }
        public DateTime? LastChecked { get => lastChecked; set => SetProperty(ref lastChecked, value); }
        //public string ParentTaskId { get; set; }
        public ObservableCollection<TaskItem> SubTasks { get; set; } = [];

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(TaskId) && !string.IsNullOrEmpty(Description); 
        [JsonIgnore]
        public string DueDateDisplay => DueDate?.ToString("MM/dd/yyyy", new CultureInfo("en-US")) ?? string.Empty;
        [JsonIgnore]
        public string LastCheckedDisplay => LastChecked?.ToString("MM/dd/yyy", new CultureInfo("en-US")) ?? string.Empty;
        [JsonIgnore]
        public bool IsOverDue => DueDate.HasValue && DueDate.Value < DateTime.Now;

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

        internal void Update(TaskItem item)
        {
            IsCompleted = item.IsCompleted;
            Category = item.Category;
            Description = item.Description;
            ServiceNowType = item.ServiceNowType;
            DueDate = item.DueDate;
            LastChecked = item.LastChecked;
            Requestor = item.Requestor;
            AssignedTo = item.AssignedTo;
            CreatedDate = item.CreatedDate;
            CompletedDate = item.CompletedDate;
        }
    }
}
