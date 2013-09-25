//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using FRBTouch.MultiTouch.Interop;

namespace FRBTouch.MultiTouch.Win32Helper
{
    public class Win32HwndWrapper : IHwndWrapper
    {
        private readonly IntPtr _hWnd;

        public Win32HwndWrapper(IntPtr hWnd)
        {
            _hWnd = hWnd;

            //HandleCreated += (s, e) => { };
            //HandleDestroyed += (s, e) => { };
        }

        #region IHwndWrapper Members

        public IntPtr Handle
        {
            get { return _hWnd; }
        }

        public object Source
        {
            get { return _hWnd; }
        }

        //public event EventHandler HandleCreated;

        //public event EventHandler HandleDestroyed;

        public bool IsHandleCreated
        {
            get { return User32.IsWindow(_hWnd); }
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point point)
        {
            POINT p = new POINT { x = point.X, y = point.Y };
            User32.ScreenToClient(_hWnd, ref p);
            return new System.Drawing.Point(p.x, p.y);
        }

        #endregion

    }

    /// <summary>
    /// A factory that creates touch or gesture handler for a HWnd based Window
    /// </summary>
    public class Factory
    {
        /// <summary>
        ///  A factory that creates touch or gesture handler for a HWnd based Window
        /// </summary>
        /// <remarks>We use factory to ensure that only one handler will be created for a window, since Gesture and Touch are mutually exclude</remarks>
        /// <typeparam name="T">The handler type</typeparam>
        /// <param name="hWnd">The Windows handle that need touch or gesture events</param>
        /// <returns>TouchHandler or Gesture Handler</returns>
        public static TouchHandler CreateTouchHandler(IntPtr hWnd)
        {
            Win32HwndWrapper win32HwndWrapper = new Win32HwndWrapper(hWnd);
            TouchHandler handler = TouchHandler.CreateTouchHandler(win32HwndWrapper);

            return handler;
        }
    }

}