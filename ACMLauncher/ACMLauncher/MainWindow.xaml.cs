using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            FindAllGames();
            
            foreach (var game in _gameManager)
            {
                GameListBox.Items.Add(game.GameName);
            }

            GameListBox.SelectedIndex = 0;

            //_listBox.BeginInit();
            //TODO: REVIEW THIS http://stackoverflow.com/questions/6919694/wpf-adding-new-items-to-a-listbox
            //foreach (var game in _gameManager.getList())
            //    var listItem = new ListBoxItem().;
            //    _listBox.Items.Add(game);
            //_listBox.EndInit();
        }

        /// <summary>
        /// Needs to find all games and populate the list dynamically
        /// </summary>
        public void FindAllGames()
        {
            //TODO: Decide to make a folder somewhere on the arcade cabinet and set the directory path equal to that
            //var gameLibraryDir = new DirectoryInfo(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));

            //CreateDirectory will create the directory if it doesn't exist. If it does, it does not try to create it
            var folderList = Directory.CreateDirectory("C:\\GameLibrary").GetDirectories();

            foreach (var gameDirectory in folderList)
            {
                /* What we are requiring
                 * 
                 * Title:
                 * Author:
                 * Version:
                 * Publisher:
                 * Genre:
                 * NumberPlayers:
                 * Description:
                 * ImageFolder:
                 * VideoDemo:
                 * 
                 */

                //TODO change json to be an actual JSON object that can be queriable
                //http://stackoverflow.com/questions/16045569/how-to-access-elements-of-a-jarray
                //This is for MainWindow - to change the image when something is selected
                var json = gameDirectory.GetFiles("*.json").FirstOrDefault();
                var game = new Game
                {
                    PathToGameFolder = gameDirectory.FullName,
                    GameName = gameDirectory.Name,
                    Executable = gameDirectory.GetFiles("*.exe").FirstOrDefault(),
                    InializerFile = json
                };

                _gameManager.Add(game);
            }

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

        //This will read the contents of the JSON file. Update text on screen once selection has changed.
        private void GameList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: GameListBox, make the JSON file work with the below objective
            //Change image based on which item in list is select, grab its images to change in this field.

            var selectedGame = _gameManager.Single(x => x.GameName.Equals(GameListBox.SelectedItem.ToString()));

            GameNameBlock.Text = selectedGame.GameName;
        }
    }
}