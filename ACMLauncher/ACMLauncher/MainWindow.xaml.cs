using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ACMLauncher.GameLibrary;
using Newtonsoft.Json;

namespace ACMLauncher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LauncherDelegate
    {
        private readonly GameList _gameManager;
        private readonly Launcher _launcher;

        // This object holds information about the currently running process. It's key to managing the launcher.
        private Process _current;
        private bool _running;
        private bool _init;

        public MainWindow()
        {
            _init = true;

            InitializeComponent();

            SetState(false);
            _launcher = new Launcher(this);

            Running = false;
            QuitButton.IsEnabled = false;
            KeyDown += ForceQuitButton_OnKeyDown;
            _gameManager = new GameList();
            PopulateListBox();

            _init = false;
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

        private void PopulateListBox()
        {
            //TODO: Come back to this once the population is finalized
            FindAllGames();

            foreach (var game in _gameManager)
            {
                GameListBox.Items.Add(game.Information.Title);
            }
            GameListBox.Focus();
            GameListBox.SelectedIndex = 0;
        }

        /// <summary>
        ///     Needs to find all games and populate the list dynamically
        /// </summary>
        public void FindAllGames()
        {
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
                 *
                 * FUTURE* Allow for a VideoDemo, would be a video format that would play in the window once a game is selected 
                 */

                var json = gameDirectory.GetFiles("*.json", SearchOption.AllDirectories).FirstOrDefault();

                //Might be a problem in some cases
                if (json == null) continue;
                var game = new Game
                {
                    PathToGameFolder = gameDirectory.FullName,
                    Executable = gameDirectory.GetFiles("*.exe").FirstOrDefault(),
                    InializerFile = json,
                    //Contains all of the information about the game from the JSON file
                    Information = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(json.FullName))
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

        //Update fields on screen once selection has changed.
        private void GameList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //***First time this class is initialized, this throws an exception
            if (_init) return;

            var selectedGame = _gameManager.First(x => x.Information.Title.Equals(GameListBox.SelectedItem));

            //Change all of the information about the game once a new one is selectd
            GameNameBlock.Text = selectedGame.Information.Title ?? "Unavailable";
            GameDescription.Text = selectedGame.Information.Description ?? "Unavailable";

            ExecutableLocationBox.Text = selectedGame.Executable == null ? "Unavailable" : selectedGame.Executable.ToString();
        }
    }
}