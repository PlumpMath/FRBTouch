using System;
using System.Collections.Generic;
using System.Linq;
using FRBTouch;
using FRBTouch.MultiTouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Telerik.JustMock.Container;

namespace FRBTouchTests
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class gesture_tests
    {
        private MockingContainer<GestureProvider> _container;

        [TestInitialize]
        public void Initialize()
        {
            _container = new MockingContainer<GestureProvider>();
        }

        [TestMethod]
        public void single_tap_registers_from_one_touch()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                      Id = 1,
                      Position = Vector2.Zero,
                      Action = TouchEvent.TouchEventAction.Move,
                      TimeStamp = DateTime.Now.AddMilliseconds(10)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Up,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                });

            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples as IList<GestureSample> ?? samples.ToList();
            Assert.AreEqual(1, gestureSamples.Count);

            var tap = gestureSamples[0];
            Assert.AreEqual(Vector2.Zero, tap.Delta);
            Assert.AreEqual(Vector2.Zero, tap.Delta2);
            Assert.AreEqual(Vector2.Zero, tap.Position);
        }

        [TestMethod]
        public void drag_registers_from_touchdown_thenmove()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                });

            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples as IList<GestureSample> ?? samples.ToList();
            Assert.AreEqual(1, gestureSamples.Count);
            var drag = gestureSamples[0];
            Assert.AreEqual(GestureType.FreeDrag, drag.GestureType);
            Assert.IsTrue(Math.Abs(drag.Delta.Y - 1.56f) < .0001f);
        }

        [TestMethod]
        public void down_move_then_up_registers_drag_then_dragend()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                }).InSequence();
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Up,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                }).InSequence();


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(1, gestureSamples.Count);

            var drag = gestureSamples[0];
            Assert.AreEqual(GestureType.FreeDrag, drag.GestureType);
            Assert.IsTrue(Math.Abs(drag.Delta.Y - 1.56f) < .0001f);



            samples = gestureProvider.GetSamples();
            gestureSamples = samples.ToList();
            Assert.AreEqual(1, gestureSamples.Count);
            drag = gestureSamples[0];
            Assert.AreEqual(GestureType.DragComplete, drag.GestureType);
        }

        [TestMethod]
        public void fdown_move_up_registers_drag_then_dragend()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Up,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                }).InSequence();


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(2, gestureSamples.Count);

            var drag = gestureSamples[0];
            Assert.AreEqual(GestureType.FreeDrag, drag.GestureType);
            Assert.IsTrue(Math.Abs(drag.Delta.Y - 1.56f) < .0001f);
            drag = gestureSamples[1];
            Assert.AreEqual(GestureType.DragComplete, drag.GestureType);
        }

        [TestMethod]
        public void two_moving_touchpoints_make_a_pinch()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        Position = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        Position = new Vector2(7f, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(1, gestureSamples.Count);

            var pinch = gestureSamples[0];
            Assert.AreEqual(GestureType.Pinch, pinch.GestureType);
            Assert.IsTrue(Math.Abs(pinch.Delta.Y) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.Delta.X - 2) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.Delta2.X - -3) < .0001f);
        }

        [TestMethod]
        public void pinch_then_release_only_returns_one_pinchcomplete_gesture()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        Position = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        Position = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        Position = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        Position = new Vector2(7f, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    },
                    new TouchEvent
                    {
                         Id = 1,
                         Position = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(213.0)
                    },
                    new TouchEvent
                    {
                         Id = 2,
                         Position = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(214.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(2, gestureSamples.Count);

            var gesture = gestureSamples[1];
            Assert.AreEqual(GestureType.PinchComplete, gesture.GestureType);
        }

    }
}
