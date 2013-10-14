using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ACMLauncher.GameLibrary;

namespace ACMLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LauncherDelegate
    {
        private readonly GameList _gameManager;
        private readonly Launcher _launcher;
        private readonly ListBox _listBox;

        // This object holds information about the currently running process. It's key to managing the launcher.
        private Process _current;
        private bool _running;

        public MainWindow()
        {
            InitializeComponent();

            SetState(false);
            _launcher = new Launcher(this);

            Running = false;
            QuitButton.IsEnabled = false;
            KeyDown += ForceQuitButton_OnKeyDown;
            _listBox = new ListBox();
            _gameManager = new GameList();
            PopulateListBox();
        }

        public bool Running
        {
            get { return _running; }

            set
            {
                _running = value;
                LaunchButton.IsEnabled = !_running;
                QuitButton.IsEnabled = _running;
                ForceQuitButton.IsEnabled = _running;
            }
        }

        private void PopulateListBox()
        {
            //TODO: Come back to this once the population is finalized
            _gameManager.FindAllGames();
            foreach (var game in _gameManager.getList())
                _listBox.Items.Add(game);
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            var path = new FileInfo(ExecutableLocationBox.Text);
            if (path.Extension != ".exe" && path.Extension != ".swf" && path.Extension != ".jar")
                //TODO: Add check for other file extensions
            {
                MessageBox.Show(
                    "That is not a valid file extension type, your extension is " + path.Extension +
                    "! \nSee documentation for valid extension types!",
                    "Invalid Path");
                return;
            }

            // Let's run our valid executable.
            try
            {
                _launcher.SetProgram(ExecutableLocationBox.Text);
                _launcher.StartProgram();
                SetState(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred in starting the process.\n" + ex.Message, "Critical Error");
                SetState(false);
            }
        }

        public void ApplicationDidQuit()
        {
            SetState(false);

            // This event handler can deal with the process closing asynchronously; it's a lot nicer than the synchronous method WaitForExit()
            // Because the Exited event will be handled on a different thread from the UI thread, we must use a lambda.
            // See http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            Dispatcher.Invoke(new Action(() =>
            {
                _current = null;
                Running = false;
            }));
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

        private void SetState(bool running)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LaunchButton.IsEnabled = !running;
                QuitButton.IsEnabled = running;
                ForceQuitButton.IsEnabled = running;
            }));
        }

        private void ForceQuitButton_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.B && Running && ExecutableLocationBox.IsFocused == false)
                _current.Kill();
        }

        private void selectProcess_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}