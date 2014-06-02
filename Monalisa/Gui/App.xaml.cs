//-----------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Gui
{
    using System.Windows;

    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            if (MessageBox.Show("An unexpected error has occurred.  You should exit this program as soon as possible.\n\n" +
                                "Exit the program now?\n\nError details:\n" + e.Exception.Message,
                                "Unexpected error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)

                Shutdown();
        }
    }
}
