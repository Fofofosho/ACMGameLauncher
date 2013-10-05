using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ACMLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LauncherDelegate
    {
        private Launcher _launcher;

        public MainWindow()
        {
            InitializeComponent();
            setState(false);
            _launcher = new Launcher(this);
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                _launcher.SetProgram(executableLocationBox.Text);
                _launcher.StartProgram();
                setState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred in starting the process.\n" + ex.Message, "Critical Error");
                setState(false);

            }
        }

        public void ApplicationDidQuit()
        {
            setState(false);
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            // This is likely what we want for the launcher. It closes the main window of the process, allowing that process time to 
            _launcher.QuitProgram();
        }

        private void forceQuitButton_Click(object sender, RoutedEventArgs e)
        {
            // This will kill the process outright. Should only really be used if process does not exit in enough time.
            _launcher.ForceQuitProgram();
        }

        private void setState(bool running)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                launchButton.IsEnabled = !running;
                quitButton.IsEnabled = running;
                forceQuitButton.IsEnabled = running;
            }));
        }
    }
}