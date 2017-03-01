namespace CarbonCore.Utils.Edge.WPF
{
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class ApplicationExtension
    {
#if !__MonoCS__
        public static void DoEvents(this Application application)
        {
            var frame = new DispatcherFrame(true);
            application.Dispatcher.Invoke(
                DispatcherPriority.Background,
                (SendOrPostCallback)(arg =>
                {
                    var f = arg as DispatcherFrame;
                    if (f != null)
                    {
                        f.Continue = false;
                    }
                }),
                frame);

            Dispatcher.PushFrame(frame);
        }
#endif
    }
}
