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
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                      Id = 1,
                      TranslatedPosition = Vector2.Zero,
                      Action = TouchEvent.TouchEventAction.Move,
                      TimeStamp = DateTime.Now.AddMilliseconds(10)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.Zero,
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
            Assert.AreEqual(Vector2.Zero, tap.WorldDelta);
            Assert.AreEqual(Vector2.Zero, tap.WorldDelta2);
            Assert.AreEqual(Vector2.Zero, tap.WorldPosition);
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
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(0, 1.56f),
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
            Assert.IsTrue(Math.Abs(drag.WorldDelta.Y - 1.56f) < .0001f);
        }

        [TestMethod]
        public void down_move_then_up_registers_drag_then_dragcomplete()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(0, 1.56f),
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
                        TranslatedPosition = new Vector2(0, 1.56f),
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
            Assert.IsTrue(Math.Abs(drag.WorldDelta.Y - 1.56f) < .0001f);



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
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(0, 1.56f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(0, 1.56f),
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
            Assert.IsTrue(Math.Abs(drag.WorldDelta.Y - 1.56f) < .0001f);
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
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(7f, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();

            var pinch = gestureSamples[0];
            Assert.AreEqual(GestureType.Pinch, pinch.GestureType);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta.Y) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta.X - 2) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta2.X - -3) < .0001f);
        }

        [TestMethod]
        public void pinch_returns_freedrag_gesture_for_center_point()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(7f, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(2, gestureSamples.Count);

            var pinch = gestureSamples[0];
            Assert.AreEqual(GestureType.Pinch, pinch.GestureType);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta.Y) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta.X - 2) < .0001f);
            Assert.IsTrue(Math.Abs(pinch.WorldDelta2.X - -3) < .0001f);

            var drag = gestureSamples[1];
            Assert.AreEqual(GestureType.FreeDrag, drag.GestureType);
            Assert.IsTrue(Math.Abs(drag.WorldPosition.X - 2.5f) < .0001f);
            Assert.IsTrue(Math.Abs(drag.WorldDelta.X - -2.5) < .0001f);
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
                        TranslatedPosition = Vector2.Zero,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(7f, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    },
                    new TouchEvent
                    {
                         Id = 1,
                         TranslatedPosition = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(213.0)
                    },
                    new TouchEvent
                    {
                         Id = 2,
                         TranslatedPosition = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(214.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(3, gestureSamples.Count);

            var gesture = gestureSamples[2];
            Assert.AreEqual(GestureType.PinchComplete, gesture.GestureType);
            Assert.AreNotEqual(GestureType.PinchComplete, gestureSamples[0].GestureType);
            Assert.AreNotEqual(GestureType.PinchComplete, gestureSamples[1].GestureType);
        }

        [TestMethod]
        public void tap_gesture_returns_translated_and_untranslated_coordinates()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 2),
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                      Id = 1,
                      TranslatedPosition = new Vector2(2, 2),
                      NonTranslatedPosition = Vector2.One,
                      Action = TouchEvent.TouchEventAction.Move,
                      TimeStamp = DateTime.Now.AddMilliseconds(10)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 2),
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Up,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                });

            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples as IList<GestureSample> ?? samples.ToList();
            Assert.AreEqual(new Vector2(2, 2), gestureSamples.First().WorldPosition);
            Assert.AreEqual(Vector2.One, gestureSamples.First().ScreenPosition);
        }

        [TestMethod]
        public void drag_gesture_returns_translated_and_untranslated_coordinates()
        {
            // Arrange
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.One,
                        NonTranslatedPosition = new Vector2(2, 2),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(1, 1.56f),
                        NonTranslatedPosition = new Vector2(2, 3),
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                });

            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples as IList<GestureSample> ?? samples.ToList();
            var drag = gestureSamples[0];
            Assert.AreEqual(new Vector2(1, 1.56f), drag.WorldPosition);
            Assert.AreEqual(new Vector2(2, 3), drag.ScreenPosition);
            Assert.IsTrue(Math.Abs(drag.WorldDelta.Y - 0.56f) < .00001f);
            Assert.AreEqual(new Vector2(0, 1f), drag.ScreenDelta);
        }

        [TestMethod]
        public void drag_complete_returns_translated_and_nontranslated_coordinates()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(0, 1.56f),
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
                        TranslatedPosition = new Vector2(0, 1.56f),
                        NonTranslatedPosition = new Vector2(1, 5),
                        Action = TouchEvent.TouchEventAction.Up,
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    }
                }).InSequence();


            var gestureProvider = _container.Instance;

            // Act
