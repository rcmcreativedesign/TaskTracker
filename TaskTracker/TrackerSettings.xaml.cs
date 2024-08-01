using System;
using System.Collections.ObjectModel;
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
        public TrackerSettings()
        {
            InitializeComponent();
            var settings = DataProcessor.GetSettings();
            foreach (var item in settings.Categories)
                Categories.Add(item);
        }

        public event EventHandler<object> WindowClosed;

        public ObservableCollection<string> Categories { get; set; } = [];

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //if (InputBox.ShowDialog())

        }
    }
}
