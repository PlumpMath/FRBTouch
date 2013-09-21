using System.Collections.Generic;

namespace FRBTouch.MultiTouch
{
    public interface ITouchEventProvider
    {
        IList<TouchEvent> Events { get; }
    }
}