using System;
using System.Drawing;
using System.Runtime.InteropServices;
using FRBTouch.MultiTouch.Interop;

namespace FRBTouch.MultiTouch
{
    /// <summary>
    ///  GestureNotify event notifies a window that gesture recognition is
    //   in progress and a gesture will be generated if one is recognized under the
    //   current gesture settings.
    /// </summary>
    public class GestureNotifyEventArgs : EventArgs
    {
        internal GestureNotifyEventArgs(IntPtr lparam)
        {
            User32.GESTURENOTIFYSTRUCT gestureNotify = 
                (User32.GESTURENOTIFYSTRUCT)Marshal.PtrToStructure(lparam, typeof(User32.GESTURENOTIFYSTRUCT));
            
            //TODO: Do we need to be DPI aware also here? 
            Location = new Point(gestureNotify.ptsLocation.x, gestureNotify.ptsLocation.y);
            TargetHwnd = gestureNotify.hwndTarget;
        }

        /// <summary>
        /// The gesture location
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// The gesture target window
        /// </summary>
        public IntPtr TargetHwnd { get; private set; }
    }
}