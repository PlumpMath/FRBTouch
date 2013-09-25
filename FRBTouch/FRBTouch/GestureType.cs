using System;

namespace FRBTouch
{
    [Flags]
    public enum GestureType
    {
        // Summary:
        //     Represents no gestures.
        None = 0,
        //
        // Summary:
        //     The user briefly touched a single point on the screen.
        Tap = 1,
        //
        // Summary:
        //     The user tapped the screen twice in quick succession. This always is preceded
        //     by a Tap gesture. If the time between taps is too great to be considered
        //     a DoubleTap, two Tap gestures will be generated instead.
        DoubleTap = 2,
        //
        // Summary:
        //     The user touched a single point on the screen for approximately one second.
        //     This is a single event, and not continuously generated while the user is
        //     holding the touchpoint.
        Hold = 4,
        //
        // Summary:
        //     The user touched the screen, and then performed a horizontal (left to right
        //     or right to left) gesture.
        HorizontalDrag = 8,
        //
        // Summary:
        //     The user touched the screen, and then performed a vertical (top to bottom
        //     or bottom to top) gesture.
        VerticalDrag = 16,
        //
        // Summary:
        //     The user touched the screen, and then performed a free-form drag gesture.
        FreeDrag = 32,
        //
        // Summary:
        //     The user touched two points on the screen, and then converged or diverged
        //     them. Pinch behaves like a two-finger drag. When this gesture is enabled,
        //     it takes precedence over drag gestures while two fingers are down.
        Pinch = 64,
        //
        // Summary:
        //     The user performed a touch combined with a quick swipe of the screen. Flicks
        //     are positionless. The velocity of the flick can be retrieved by reading the
        //     Delta member of GestureSample.
        Flick = 128,
        //
        // Summary:
        //     A drag gesture (VerticalDrag, HorizontalDrag, or FreeDrag) was completed.
        //     This signals only completion. No position or delta data is valid for this
        //     sample.
        DragComplete = 256,
        //
        // Summary:
        //     A pinch operation was completed. This signals only completion. No position
        //     or delta data is valid for this sample.
        PinchComplete = 512,
    }
}