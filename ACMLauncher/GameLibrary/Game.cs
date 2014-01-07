using System.IO;
using Newtonsoft.Json.Linq;

namespace ACMLauncher.GameLibrary
{
    public class Game
    {
        public string PathToGameFolder { get; set; }
        public FileInfo Executable { get; set; }
        public string GameName { get; set; }

        //This is file info just so it points to the JSON file
        public FileInfo InializerFile { get; set; }
    }
}