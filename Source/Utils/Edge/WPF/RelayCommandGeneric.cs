namespace CarbonCore.Utils.Edge.WPF
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

#pragma warning disable SA1649 // File name must match first type name
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
                throw new ArgumentNullException(nameof(execute));
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
#pragma warning restore SA1649 // File name must match first type name
}
