using System;
using System.Collections.Generic;
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
        private Category category;
        private DateTime? dueDate;
        private DateTime? lastChecked;

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
        public Category Category { get => category; set => SetProperty(ref category, value); }
        public DateTime? DueDate { get => dueDate; set => SetProperty(ref dueDate, value); }
        public DateTime? LastChecked { get => lastChecked; set => SetProperty(ref lastChecked, value); }
        [JsonIgnore]
        public bool IsValid { get => !string.IsNullOrEmpty(TaskId) && !string.IsNullOrEmpty(Description); }
        [JsonIgnore]
        public string DueDateDisplay => DueDate?.ToString("MM/dd/yyyy", new CultureInfo("en-US")) ?? string.Empty;
        [JsonIgnore]
        public string LastCheckedDisplay => LastChecked?.ToString("MM/dd/yyy", new CultureInfo("en-US")) ?? string.Empty;

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
            Category = item.Category;
            Description = item.Description;
            ServiceNowType = item.ServiceNowType;
            DueDate = item.DueDate;
            LastChecked = item.LastChecked;
            Requestor = item.Requestor;
            AssignedTo = item.AssignedTo;
        }
    }
}
