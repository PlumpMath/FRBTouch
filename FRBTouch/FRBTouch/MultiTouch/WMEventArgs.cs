using System;

namespace FRBTouch.MultiTouch
{
    /// <summary>
    /// Enable advanced message handling/blocking
    /// </summary>
    internal class WMEventArgs : EventArgs
    {
        public WMEventArgs(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            HWnd = hWnd;
            Message = msg;
            WParam = wparam;
            LParam = lparam;
        }

        public IntPtr HWnd { get; private set; }
        public int Message { get; private set; }
        public IntPtr WParam { get; private set; }
        public IntPtr LParam { get; private set; }
    }
}