using System;
using System.Windows.Input;

namespace AutoIndustrial2
{
    public class RelayCommand : ICommand
    {
        private Action mAction;
        private Func<bool> mFunc;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action action)
        {
            mAction = action;
            mFunc = null;
        }

        public RelayCommand(Action action, Func<bool> func)
        {
            mAction = action;
            mFunc = func;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        
        public bool CanExecute(object parameter)
        {
            if (mFunc == null)
                return true;
            return mFunc();
        }

        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
