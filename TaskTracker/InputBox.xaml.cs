using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window, INotifyPropertyChanged
    {
        private string textLabel;
        private string text;

        public InputBox() : this("Enter a Category:", string.Empty) { }

        public InputBox(string label, string defaultText)
        {
            InitializeComponent();
            textBox.Focus();

            TextLabel = label;
            Text = defaultText;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string TextLabel { get => textLabel; set => SetProperty(ref textLabel, value); }
        public string Text { get => text; set => SetProperty(ref text, value); }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
            Close();
        }

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
