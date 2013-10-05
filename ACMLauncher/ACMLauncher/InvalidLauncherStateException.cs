using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACMLauncher
{
    class InvalidLauncherStateException : Exception
    {
        public InvalidLauncherStateException(String msg) : base(msg)
        {
        }
    }
}
