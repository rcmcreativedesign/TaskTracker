using System.Windows;
using TaskTracker.Helpers;

namespace TaskTracker
{
    public partial class AddTask : Window
    {
        public AddTask()
        {
            InitializeComponent();
        }

        public TaskItem TaskItem { get; set; } = new();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskItem.TaskId) ||
                string.IsNullOrEmpty(TaskItem.Description)) 
            { 
                return;
            }

            DataProcessor.SaveTaskItem(TaskItem);
        }
    }
}