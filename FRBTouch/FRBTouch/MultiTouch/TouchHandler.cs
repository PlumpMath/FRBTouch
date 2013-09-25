//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FRBTouch.MultiTouch;
using FRBTouch.MultiTouch.Interop;

namespace FRBTouch.MultiTouch
{
    /// <summary>
    /// Base class for handling Gesture and Touch event
    /// </summary>
    /// <remarks>
    /// A form can have one handler, either touch handler or gesture handler. 
    /// The form need to create the handler and register to events. 
    /// The code is not thread safe, we assume that the calling thread is the 
    /// UI thread. There is no blocking operation in the code.
    /// </remarks>
    public class TouchHandler
    {
        private readonly IHwndWrapper _hWndWrapper;
        static private readonly List<object> ControlInUse = new List<object>();
        static private User32.WindowProcDelegate _windowProcDelegate;
        private IntPtr _originalWindowProcId;

        /// <summary>
        /// Initiate touch support for the underline hWnd 
        /// </summary>
        /// <remarks>Registering the hWnd to touch support or configure the hWnd to receive gesture messages</remarks>
        /// <returns>true if succeeded</returns>
        protected bool SetHWndTouchInfo()
        {
            return User32.RegisterTouchWindow(ControlHandle, _disablePalmRejection ? User32.TouchWindowFlag.WantPalm : 0);
        }

        public static TouchHandler CreateTouchHandler(IHwndWrapper hWndWrapper)
        {
            if (ControlInUse.Contains(hWndWrapper.Source))
                throw new Exception("Only one handler can be registered for a control.");

            //hWndWrapper.HandleDestroyed += (s, e) => ControlInUse.Remove(s);
            //_controlInUse.Add(hWndWrapper.Source);

            return new TouchHandler(hWndWrapper);
        }


        /// <summary>
        /// We create the hanlder using a factory method.
        /// </summary>
        /// <param name="hWndWrapper">The control or Window that registered for touch/gesture events</param>
        internal TouchHandler(IHwndWrapper hWndWrapper)
        {
            _hWndWrapper = hWndWrapper;

            Initialize();

        }

        /// <summary>
        /// Connect the handler to the Control
        /// </summary>
        /// <remarks>
        /// The trick is to subclass the Control and intercept touch/gesture events, then reflect
        /// them back to the control.
        /// </remarks>
        private void Initialize()
        {
            if (!SetHWndTouchInfo())
            {
                throw new NotSupportedException("Cannot register window");
            }

            _windowProcDelegate = WindowProcSubClass;

            //According to the SDK doc SetWindowLongPtr should be exported by both 32/64 bit O/S
            //But it does not.
            _originalWindowProcId = IntPtr.Size == 4 ?
                User32.SubclassWindow(_hWndWrapper.Handle, User32.GWLP_WNDPROC, _windowProcDelegate) :
                User32.SubclassWindow64(_hWndWrapper.Handle, User32.GWLP_WNDPROC, _windowProcDelegate);

            //take the desktop DPI
            using (Graphics graphics = Graphics.FromHwnd(_hWndWrapper.Handle))
            {
                IntPtr desktop = graphics.GetHdc();

                int Xdpi = User32.GetDeviceCaps(desktop, (int)User32.DeviceCap.LOGPIXELSX);
                int Ydpi = User32.GetDeviceCaps(desktop, (int)User32.DeviceCap.LOGPIXELSY);
                DpiX = Xdpi;
                DpiY = Ydpi;
            }

            //WindowMessage += (s, e) => { };
        }

        /// <summary>
        /// Intercept touch/gesture events using Windows subclassing
        /// </summary>
        /// <param name="hWnd">The hWnd of the registered form</param>
        /// <param name="msg">The WM code</param>
        /// <param name="wparam">The WM WParam</param>
        /// <param name="lparam">The WM LParam</param>
        /// <returns></returns>
        private uint WindowProcSubClass(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            //WindowMessage(this, new WMEventArgs(hWnd, msg, wparam, lparam));

            if (msg == User32.WM_GESTURENOTIFY && GestureNotify != null)
            {
                GestureNotify(this, new GestureNotifyEventArgs(lparam));
            }
            else
            {

                uint result = WindowProc(hWnd, msg, wparam, lparam);

                if (result != 0)
                    return result;
            }

            return User32.CallWindowProc(_originalWindowProcId, hWnd, msg, wparam, lparam);
        }


