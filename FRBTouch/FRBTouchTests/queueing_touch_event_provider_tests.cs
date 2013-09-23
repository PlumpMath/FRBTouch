using System;
using FRBTouch;
using FRBTouch.MultiTouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Container;
using Telerik.JustMock.Helpers;

namespace FRBTouchTests
{
    [TestClass]
// ReSharper disable once InconsistentNaming
    public class queueing_touch_event_provider_tests
    {
        [TestMethod]
        public void once_events_are_retrieved_theyre_wiped()
        {
            // Arrange
            var translator = Mock.Create<ICoordinateTranslator>();
            var provider = new QueueingTouchEventProvider(new IntPtr(), translator);
            provider.AddEvent(new TouchEvent
            {
                Id=1,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                Position = Vector2.Zero
            });

            provider.AddEvent(new TouchEvent
            {
                Id = 2,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                Position = Vector2.Zero
            });

            // Act
            var events = provider.Events;
            var eventsAfter = provider.Events;

            // Assert
            Assert.AreEqual(2, events.Count);
            Assert.IsNull(eventsAfter);
        }

        [TestMethod]
        public void touch_event_provider_uses_translator_to_get_coordinates()
        {
            // Arrange
            var mockTranslator = Mock.Create<ICoordinateTranslator>();
            var mockHandler = Mock.Create<TouchHandler>();

            mockTranslator
                .Arrange(t => t.TranslateCoordinates(Arg.IsAny<Vector2>()))
                .Returns(Vector2.One)
                .Occurs(2);

            var provider = new QueueingTouchEventProvider(new IntPtr(), mockTranslator, mockHandler);

            provider.AddEvent(new TouchEvent
            {
                Id=1,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                Position = Vector2.Zero
            });

            provider.AddEvent(new TouchEvent
            {
                Id = 2,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                Position = Vector2.Zero
            });

            // Act
            var events = provider.Events;

            // Assert
            mockTranslator.AssertAll();
            Assert.AreEqual(Vector2.One, events[0].Position);
            Assert.AreEqual(Vector2.One, events[1].Position);
        }
    }
}
