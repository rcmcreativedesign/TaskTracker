using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TaskTracker.Helpers;
using TaskTracker.Models;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class TrackerSettings : Window
    {
        private readonly Settings settings;

        public TrackerSettings()
        {
            InitializeComponent();
            settings = DataProcessor.GetSettings();
            foreach (var item in settings.Categories)
                Categories.Add(item);
        }

        public event EventHandler<object> WindowClosed;

        public ObservableCollection<string> Categories { get; set; } = [];

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var inputBox = new InputBox();
            inputBox.ShowDialog();
            if (inputBox.Text.Length > 0)
                Categories.Add(inputBox.Text);
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            settings.Categories = [.. Categories];
            DataProcessor.SaveSettings(settings);
            Close();
        }
    }
}
