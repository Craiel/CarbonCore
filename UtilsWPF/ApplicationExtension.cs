namespace CarbonCore.UtilsWPF
{
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class ApplicationExtension
    {
        public static void DoEvents(this Application application)
        {
            var frame = new DispatcherFrame(true);
            application.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, (SendOrPostCallback)delegate(object arg)
                    {
                        var f = arg as DispatcherFrame;
                        if (f != null)
                        {
                            f.Continue = false;
                        }
                    },
                frame);
            Dispatcher.PushFrame(frame);
        }
    }
}
