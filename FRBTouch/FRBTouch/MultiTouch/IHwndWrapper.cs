using System;
using System.Drawing;

namespace FRBTouch.MultiTouch
{
    /// <summary>
    /// Wrapp HWND source such as System.Windows.Forms.Control, or System.Windows.Window
    /// </summary>
    public interface IHwndWrapper
    {
        /// <summary>
        /// The Underline Windows Handle
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// The .NET object that wrap the Windows Handle (WinForm, WinForm Control, WPF Window, IntPtr of HWND)
        /// </summary>
        object Source { get; }

        /// <summary>
        /// The Win32 Handle has been created
        /// </summary>
        event EventHandler HandleCreated;

        /// <summary>
        /// /// The Win32 Handle has been destroyed
        /// </summary>
        event EventHandler HandleDestroyed;

        /// <summary>
        /// Check if the Win32 Handle is already created
        /// </summary>
        bool IsHandleCreated { get; }

        /// <summary>
        /// Computes the location of the specified screen point into client coordinates
        /// </summary>
        /// <param name="point">The screen coordinate System.Drawing.Point to convert</param>
        /// <returns>A point that represents the converted point in client coordinates</returns>
        Point PointToClient(Point point);
    }
}