namespace CarbonCore.Utils.Diagnostics
{
    public static class Internal
    {
        public static T NotImplemented<T>(
            string customMessage = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string message = string.Format("Member with return of type {0} is not Implemented: {1}, line {2} in {3}", typeof(T), memberName, sourceLineNumber, sourceFilePath);
            System.Diagnostics.Trace.TraceError(message);
            System.Diagnostics.Trace.Assert(false, message);
            return default(T);
        }

        public static void NotImplemented(
            string customMessage = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string message = string.Format("Member is not Implemented: {0}, line {1} in {2}", memberName, sourceLineNumber, sourceFilePath);
            System.Diagnostics.Trace.TraceError(message);
            System.Diagnostics.Trace.Assert(false, message);
        }
    }
}
