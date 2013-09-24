using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FRBTouch.MultiTouch;

namespace FRBTouch.FlatRedBall
{
    public class FRBGestureProvider : GestureProvider
    {
        public FRBGestureProvider(ITouchEventProvider touchEventProvider = null) : base(touchEventProvider ?? new QueueingTouchEventProvider(new FRBCameraCoordinateTranslator(Camera.Main), FRBTouchHandler.CreateFRBWindowHandler()))
        {
        }
    }
}
