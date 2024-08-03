using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : Window
    {
        public EditTask()
        {
            InitializeComponent();
            PopulateCategories();
        }

        public event EventHandler<TaskItem> WindowClosed;

        public TaskItem TaskItem { get; set; } = new();

        public ObservableCollection<string> Categories { get; set; }

        private void PopulateCategories()
        {
            var settings = DataProcessor.GetSettings();
            Categories = new ObservableCollection<string>(settings.Categories);
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
    }
}
