using System;
using System.Collections.ObjectModel;
using System.Windows;
using TaskTracker.Models;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        public event EventHandler<object> WindowClosed;

        public ObservableCollection<string> Categories { get; set; } = [];
    }
}
