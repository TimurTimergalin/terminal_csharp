using System;

namespace Terminal
{
    /// <summary>
    /// Main instance of your app.
    /// </summary>
    public class WindowManager
    {
        private Window _running;
        /// <summary>
        /// Holds a window which is currently being displayed.
        /// When is null, the service stops.
        /// </summary>
        /// <seealso cref="Window"/>
        public Window Running
        {
            get => _running;
            set
            {
                _running?.Stop();
                _running = value;
                _running?.Start();
            }
        }

        private bool HandleKey(ConsoleKeyInfo keyInfo)
        {
            return Running != null && Running.HandleKey(keyInfo);
        }

        /// <summary>
        /// Runs the terminal program
        /// </summary>
        public void Serve()
        {
            while (Running != null)
            {
                if (Console.KeyAvailable)
                {
                    HandleKey(Console.ReadKey(true));
                }
            }
        }
    }
}