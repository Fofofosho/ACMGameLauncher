using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ACMLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // This object holds information about the currently running process. It's key to managing the launcher.
        private Process _current;
        private bool _running;

        public MainWindow()
        {
            InitializeComponent();
            Running = false;
            quitButton.IsEnabled = false;
        }

        public bool Running
        {
            get { return _running; }

            set
            {
                _running = value;
                launchButton.IsEnabled = !_running;
                quitButton.IsEnabled = _running;
                forceQuitButton.IsEnabled = _running;
            }
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            var path = new FileInfo(executableLocationBox.Text);
            if (path.Extension != ".exe" && path.Extension != ".swf" && path.Extension != ".jar") //TODO: Add check for other file extensions
            {
                MessageBox.Show(
                    "That is not a windows executable. Windows executables are files which end in \".exe\"",
                    "Invalid Path");
                return;
            }

            // Let's run our valid executable.
            try
            {
                _current = new Process();

                _current.StartInfo.FileName = path.ToString();

                //Add working directory info so that we can specify the working directory so our programs know where to grab assets
                //If we don't the programs don't load assets
                _current.StartInfo.WorkingDirectory = Path.GetDirectoryName(path.ToString());

                // Allows the Process object to raise events. The important event is Exited.
                _current.EnableRaisingEvents = true;

                // Register our event handler (see process_Exited() below).
                _current.Exited += process_Exited;

                //pass our start
                _current.Start();
                Running = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred in starting the process.\n" + ex.Message, "Critical Error");
                _current = null;
                Running = false;
            }
        }

        private void process_Exited(object sender, EventArgs e)
        {
            // This event handler can deal with the process closing asynchronously; it's a lot nicer than the synchronous method WaitForExit()
            // Because the Exited event will be handled on a different thread from the UI thread, we must use a lambda.
            // See http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            Dispatcher.Invoke((Action) (() =>
            {
                _current = null;
                Running = false;
            }));
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            // This is likely what we want for the launcher. It closes the main window of the process, allowing that process time to 
            _current.CloseMainWindow();
        }

        private void forceQuitButton_Click(object sender, RoutedEventArgs e)
        {
            // This will kill the process outright. Should only really be used if process does not exit in enough time.
            _current.Kill();
        }
    }
}