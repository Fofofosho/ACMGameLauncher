using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ACMLauncher.GameLibrary
{
    public class GameList : ObservableCollection<Game>
    {
        //It's my special type

        public GameList()
        {
            
        }

    }

}