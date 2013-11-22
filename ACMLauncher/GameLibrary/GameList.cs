using System;
using System.Collections.Generic;
using System.IO;

namespace ACMLauncher.GameLibrary
{
    public class GameList
    {
        //TODO: CHANGE THIS FILE -- CURRENTLY BEING WORKED ON

        private List<Game> _gameList;

        public GameList()
        {
            _gameList = new List<Game>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Game> getList()
        {
            return _gameList;
        }

        /// <summary>
        /// Needs to find all games and populate the list dynamically
        /// </summary>
        public void FindAllGames()
        {
            //TODO: Decide to make a folder somewhere on the arcade cabinet and set the directory path equal to that
            //var gameLibraryDir = new DirectoryInfo(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));

            var folderList = new DirectoryInfo("C:\\GameLibrary").GetDirectories();
            //pathOfFolder.

            foreach (var gameDirectory in folderList)
            {
                var game = new Game();
                game.PathToGameFolder = gameDirectory.FullName;
                game.GameName = gameDirectory.Name;
                
                //TODO: Fix this >>
                //game.Executable = gameDirectory.GetFiles("*.exe");

                //game.InializerFile =
                //_gameList.Add(game);
            }

        }

    }
}