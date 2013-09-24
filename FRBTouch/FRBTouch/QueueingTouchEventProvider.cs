using System;
using System.Collections.Generic;
using FRBTouch.MultiTouch;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public class QueueingTouchEventProvider : ITouchEventProvider, ITouchEventReceiver
    {
        private readonly ICoordinateTranslator _translator;

        public QueueingTouchEventProvider(ICoordinateTranslator translator, TouchHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _translator = translator;
            handler.TouchDown += OnHandlerOnTouchDown;
            handler.TouchMove += OnHandlerOnTouchMove;
            handler.TouchUp += OnHandlerOnTouchUp;
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
                TranslatedPosition = new Vector2(args.Location.X, args.Location.Y),
                NonTranslatedPosition = new Vector2(args.Location.X, args.Location.Y),
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
                    foreach (var t in oldEvents)
                    {
                        t.TranslatedPosition = _translator.TranslateCoordinates(t.TranslatedPosition);
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