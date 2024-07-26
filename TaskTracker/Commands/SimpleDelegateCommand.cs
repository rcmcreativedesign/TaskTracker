using System;
using System.Windows;
using System.Windows.Input;

namespace TaskTracker.Commands
{
    public class SimpleDelegateCommand(Action<object> executeDelegate, Func<object, bool> canExecuteDelegate) : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public SimpleDelegateCommand(Action<object> executeDelegate) : this(executeDelegate, (o) => true) { }

        public bool CanExecute(object parameter)
        {
            return canExecuteDelegate(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                executeDelegate(parameter);
            }
            catch (Exception)
            {
                MessageBox.Show("Error running command");
            }
        }
    }
}
