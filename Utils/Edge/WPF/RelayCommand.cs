using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CarbonCore.Utils.Edge.WPF
{
    public class RelayCommand : ICommand
    {
        private readonly Action executeAction;
        private readonly Func<bool> canExecuteAction;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.executeAction = execute;
            this.canExecuteAction = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecuteAction != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecuteAction != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.canExecuteAction == null || this.canExecuteAction();
        }

        public void Execute(object parameter)
        {
            this.executeAction();
        }
    }
}
