using System;
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
        private readonly bool _init;

        //TODO
        /*
         * Will contain usage information
        Create a .log file in the C://GameLibrary
        
        **Format
        Timestamp -- What occured
         */

        public MainWindow()
        {
            _init = true;

            InitializeComponent();

            _launcher = new Launcher(this);

            Running = false;
            KeyDown += SelectGame_OnKeyDown;
            _gameManager = new GameList();
            PopulateListBox();

            _init = false;
        }

        public bool Running { get; set; }

        public void ApplicationDidQuit()
        {
            //SetState(false);

            // This event handler can deal with the process closing asynchronously; it's a lot nicer than the synchronous method WaitForExit()
            // Because the Exited event will be handled on a different thread from the UI thread, we must use a lambda.
            // See http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            Dispatcher.Invoke(new Action(() =>
            {
                Running = false;
            }));

        }

        private void PopulateListBox()
        {
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

        private void SelectGame_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.R && e.Key != Key.Z) return;

            var selectedGame = _gameManager.First(x => x.Information.Title.Equals(GameListBox.SelectedItem));

            //Run our selectedGame's executable
            try
            {
                if (selectedGame == null) return;
                _launcher.SetProgram(selectedGame.Executable.FullName);
                _launcher.StartProgram();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred in starting the process.\n" + ex.Message, "Critical Error");
            }
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

            //TODO - add information that is in the json file to the screen

        }

        //Handles the moving the selector upwards in the listbox
        private void SelectUp_ListBox(object sender, CanExecuteRoutedEventArgs e)
        {
            if (GameListBox.SelectedIndex > 0)
                GameListBox.SelectedIndex--;

            //Required for re-entry of MainWindow
            GameListBox.Focus();
        }

        //Handles the moving the selector downwards in the listbox
        private void SelectDown_ListBox(object sender, CanExecuteRoutedEventArgs e)
        {
            if (GameListBox.SelectedIndex < GameListBox.Items.Count)
                GameListBox.SelectedIndex++;

            //Required for re-entry of MainWindow
            GameListBox.Focus();
        }
    }
}