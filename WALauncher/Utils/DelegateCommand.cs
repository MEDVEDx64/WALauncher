using System;
using System.Windows.Input;

namespace WALauncher.Utils
{
    public class DelegateCommand : ICommand
    {
        Action action = null;

        public DelegateCommand(Action a)
        {
            action = a;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        Action<T> action = null;

        public DelegateCommand(Action<T> a)
        {
            action = a;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }
    }
}
