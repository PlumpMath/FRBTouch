using System;

namespace FRBTouch.MultiTouch
{
    /// <summary>
    /// A Common interface foir timer.
    /// The timer has to be in the UI thread context
    /// </summary>
    public interface IGUITimer : IDisposable
    {
        /// <summary>
        /// Gets or sets whether the timer is running.
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// Gets or sets the time, in milliseconds, before the Tick event is raised
        /// </summary>
        int Interval { get; set; }
        
        /// <summary>
        ///   Occurs when the specified timer interval has elapsed and the timer is enabled.
        /// </summary>
        event EventHandler Tick;

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}