using System;
using FRBTouch.MultiTouch;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public struct GestureSample
    {
        private readonly GestureType _gestureType;
        private readonly TimeSpan _timestamp;
        private readonly Vector2 _worldPosition;
        private readonly Vector2 _worldPosition2;
        private readonly Vector2 _worldDelta;
        private readonly Vector2 _worldDelta2;
        private readonly Vector2 _screenPosition;
        private readonly Vector2 _screenPosition2;
        private readonly Vector2 _screenDelta;
        private readonly Vector2 _screenDelta2;

        public GestureSample(GestureType gestureType, TimeSpan timestamp, Vector2 worldPosition, Vector2 worldPosition2,
            Vector2 worldDelta, Vector2 worldDelta2, Vector2 screenPosition, Vector2 screenPosition2, Vector2 screenDelta, Vector2 screenDelta2)
        {
            _gestureType = gestureType;
            _timestamp = timestamp;
            _worldPosition = worldPosition;
            _worldPosition2 = worldPosition2;
            _worldDelta = worldDelta;
            _worldDelta2 = worldDelta2;
            _screenPosition = screenPosition;
            _screenPosition2 = screenPosition2;
            _screenDelta = screenDelta;
            _screenDelta2 = screenDelta2;
        }

        // Summary:
        //     Holds delta information about the first touchpoint in a multitouch gesture.
        public Vector2 WorldDelta
        {
            get
            {
                return _worldDelta;
            }
        }

        //
        // Summary:
        //     Holds delta information about the second touchpoint in a multitouch gesture.
        public Vector2 WorldDelta2
        {
            get
            {
                return _worldDelta2;
            }
        }

        //
        // Summary:
        //     The type of gesture in a multitouch gesture sample.
        public GestureType GestureType
        {
            get
            {
                return _gestureType;
            }
        }

        //
        // Summary:
        //     Holds the current position of the first touchpoint in this gesture sample.
        public Vector2 WorldPosition
        {
            get
            {
                return _worldPosition;
            }
        }

        //
        // Summary:
        //     Holds the current position of the the second touchpoint in this gesture sample.
        public Vector2 WorldPosition2
        {
            get
            {
                return _worldPosition2;
            }
        }

        //
        // Summary:
        //     Holds the starting time for this touch gesture sample.
        public TimeSpan Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public Vector2 ScreenPosition
        {
            get { return _screenPosition; }
        }

        public Vector2 ScreenPosition2
        {
            get { return _screenPosition2; }
        }

        public Vector2 ScreenDelta
        {
            get { return _screenDelta; }
        }

        public Vector2 ScreenDelta2
        {
            get { return _screenDelta2; }
        }
    }
}