// ReSharper disable once RedundantAssignment
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert

            samples = gestureProvider.GetSamples();
            var gestureSamples = samples.ToList();
            var dragcomplete = gestureSamples[0];
            Assert.AreEqual(GestureType.DragComplete, dragcomplete.GestureType);
            Assert.IsTrue(Math.Abs(dragcomplete.WorldPosition.X) < .00001f);
            Assert.IsTrue(Math.Abs(dragcomplete.WorldPosition.Y - 1.56f) < .00001f);
            Assert.IsTrue(Math.Abs(dragcomplete.ScreenPosition.X - 1f) < .00001f);
            Assert.IsTrue(Math.Abs(dragcomplete.ScreenPosition.Y - 5f) < .00001f);
        }

        [TestMethod]
        public void pinch_complete_returns_translated_and_nontranslated_coordinates()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.Zero,
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        NonTranslatedPosition = new Vector2(3, 3),
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(7f, 0f),
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    },
                    new TouchEvent
                    {
                         Id = 1,
                         TranslatedPosition = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(213.0)
                    },
                    new TouchEvent
                    {
                         Id = 2,
                         TranslatedPosition = new Vector2(7f, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(214.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(3, gestureSamples.Count);

            var pinchComplete = gestureSamples[2];
            Assert.AreEqual(GestureType.PinchComplete, pinchComplete.GestureType);
            Assert.AreEqual(new Vector2(2, 0), pinchComplete.WorldPosition);
            Assert.AreEqual(new Vector2(7, 0), pinchComplete.WorldPosition2);
            Assert.AreEqual(new Vector2(3, 3), pinchComplete.ScreenPosition);
            Assert.AreEqual(Vector2.One, pinchComplete.ScreenPosition2);
        }

        [TestMethod]
        public void pinch_returns_translated_and_nontranslated_coordinates()
        {
            // Arrange
            _container
                .Arrange<ITouchEventProvider>(p => p.Events)
                .Returns(new List<TouchEvent>
                {
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = Vector2.Zero,
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(10f, 0),
                        Action = TouchEvent.TouchEventAction.Down,
                        TimeStamp = DateTime.Now.AddMilliseconds(100.0)
                    },
                    new TouchEvent
                    {
                        Id = 1,
                        TranslatedPosition = new Vector2(2, 0f),
                        Action = TouchEvent.TouchEventAction.Move,
                        NonTranslatedPosition = new Vector2(3, 3),
                        TimeStamp = DateTime.Now.AddMilliseconds(200.0)
                    },
                    new TouchEvent
                    {
                        Id = 2,
                        TranslatedPosition = new Vector2(7f, 0f),
                        NonTranslatedPosition = Vector2.One,
                        Action = TouchEvent.TouchEventAction.Move,
                        TimeStamp = DateTime.Now.AddMilliseconds(210.0)
                    },
                    new TouchEvent
                    {
                         Id = 1,
                         TranslatedPosition = new Vector2(2, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(213.0)
                    },
                    new TouchEvent
                    {
                         Id = 2,
                         TranslatedPosition = new Vector2(7f, 0f),
                         Action = TouchEvent.TouchEventAction.Up,
                         TimeStamp = DateTime.Now.AddMilliseconds(214.0)
                    }
                });


            var gestureProvider = _container.Instance;

            // Act
            IEnumerable<GestureSample> samples = gestureProvider.GetSamples();

            // Assert
            var gestureSamples = samples.ToList();
            Assert.AreEqual(3, gestureSamples.Count);

            var pinchComplete = gestureSamples[0];
            Assert.AreEqual(GestureType.Pinch, pinchComplete.GestureType);
            Assert.AreEqual(new Vector2(2, 0), pinchComplete.WorldPosition);
            Assert.AreEqual(new Vector2(7, 0), pinchComplete.WorldPosition2);
            Assert.AreEqual(new Vector2(3, 3), pinchComplete.ScreenPosition);
            Assert.AreEqual(Vector2.One, pinchComplete.ScreenPosition2);
        }

        [TestMethod]
        public void double_tap_gesture_returns_for_up_to_500ms_taps()
        {
            var now = DateTime.Now;
            // Arrange
            _container.Arrange<ITouchEventProvider>(t => t.Events).Returns(new List<TouchEvent>
            {
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Down,
                    Id=1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Move,
                    Id=1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now.AddMilliseconds(10.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Up,
                    Id = 1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now.AddMilliseconds(20.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Down,
                    Id=2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(518.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Move,
                    Id=2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(519.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Up,
                    Id = 2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(520.0)
                }
            });

            // Act
            var samples = _container.Instance.GetSamples().ToList();

            // Assert
            Assert.AreEqual(2, samples.Count);
            Assert.AreEqual(GestureType.Tap, samples[0].GestureType);
            Assert.AreEqual(GestureType.DoubleTap, samples[1].GestureType);
            Assert.AreEqual(Vector2.One, samples[0].ScreenPosition);
            Assert.AreEqual(Vector2.One, samples[0].WorldPosition);
            Assert.AreEqual(new Vector2(3, 3), samples[1].ScreenPosition);
            Assert.AreEqual(new Vector2(4, 4), samples[1].WorldPosition);
        }

        [TestMethod]
        public void two_taps_return_for_over_500ms_taps()
        {
            var now = DateTime.Now;
            // Arrange
            _container.Arrange<ITouchEventProvider>(t => t.Events).Returns(new List<TouchEvent>
            {
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Down,
                    Id=1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Move,
                    Id=1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now.AddMilliseconds(10.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Up,
                    Id = 1,
                    NonTranslatedPosition = Vector2.One,
                    TranslatedPosition = Vector2.One,
                    TimeStamp = now.AddMilliseconds(20.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Down,
                    Id=2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(518.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Move,
                    Id=2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(519.0)
                },
                new TouchEvent
                {
                    Action = TouchEvent.TouchEventAction.Up,
                    Id = 2,
                    NonTranslatedPosition = new Vector2(3, 3),
                    TranslatedPosition = new Vector2(4, 4),
                    TimeStamp = now.AddMilliseconds(521.0)
                }
            });

            // Act
            var samples = _container.Instance.GetSamples().ToList();

            // Assert
            Assert.AreEqual(2, samples.Count);
            Assert.AreEqual(GestureType.Tap, samples[0].GestureType);
            Assert.AreEqual(GestureType.Tap, samples[1].GestureType);
            Assert.AreEqual(Vector2.One, samples[0].ScreenPosition);
            Assert.AreEqual(Vector2.One, samples[0].WorldPosition);
            Assert.AreEqual(new Vector2(3, 3), samples[1].ScreenPosition);
            Assert.AreEqual(new Vector2(4, 4), samples[1].WorldPosition);
        }


    }
}
