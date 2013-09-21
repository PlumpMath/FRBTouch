using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public struct TouchEvent
    {
        private sealed class IdEqualityComparer : IEqualityComparer<TouchEvent>
        {
            public bool Equals(TouchEvent x, TouchEvent y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(TouchEvent obj)
            {
                return obj.Id;
            }
        }

        private static readonly IEqualityComparer<TouchEvent> IdComparerInstance = new IdEqualityComparer();

        public static IEqualityComparer<TouchEvent> IdComparer
        {
            get { return IdComparerInstance; }
        }

        public bool Equals(TouchEvent other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TouchEvent && Equals((TouchEvent) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public enum TouchEventAction
        {
            Up,
            Down,
            Move
        };

        public int Id { get; set; }
        public TouchEventAction Action { get; set; }

        public Vector2 Position { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}