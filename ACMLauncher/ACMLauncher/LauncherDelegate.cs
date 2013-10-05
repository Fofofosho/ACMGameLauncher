using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACMLauncher
{
    // Callback for the Exited event on Process, so a GUI can react.
    interface LauncherDelegate
    {
        void ApplicationDidQuit();
    }
}
