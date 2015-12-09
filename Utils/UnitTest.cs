namespace CarbonCore.Utils
{
    using System;
    using System.Reflection;

    public static class UnitTest
    {
        private static readonly bool RunningFromNUnit;

        static UnitTest()
        {
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assem.FullName.ToLowerInvariant().StartsWith("nunit.framework"))
                {
                    RunningFromNUnit = true;
                    break;
                }
            }
        }

        public static bool IsRunningFromNunit
        {
            get { return RunningFromNUnit; }
        }
    }
}
