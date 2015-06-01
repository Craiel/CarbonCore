﻿namespace CarbonCore.ToolFramework.ViewModel
{
    using System;
    using System.Diagnostics;

    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.Utils.Diagnostics;

    public class LogEntryViewModel : BaseViewModel, ILogEntryViewModel
    {
        private TraceEventData data;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DateTime Time
        {
            get
            {
                return this.data != null ? this.data.Time : DateTime.MinValue;
            }
        }

        public TraceEventType Type
        {
            get
            {
                return this.data != null ? this.data.Type : TraceEventType.Error;
            }
        }

        public string Source
        {
            get
            {
                return this.data != null ? this.data.Source : string.Empty;
            }
        }

        public string Message
        {
            get
            {
                if (this.data == null)
                {
                    return string.Empty;
                }

                if (this.data.Args != null && this.data.Args.Length > 0)
                {
                    return string.Format(this.data.Format, this.data.Args);
                }

                return this.data.Format;
            }
        }

        public void SetData(TraceEventData newData)
        {
            this.data = newData;
            this.NotifyPropertyChangedAll();
        }
    }
}
