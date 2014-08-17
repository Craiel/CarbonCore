namespace CarbonCore.ToolFramework.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using CarbonCore.ToolFramework.Annotations;
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.UtilsWPF;

    public abstract class BaseViewModel : IBaseViewModel
    {
        private ICommand commandUndo;
        private ICommand commandRedo;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event PropertyChangingCancellableEventHandler PropertyChanging;
        public event PropertyChangedDetailedEventHandler PropertyChangedDetailed;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CommandUndo
        {
            get
            {
                return this.commandUndo ?? (this.commandUndo = new RelayCommand(this.OnUndo));
            }
        }

        public ICommand CommandRedo
        {
            get
            {
                return this.commandRedo ?? (this.commandRedo = new RelayCommand(this.OnRedo));
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string source = null)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.CheckPropertyChange(source);
            this.PropertyChanged(this, new PropertyChangedEventArgs(source));
        }

        protected void NotifyPropertyChangedExplicit(string propertyName)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(propertyName), "Explicit notify must have value, call All instead!");

            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChangedAll()
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected void NotifyPropertyChangedDetailed(object oldValue, object newValue, string source)
        {
            if (this.PropertyChangedDetailed == null)
            {
                return;
            }

            this.PropertyChangedDetailed(this, oldValue, newValue, new PropertyChangedEventArgs(source));
        }

        protected bool NotifyPropertyChanging(PropertyChangingEventArgs args)
        {
            if (this.PropertyChanging != null)
            {
                if (!string.IsNullOrEmpty(args.PropertyName))
                {
                    throw new InvalidDataException("NotifyProperty called with invalid arguments");
                }

                return this.PropertyChanging(this, args);
            }

            return true;
        }

        protected void PropertySet<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
            where T : class
        {
            if (target == value)
            {
                return;
            }

            if (!this.NotifyPropertyChanging(new PropertyChangingEventArgs(propertyName)))
            {
                return;
            }

            T oldValue = target;
            target = value;

            // Todo: Create undo / redo event

            this.NotifyPropertyChangedDetailed(oldValue, value, propertyName);
            this.NotifyPropertyChangedExplicit(propertyName);
        }

        protected void PropertySet<T>(T target, T value, out T result, [CallerMemberName] string propertyName = null)
            where T : struct
        {
            result = target;
            if (target.Equals(value))
            {
                return;
            }

            if (!this.NotifyPropertyChanging(new PropertyChangingEventArgs(propertyName)))
            {
                return;
            }

            T oldValue = result;
            result = value;

            // Todo: Create undo / redo event

            this.NotifyPropertyChangedDetailed(oldValue, value, propertyName);
            this.NotifyPropertyChangedExplicit(propertyName);
        }

        protected void InvokeMainThread(Action action)
        {
            if (Dispatcher.CurrentDispatcher != Application.Current.Dispatcher)
            {
                Application.Current.Dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }

        protected void DoEvents()
        {
            if (Application.Current != null)
            {
                Application.Current.DoEvents();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnUndo()
        {
            throw new System.NotImplementedException();
        }

        private void OnRedo()
        {
            throw new System.NotImplementedException();
        }

        private void CheckPropertyChange(string propertyName)
        {
            PropertyInfo info = this.GetType().GetProperty(propertyName);
#if DEBUG
            System.Diagnostics.Debug.Assert(info != null, "Property missing", "Property {0} does not exist on {1}", this.GetType(), propertyName);
#else
            System.Diagnostics.Trace.TraceWarning("PropertyChanged on {0} with invalid property {1}", this.GetType(), propertyName);
#endif
        }
    }
}
