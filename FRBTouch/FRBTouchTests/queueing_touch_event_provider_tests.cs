using System;
using FRBTouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Telerik.JustMock;
using Telerik.JustMock.Container;

namespace FRBTouchTests
{
    [TestClass]
// ReSharper disable once InconsistentNaming
    public class queueing_touch_event_provider_tests
    {
        private MockingContainer<QueueingTouchEventProvider> _container;


        [TestInitialize]
        public void Initialize()
        {
            _container = new MockingContainer<QueueingTouchEventProvider>();
        }

        [TestMethod]
        public void once_events_are_retrieved_theyre_wiped()
        {
            // Arrange
            var provider = _container.Instance;
            provider.AddEvent(new TouchEvent
            {
                Id=1,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                TranslatedPosition = Vector2.Zero
            });

            provider.AddEvent(new TouchEvent
            {
                Id = 2,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                TranslatedPosition = Vector2.Zero
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
            _container
                .Arrange <ICoordinateTranslator>(t => t.TranslateCoordinates(Arg.IsAny<Vector2>()))
                .Returns(Vector2.One)
                .Occurs(2);

            var provider = _container.Instance;

            provider.AddEvent(new TouchEvent
            {
                Id=1,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                TranslatedPosition = Vector2.Zero
            });

            provider.AddEvent(new TouchEvent
            {
                Id = 2,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                TranslatedPosition = Vector2.Zero
            });

            // Act
            var events = provider.Events;

            // Assert
            _container.AssertAll();
            Assert.AreEqual(Vector2.One, events[0].TranslatedPosition);
            Assert.AreEqual(Vector2.One, events[1].TranslatedPosition);
        }

        [TestMethod]
        public void touch_events_return_both_translated_and_untranslated_vectors()
        {
            // Arrange
            _container
                .Arrange<ICoordinateTranslator>(t => t.TranslateCoordinates(Arg.IsAny<Vector2>()))
                .Returns(Vector2.One);

            var provider = _container.Instance;

            provider.AddEvent(new TouchEvent
            {
                Id = 1,
                TimeStamp = DateTime.Now,
                Action = TouchEvent.TouchEventAction.Down,
                TranslatedPosition = new Vector2(1, 3),
                NonTranslatedPosition = new Vector2(1, 3)
            });

            // Act
            var events = provider.Events;

            // Assert
            Assert.AreEqual(Vector2.One, events[0].TranslatedPosition);
            Assert.AreEqual(new Vector2(1, 3), events[0].NonTranslatedPosition);
        }
    }
}
