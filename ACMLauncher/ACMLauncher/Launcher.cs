using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;

namespace ACMLauncher
{
    class Launcher
    {
        private Process _current;
        private bool _running;
        public bool Running
        {
            get { return _running; }

            set
            {
                _running = value;
            }
        }

        private LauncherDelegate _ld;

        public LauncherDelegate LauncherHandler
        {
            set
            {
                _ld = value;
            }
        }

        public Launcher(LauncherDelegate ld)
        {
            _current = new Process();
            _current.EnableRaisingEvents = true;            // Needed for the Exited event
            _current.StartInfo.UseShellExecute = true;      // Needed for launching of jars and swfs without setting Arguments

            //DEBUG
            _current.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            _current.Exited += Process_Exited;

            _ld = ld;
        }

        // Throws an InvalidPathException if the file extension does not match a supported type.
        // Takes an absolute path. IF YOU MODIFY THAT, MAKE SURE YOU CHANGE GETPROGRAM() TO MATCH.
        public void SetProgram(String path)
        {
            var file = new FileInfo(path);
            if (file.Extension != ".exe" && file.Extension != ".swf" && file.Extension != ".jar")
            {
                throw new InvalidLauncherStateException("That is not a valid file extension type, your extension is " + file.Extension + "! \nSee documentation for valid extension types!");
            }
            if (Running)
            {
                throw new InvalidLauncherStateException("You cannot change the process while it is still running");
            }

            // We are now clear to modify the process
            _current.StartInfo.FileName = file.ToString();
            _current.StartInfo.WorkingDirectory = file.Directory.ToString();
        }

        
        public String GetProgram()
        {
            // Assuming that this is an absolute path, which it should be unless SetProgram is modified.
            return _current.StartInfo.FileName;
        }

        // Return the amount of physical memory in use by the process.
        // If the process is not running, return -1.
        public long GetMemorySize()
        {
            if (!Running) return -1;
            return _current.WorkingSet64;
        }

        // Start the program defined earlier. If the program is already running, this is a no-op.
        public void StartProgram()
        {
            if (!Running)
            {
                _current.Start();
                Running = true;
            }
        }

        // Close the MainWindow of the process
        // If the process is not running, this is a no-op
        // May want to rewrite this to spawn a new thread, in the case this is a blocking call (MSDN not conclusive)
        public void QuitProgram()
        {
            if (Running)
            {
                _current.CloseMainWindow();
            }
        }

        // Force close the process
        public void ForceQuitProgram()
        {
            if (Running)
            {
                _current.Kill();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            // This event handler can deal with the process closing asynchronously; it's a lot nicer than the synchronous method WaitForExit()
            // Because the Exited event will be handled on a different thread from the UI thread, we must use a lambda.
            // See http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            Dispatcher.CurrentDispatcher.Invoke((Action) (() =>
            {
                Running = false;
                _ld.ApplicationDidQuit();
            }));
        }
    }
}
