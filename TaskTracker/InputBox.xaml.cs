using System.Windows;
using System.Windows.Controls;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
            textBox.Focus();
        }

        public string Text { get; set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
            Close();
        }
    }
}
