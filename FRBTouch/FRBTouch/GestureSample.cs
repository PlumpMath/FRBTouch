using System;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public struct GestureSample
    {
        public GestureSample(GestureType gestureType, TimeSpan timestamp, Vector2 worldPosition, Vector2 worldPosition2,
            Vector2 worldDelta, Vector2 worldDelta2, Vector2 screenPosition, Vector2 screenPosition2, Vector2 screenDelta, Vector2 screenDelta2) : this()
        {
            GestureType = gestureType;
            Timestamp = timestamp;
            WorldPosition = worldPosition;
            WorldPosition2 = worldPosition2;
            WorldDelta = worldDelta;
            WorldDelta2 = worldDelta2;
            ScreenPosition = screenPosition;
            ScreenPosition2 = screenPosition2;
            ScreenDelta = screenDelta;
            ScreenDelta2 = screenDelta2;
        }

        // Summary:
        //     Holds delta information about the first touchpoint in a multitouch gesture.
        public Vector2 WorldDelta { get; set; }

        //
        // Summary:
        //     Holds delta information about the second touchpoint in a multitouch gesture.
        public Vector2 WorldDelta2 { get; set; }

        //
        // Summary:
        //     The type of gesture in a multitouch gesture sample.
        public GestureType GestureType { get; set; }

        //
        // Summary:
        //     Holds the current position of the first touchpoint in this gesture sample.
        public Vector2 WorldPosition { get; set; }

        //
        // Summary:
        //     Holds the current position of the the second touchpoint in this gesture sample.
        public Vector2 WorldPosition2 { get; set; }

        //
        // Summary:
        //     Holds the starting time for this touch gesture sample.
        public TimeSpan Timestamp { get; set; }

        public Vector2 ScreenPosition { get; set; }

        public Vector2 ScreenPosition2 { get; set; }

        public Vector2 ScreenDelta { get; set; }

        public Vector2 ScreenDelta2 { get; set; }
    }
}