namespace CarbonCore.Tests.Edge.ToolFramework
{
    using System.Windows;

    using NUnit.Framework;

    public partial class FrameworkTestMainWindow
    {
        public FrameworkTestMainWindow()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Do some testing to see if we had the proper view model
            Assert.AreEqual(this.Title, "FrameworkTest");

            this.Close();
        }
    }
}
