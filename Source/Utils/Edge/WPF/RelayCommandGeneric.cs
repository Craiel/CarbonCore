namespace CarbonCore.Utils.Edge.WPF
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> executeAction;
        private readonly Func<T, bool> canExecuteAction;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
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
            return this.canExecuteAction == null || this.canExecuteAction((T)parameter);
        }

        public void Execute(object parameter)
        {
            this.executeAction((T)parameter);
        }
    }
}
