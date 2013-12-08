using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                //var json = gameDirectory.GetFiles("*.json").FirstOrDefault() as JArray;
                //var game = new Game 
                //         {  PathToGameFolder    = gameDirectory.FullName, 
                //            GameName            = gameDirectory.Name,
                //            Executable          = gameDirectory.GetFiles("*.exe").FirstOrDefault(),
                //            InializerFile       = json
                //         };

                //_gameList.Add(game);
            }

        }

    }
}