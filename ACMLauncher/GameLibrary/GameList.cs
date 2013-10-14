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
            var dir = new DirectoryInfo(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory));
            dir = dir.Parent.Parent;
            //pathOfFolder.

            foreach (var gameDirectory in dir.GetDirectories())
            {
                var game = new Game();
                //game.PathToGameFolder = gameDirectory.FullName;
                //game.GameName = //parse folder name OR executable
                //game.Executable =
                //game.InializerFile =
                //_gameList.Add(game);
            }

        }

    }
}