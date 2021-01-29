using System.Windows;

namespace MFlow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Overrides of Application

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mainWindow = new Window { Width = 1, Height = 1};
            Current.MainWindow = mainWindow;
                
            mainWindow.Show();
            mainWindow.Hide();
            
            var application = new Integration.Application();
            
            application.Run(() =>
            {
                mainWindow.Close();
            });
        }
        
        #endregion
    }
}