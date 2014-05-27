﻿namespace CarbonCore.Utils
{
    using System.Diagnostics;
    using System.Reflection;

    using CarbonCore.Utils.IO;

    public static class RuntimeInfo
    {
        static RuntimeInfo()
        {
            UpdateRuntimeInfo();
        }

        public static Assembly Assembly { get; private set; }

        public static int ProcessId { get; private set; }

        public static string ProcessName { get; private set; }

        public static string AssemblyName { get; private set; }

        public static CarbonDirectory Path { get; private set; }

        private static void UpdateRuntimeInfo()
        {
            if (ProcessName == null)
            {
                Process process = Process.GetCurrentProcess();
                ProcessId = process.Id;
                ProcessName = process.ProcessName;
            }

            if (AssemblyName == null)
            {
                Assembly = UnitTest.IsRunningFromNunit ? Assembly.GetExecutingAssembly() : Assembly.GetEntryAssembly();
                AssemblyName = System.IO.Path.GetFileName(Assembly.Location);
                Path = Assembly.GetDirectory();
            }
        }
    }
}
