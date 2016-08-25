using System;
using System.Windows.Input;

namespace WALauncher.Utils
{
    public class DelegateCommand : ICommand
    {
        Action action = null;
        Action<object> pAction = null;

        public DelegateCommand(Action a)
        {
            action = a;
        }

        public DelegateCommand(Action<object> a)
        {
            pAction = a;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(action == null)
            {
                pAction(parameter);
                return;
            }

            action();
        }
    }
}
