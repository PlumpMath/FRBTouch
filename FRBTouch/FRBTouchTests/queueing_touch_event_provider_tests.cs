using System;
using FRBTouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Telerik.JustMock.Container;

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
            var provider = new QueueingTouchEventProvider(new IntPtr());
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
    }
}
