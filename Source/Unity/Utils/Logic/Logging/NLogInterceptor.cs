namespace CarbonCore.Unity.Utils.Logic.Logging
{
    using System;
    using System.Collections.Generic;

    using Logic;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class NLogInterceptor : UnitySingleton<NLogInterceptor>
    {
        private NLogInterceptTarget target;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnLogChanged;
        
        public bool PauseOnError { get; set; }

        public IList<LogEventInfo> Events { get; private set; }

        public IDictionary<LogLevel, int> Count { get; private set; }

        public IList<string> Names { get; private set; }
        
        public override void Initialize()
        {
            base.Initialize();

            this.Events = new List<LogEventInfo>();
            this.Count = new Dictionary<LogLevel, int>();
            this.Names = new List<string>();

            this.target = new NLogInterceptTarget();
            this.target.OnEventReceived += this.OnEventReceived;

            this.Configure();

            UnityEngine.Debug.Log("NLog Interceptor Initialized");
        }

        public void Clear()
        {
            this.Events.Clear();
            this.Count.Clear();
            this.Names.Clear();
        }

        public int GetCount(LogLevel level)
        {
            int result;
            if (this.Count != null && this.Count.TryGetValue(level, out result))
            {
                return result;
            }

            return 0;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Configure()
        {
            var config = new LoggingConfiguration();
            config.AddTarget("intercept", this.target);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/all.log";
            fileTarget.Layout = "${message}";

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, LogLevel.Error, this.target));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, LogLevel.Error, fileTarget));

            LogManager.Configuration = config;
        }

        private void OnEventReceived(LogEventInfo @event)
        {
            this.Events.Add(@event);

            if (!this.Count.ContainsKey(@event.Level))
            {
                this.Count.Add(@event.Level, 1);
            }
            else
            {
                this.Count[@event.Level]++;
            }

            if (@event.Level == LogLevel.Error && this.PauseOnError)
            {
                UnityEngine.Debug.Break();
            }

            if (!this.Names.Contains(@event.LoggerName))
            {
                this.Names.Add(@event.LoggerName);
            }

            if (this.OnLogChanged != null)
            {
                this.OnLogChanged();
            }
        }
    }
}
