using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ACMLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool running = false;
        public bool Running
        {
            get
            {
                return this.running;
            }

            set
            {
                this.running = value;
                launchButton.IsEnabled = !running;
                quitButton.IsEnabled = running;
                forceQuitButton.IsEnabled = running;
            }
        }
        
        // This object holds information about the currently running process. It's key to managing the launcher.
        Process Current;
        public MainWindow()
        {
            InitializeComponent();
            Running = false;
            quitButton.IsEnabled = false;
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            String path = executableLocationBox.Text;
            //String extension = path.Substring(path.LastIndexOf('.'));
            if (path.LastIndexOf('.') == -1 || !path.Substring(path.LastIndexOf('.')).Equals(".exe"))
            {
                MessageBox.Show("That is not a windows executable. Windows executables are files which end in \".exe\"", "Invalid Path");
                return;
            }

            // Let's run our valid executable.
            try
            {
                Current = new Process();
                Current.StartInfo.FileName = path;

                // Allows the Process object to raise events. The important event is Exited.
                Current.EnableRaisingEvents = true;

                // Register our event handler (see process_Exited() below).
                Current.Exited += new EventHandler(process_Exited);

                Current.Start();
                Running = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred in starting the process.\n" + ex.Message, "Critical Error");
                Current = null;
                Running = false;
                return;
            }
        }

        private void process_Exited(object sender, EventArgs e)
        {
            // This event handler can deal with the process closing asynchronously; it's a lot nicer than the synchronous method WaitForExit()
            // Because the Exited event will be handled on a different thread from the UI thread, we must use a lambda.
            // See http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            this.Dispatcher.Invoke((Action)(() =>
            {
                Current = null;
                Running = false;
            }));
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            // This is likely what we want for the launcher. It closes the main window of the process, allowing that process time to 
            Current.CloseMainWindow();
        }

        private void forceQuitButton_Click(object sender, RoutedEventArgs e)
        {
            // This will kill the process outright. Should only really be used if process does not exit in enough time.
            Current.Kill();
        }


    }
}
