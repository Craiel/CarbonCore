namespace CarbonCore.UtilsWPF.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Threading;

    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        private bool suspendNotify;

        private ReadOnlyObservableCollection<T> readOnly;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ExtendedObservableCollection()
        {
            this.NotifyChangesAsReset = true;
        }

        public ExtendedObservableCollection(IEnumerable<T> source)
            : base(source)
        {
            this.NotifyChangesAsReset = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool NotifyChangesAsReset { get; set; }

        public ReadOnlyObservableCollection<T> AsReadOnly
        {
            get
            {
                return this.readOnly ?? (this.readOnly = new ReadOnlyObservableCollection<T>(this));
            }
        }

        public void AddItems(IEnumerable<T> items, bool replaceExisting = false)
        {
            if (items == null)
            {
                throw new ArgumentException();
            }

            var itemArray = items as T[] ?? items.ToArray();
            if (itemArray.Length <= 0)
            {
                throw new ArgumentException();
            }

            using (this.BlockReentrancy())
            {
                bool suspendState = this.suspendNotify;
                if (!suspendState)
                {
                    this.SuspendNotification();
                }

                if (replaceExisting)
                {
                    this.Clear();
                }

                foreach (T item in itemArray)
                {
                    this.Add(item);
                }

                if (!suspendState)
                {
                    this.ResumeNotification();
                    if (this.NotifyChangesAsReset)
                    {
                        this.NotifyCollectionChange(NotifyCollectionChangedAction.Reset);
                    }
                    else
                    {
                        this.NotifyCollectionChange(NotifyCollectionChangedAction.Add, itemArray);
                    }
                }
            }
        }

        public void RemoveItems(ICollection<T> items)
        {
            if (items == null || items.Count <= 0)
            {
                throw new ArgumentException();
            }

            using (this.BlockReentrancy())
            {
                bool suspendState = this.suspendNotify;
                if (!suspendState)
                {
                    this.SuspendNotification();
                }

                foreach (T item in items)
                {
                    this.Remove(item);
                }

                if (!suspendState)
                {
                    this.ResumeNotification();
                    if (this.NotifyChangesAsReset)
                    {
                        this.NotifyCollectionChange(NotifyCollectionChangedAction.Reset);
                    }
                    else
                    {
                        this.NotifyCollectionChange(NotifyCollectionChangedAction.Add, items);
                    }
                }
            }
        }

        public ExtendedObservableCollectionSuspendRegion<T> BeginSuspendNotification()
        {
            return new ExtendedObservableCollectionSuspendRegion<T>(this);
        }

        public void SuspendNotification()
        {
            this.suspendNotify = true;
        }

        public void ResumeNotification()
        {
            this.suspendNotify = false;
        }

        public void NotifyReset()
        {
            this.NotifyCollectionChange(NotifyCollectionChangedAction.Reset);
        }

        public void Sort(IComparer<T> comparer = null)
        {
            this.Sort(0, this.Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer = null)
        {
            bool suspendState = this.suspendNotify;
            if (!suspendState)
            {
                this.SuspendNotification();
            }

            if (this.Items as List<T> == null)
            {
                return;
            }

            (this.Items as List<T>).Sort(index, count, comparer);

            if (!suspendState)
            {
                this.ResumeNotification();
                this.NotifyCollectionChange(NotifyCollectionChangedAction.Reset);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.suspendNotify)
            {
                return;
            }

            using (this.BlockReentrancy())
            {
                NotifyCollectionChangedEventHandler eventHandler = this.CollectionChanged;
                if (eventHandler == null)
                {
                    return;
                }

                Delegate[] delegates = eventHandler.GetInvocationList();
                foreach (NotifyCollectionChangedEventHandler handler in delegates)
                {
                    // If the subscriber is a DispatcherObject and different thread
                    var dispatcherObject = handler.Target as DispatcherObject;
                    if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                    {
                        // Invoke handler in the target dispatcher's thread... 
                        // asynchronously for better responsiveness
                        dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                    }
                    else
                    {
                        // Execute handler as is
                        handler(this, e);
                    }
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void NotifyCollectionChange(NotifyCollectionChangedAction action, IEnumerable<T> touchedItems = null)
        {
            if (this.CollectionChanged == null)
            {
                return;
            }

            NotifyCollectionChangedEventArgs args;
            switch (action)
            {
                case NotifyCollectionChangedAction.Reset:
                    {
                        args = new NotifyCollectionChangedEventArgs(action);
                        this.OnCollectionChanged(args);
                        break;
                    }

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    {
                        args = new NotifyCollectionChangedEventArgs(action, (IList)touchedItems);
                        this.OnCollectionChanged(args);
                        break;
                    }

                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }
    }
}
