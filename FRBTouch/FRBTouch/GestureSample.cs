using System;
using FRBTouch.MultiTouch;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public struct GestureSample
    {
        private readonly GestureType _gestureType;
        private readonly TimeSpan _timestamp;
        private readonly Vector2 _position;
        private readonly Vector2 _position2;
        private readonly Vector2 _delta;
        private readonly Vector2 _delta2;

        public GestureSample(GestureType gestureType, TimeSpan timestamp, Vector2 position, Vector2 position2,
            Vector2 delta, Vector2 delta2)
        {
            _gestureType = gestureType;
            _timestamp = timestamp;
            _position = position;
            _position2 = position2;
            _delta = delta;
            _delta2 = delta2;
        }

        // Summary:
        //     Holds delta information about the first touchpoint in a multitouch gesture.
        public Vector2 Delta
        {
            get
            {
                return _delta;
            }
        }

        //
        // Summary:
        //     Holds delta information about the second touchpoint in a multitouch gesture.
        public Vector2 Delta2
        {
            get
            {
                return _delta2;
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
        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        //
        // Summary:
        //     Holds the current position of the the second touchpoint in this gesture sample.
        public Vector2 Position2
        {
            get
            {
                return _position2;
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
    }
}