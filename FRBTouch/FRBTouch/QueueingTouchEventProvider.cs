using System;
using System.Collections.Generic;
using FRBTouch.MultiTouch;
using FRBTouch.MultiTouch.Win32Helper;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public class QueueingTouchEventProvider : ITouchEventProvider, ITouchEventReceiver
    {
        private readonly ICoordinateTranslator _mockTranslator;

        public QueueingTouchEventProvider(IntPtr handle, ICoordinateTranslator mockTranslator, TouchHandler handler = null)
        {
            _mockTranslator = mockTranslator;
            var handler1 = handler ?? Handler.CreateHandler<TouchHandler>(new Win32HwndWrapper(handle));
            handler1.TouchDown += OnHandlerOnTouchDown;
            handler1.TouchMove += OnHandlerOnTouchMove;
            handler1.TouchUp += OnHandlerOnTouchUp;
        }

        private void OnHandlerOnTouchUp(object sender, TouchEventArgs touchEventArgs)
        {
            _queuedEvents.Add(CreateTouchEvent(touchEventArgs, TouchEvent.TouchEventAction.Up));
        }

        private void OnHandlerOnTouchMove(object sender, TouchEventArgs touchEventArgs)
        {
            _queuedEvents.Add(CreateTouchEvent(touchEventArgs, TouchEvent.TouchEventAction.Move));
        }

        private void OnHandlerOnTouchDown(object sender, TouchEventArgs args)
        {
            _queuedEvents.Add(CreateTouchEvent(args, TouchEvent.TouchEventAction.Down));
        }

        private TouchEvent CreateTouchEvent(TouchEventArgs args, TouchEvent.TouchEventAction action)
        {
            return new TouchEvent
            {
                Action = action,
                Id = args.Id,
                Position = new Vector2(args.Location.X, args.Location.Y),
                TimeStamp = DateTime.Now
            };
        }

        private List<TouchEvent> _queuedEvents = new List<TouchEvent>();

        public IList<TouchEvent> Events
        {
            get
            {
                if (_queuedEvents.Count > 0)
                {
                    var oldEvents = _queuedEvents;
                    _queuedEvents = new List<TouchEvent>();
                    foreach (TouchEvent t in oldEvents)
                    {
                        t.Position = _mockTranslator.TranslateCoordinates(t.Position);
                    }
                    return oldEvents;
                }
                return null;
            }
        }

        public void AddEvent(TouchEvent eventToAdd)
        {
            _queuedEvents.Add(eventToAdd);
        }
    }
}