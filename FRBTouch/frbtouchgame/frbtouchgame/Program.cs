using System;

namespace frbtouchgame
{
#if WINDOWS || XBOX
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();

            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