        /// <summary>
        /// The registered control wrapper
        /// </summary>
        internal IHwndWrapper HWndWrapper
        {
            get
            {
                return _hWndWrapper;
            }
        }

        /// <summary>
        /// The registered control's handler
        /// </summary>
        protected IntPtr ControlHandle
        {
            get
            {
                return _hWndWrapper.IsHandleCreated ? _hWndWrapper.Handle : IntPtr.Zero;
            }
        }

        /// <summary>
        /// The X DPI of the target window
        /// </summary>
        public float DpiX { get; private set; }

        /// <summary>
        /// The Y DPI of the target window
        /// </summary>
        public float DpiY { get; private set; }

        /// <summary>
        ///  GestureNotify event notifies a window that gesture recognition is
        //   in progress and a gesture will be generated if one is recognized under the
        //   current gesture settings.
        /// </summary>
        public event EventHandler<GestureNotifyEventArgs> GestureNotify;

        /// <summary>
        /// Enable advanced message handling/blocking
        /// </summary>
        internal event EventHandler<WMEventArgs> WindowMessage;

        /// <summary>
        /// Report digitizer capabilities
        /// </summary>
        public static class DigitizerCapabilities
        {
            /// <summary>
            /// Get the current Digitizer Status
            /// </summary>
            public static DigitizerStatus Status
            {
                get
                {
                    return (DigitizerStatus)User32.GetDigitizerCapabilities(User32.DigitizerIndex.SM_DIGITIZER);
                }
            }

            /// <summary>
            /// Get the maximum touches capability
            /// </summary>
            public static int MaxumumTouches
            {
                get
                {
                    return User32.GetDigitizerCapabilities(User32.DigitizerIndex.SM_MAXIMUMTOUCHES);
                }
            }

            /// <summary>
            /// Check for integrated touch support
            /// </summary>
            public static bool IsIntegratedTouch
            {
                get
                {
                    return (Status & DigitizerStatus.IntegratedTouch) != 0;
                }
            }

            /// <summary>
            /// Check for external touch support
            /// </summary>
            public static bool IsExternalTouch
            {
                get
                {
                    return (Status & DigitizerStatus.ExternalTouch) != 0;
                }
            }

            /// <summary>
            /// Check for Pen support
            /// </summary>
            public static bool IsIntegratedPan
            {
                get
                {
                    return (Status & DigitizerStatus.IntegratedPan) != 0;
                }
            }

            /// <summary>
            /// Check for external Pan support
            /// </summary>
            public static bool IsExternalPan
            {
                get
                {
                    return (Status & DigitizerStatus.ExternalPan) != 0;
                }
            }

            /// <summary>
            /// Check for multi-input
            /// </summary>
            public static bool IsMultiInput
            {
                get
                {
                    return (Status & DigitizerStatus.MultiInput) != 0;
                }
            }

            /// <summary>
            /// Check if touch device is ready
            /// </summary>
            public static bool IsStackReady
            {
                get
                {
                    return (Status & DigitizerStatus.StackReady) != 0;
                }
            }

            /// <summary>
            /// Check if Multi-touch support device is ready
            /// </summary>
            public static bool IsMultiTouchReady
            {
                get
                {
                    return (Status & (DigitizerStatus.StackReady | DigitizerStatus.MultiInput)) != 0;
                }
            }
        }

        /// <summary>
        /// Check if the Window is registered for multitouch events
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool IsTouchWindows(IntPtr hWnd)
        {
            uint cap;
            return User32.IsTouchWindow(hWnd, out cap);
        }

        private bool _disablePalmRejection;

        /// <summary>
        /// Enabling this flag disables palm rejection
        /// </summary>
        public bool DisablePalmRejection
        {
            get
            {
                return _disablePalmRejection;
            }
            set
            {
                if (_disablePalmRejection == value)
                    return;

                _disablePalmRejection = value;

                if (HWndWrapper.IsHandleCreated)
                {
                    User32.UnregisterTouchWindow(HWndWrapper.Handle);
                    SetHWndTouchInfo();
                }
            }
        }

