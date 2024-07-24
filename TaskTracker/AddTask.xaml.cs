using System.Windows;

namespace TaskTracker
{
    public partial class AddTask : Window
    {
        public AddTask()
        {
            InitializeComponent();
        }

        public TaskItem TaskItem { get; set; }
    }
}