        /// <summary>
        /// Intercept and fire touch events
        /// </summary>
        /// <param name="hWnd">The Windows Handle</param>
        /// <param name="msg">Windows Message</param>
        /// <param name="wparam">wParam</param>
        /// <param name="lparam">lParam</param>
        /// <returns></returns>
        protected uint WindowProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            switch (msg)
            {
                case User32.WM_TOUCH:

                    foreach (TouchEventArgs arg in DecodeMessage(hWnd, msg, wparam, lparam, DpiX, DpiY))
                    {
                        if (TouchDown != null && arg.IsTouchDown)
                        {
                            TouchDown(this, arg);
                        }

                        if (TouchMove != null && arg.IsTouchMove)
                        {
                            TouchMove(this, arg);
                        }

                        if (TouchUp != null && arg.IsTouchUp)
                        {
                            TouchUp(this, arg);
                        }
                    }
                    return 1; //handled
            }
            return 0;
        }


        /// <summary>
        /// Decode the message and create a collection of event arguments
        /// </summary>
        /// <remarks>
        /// One Windows message can result a group of events
        /// </remarks>
        /// <returns>An enumerator of thr resuting events</returns>
        /// <param name="hWnd">the WndProc hWnd</param>
        /// <param name="msg">the WndProc msg</param>
        /// <param name="wParam">the WndProc wParam</param>
        /// <param name="lParam">the WndProc lParam</param>
        private IEnumerable<TouchEventArgs> DecodeMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, float dpiX, float dpiY)
        {
            // More than one touchinput may be associated with a touch message,
            // so an array is needed to get all event information.
            int inputCount = User32.LoWord(wParam.ToInt32()); // Number of touch inputs, actual per-contact messages

            TOUCHINPUT[] inputs; // Array of TOUCHINPUT structures
            inputs = new TOUCHINPUT[inputCount]; // Allocate the storage for the parameters of the per-contact messages
            try
            {
                // Unpack message parameters into the array of TOUCHINPUT structures, each
                // representing a message for one single contact.
                if (!User32.GetTouchInputInfo(lParam, inputCount, inputs, Marshal.SizeOf(inputs[0])))
                {
                    // Get touch info failed.
                    throw new Exception("Error calling GetTouchInputInfo API");
                }

                // For each contact, dispatch the message to the appropriate message
                // handler.
                // Note that for WM_TOUCHDOWN you can get down & move notifications
                // and for WM_TOUCHUP you can get up & move notifications
                // WM_TOUCHMOVE will only contain move notifications
                // and up & down notifications will never come in the same message
                for (int i = 0; i < inputCount; i++)
                {
                    TouchEventArgs touchEventArgs = new TouchEventArgs(HWndWrapper, dpiX, dpiY, ref inputs[i]);
                    yield return touchEventArgs;
                }
            }
            finally
            {
                User32.CloseTouchInputHandle(lParam);
            }
        }


        // Touch event handlers

        /// <summary>
        /// Register to receive TouchDown Events
        /// </summary>
        public event EventHandler<TouchEventArgs> TouchDown;   // touch down event handler

        /// <summary>
        /// Register to receive TouchUp Events
        /// </summary>
        public event EventHandler<TouchEventArgs> TouchUp;     // touch up event handler

        /// <summary>
        /// Register to receive TouchMove Events
        /// </summary>
        public event EventHandler<TouchEventArgs> TouchMove;   // touch move event handler
    }


    /// <summary>
    /// EventArgs passed to Touch handlers 
    /// </summary>
    public class TouchEventArgs : EventArgs
    {
        private readonly IHwndWrapper _hWndWrapper;
        private readonly float _dpiXFactor;
        private readonly float _dpiYFactor;

        /// <summary>
        /// Create new touch event argument instance
        /// </summary>
        /// <param name="hWndWrapper">The target control</param>
        /// <param name="touchInput">one of the inner touch input in the message</param>
        internal TouchEventArgs(IHwndWrapper hWndWrapper, float dpiX, float dpiY, ref TOUCHINPUT touchInput)
        {
            _hWndWrapper = hWndWrapper;
            _dpiXFactor = 96F / dpiX;
            _dpiYFactor = 96F / dpiY;
            DecodeTouch(ref touchInput);
        }

        private bool CheckFlag(int value)
        {
            return (Flags & value) != 0;
        }



        // Decodes and handles WM_TOUCH* messages.
        private void DecodeTouch(ref TOUCHINPUT touchInput)
        {
            // TOUCHINFO point coordinates and contact size is in 1/100 of a pixel; convert it to pixels.
            // Also convert screen to client coordinates.
            if ((touchInput.dwMask & User32.TOUCHINPUTMASKF_CONTACTAREA) != 0)
                ContactSize = new Size(AdjustDpiX(touchInput.cyContact / 100), AdjustDpiY(touchInput.cyContact / 100));

            Id = touchInput.dwID;

            Point p = _hWndWrapper.PointToClient(new Point(touchInput.x / 100, touchInput.y / 100));
            Location = p;
            //Location = new Point(AdjustDpiX(p.X), AdjustDpiY(p.Y));

            Time = touchInput.dwTime;
            TimeSpan ellapse = TimeSpan.FromMilliseconds(Environment.TickCount - touchInput.dwTime);
            AbsoluteTime = DateTime.Now - ellapse;

            Mask = touchInput.dwMask;
            Flags = touchInput.dwFlags;
        }


        private int AdjustDpiX(int value)
        {
            return (int)(value * _dpiXFactor);
        }

        private int AdjustDpiY(int value)
        {
            return (int)(value * _dpiYFactor);
        }

        /// <summary>
        /// Touch client coordinate in pixels
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// A touch point identifier that distinguishes a particular touch input
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// A set of bit flags that specify various aspects of touch point
        /// press, release, and motion. 
        /// </summary>
        public int Flags { get; private set; }

        /// <summary>
        /// mask which fields in the structure are valid
        /// </summary>
        public int Mask { get; private set; }

        /// <summary>
        /// touch event time
        /// </summary>
        public DateTime AbsoluteTime { get; private set; }

        /// <summary>
        /// touch event time from system up
        /// </summary>
        public int Time { get; private set; }

        /// <summary>
        /// the size of the contact area in pixels
        /// </summary>
        public Size? ContactSize { get; private set; }

        /// <summary>
        /// Is Primary Contact (The first touch sequence)
        /// </summary>
        public bool IsPrimaryContact
        {
            get { return (Flags & User32.TOUCHEVENTF_PRIMARY) != 0; }
        }

        /// <summary>
        /// Specifies that movement occurred
        /// </summary>
        public bool IsTouchMove
        {
            get { return CheckFlag(User32.TOUCHEVENTF_MOVE); }
        }

        /// <summary>
        /// Specifies that the corresponding touch point was established through a new contact
        /// </summary>
        public bool IsTouchDown
        {
            get { return CheckFlag(User32.TOUCHEVENTF_DOWN); }
        }

        /// <summary>
        /// Specifies that a touch point was removed
        /// </summary>
        public bool IsTouchUp
        {
            get { return CheckFlag(User32.TOUCHEVENTF_UP); }
        }

        /// <summary>
        /// Specifies that a touch point is in range
        /// </summary>
        public bool IsTouchInRange
        {
            get { return CheckFlag(User32.TOUCHEVENTF_INRANGE); }
        }

        /// <summary>
        /// specifies that this input was not coalesced.
        /// </summary>
        public bool IsTouchNoCoalesce
        {
            get { return CheckFlag(User32.TOUCHEVENTF_NOCOALESCE); }
        }

        /// <summary>
        /// Specifies that the touch point is associated with a pen contact
        /// </summary>
        public bool IsTouchPen
        {
            get { return CheckFlag(User32.TOUCHEVENTF_PEN); }
        }

        /// <summary>
        /// The touch event came from the user's palm
        /// </summary>
        /// <remarks>Set <see cref="DisablePalmRejection"/> to true</remarks>
        public bool IsTouchPalm
        {
            get { return CheckFlag(User32.TOUCHEVENTF_PALM); }
        }
    }
}