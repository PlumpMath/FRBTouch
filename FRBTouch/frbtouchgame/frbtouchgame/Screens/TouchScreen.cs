using System.Windows;
using FlatRedBall;
#if FRB_XNA || SILVERLIGHT

#endif
using FlatRedBall.Input;
using FlatRedBall.Math.Geometry;
using FRBTouch;
using FRBTouch.MultiTouch;
using FRBTouch.MultiTouch.Win32Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace frbtouchgame.Screens
{
    public partial class TouchScreen
    {
        private GestureProvider _gestureProvider;

        void CustomInitialize()
        {
            _gestureProvider = new GestureProvider(new QueueingTouchEventProvider(FlatRedBallServices.Game.Window.Handle));
        }

        void CustomActivity(bool firstTimeCalled)
        {
            var gestures = _gestureProvider.GetSamples();

            if (gestures != null)
            {
                foreach (var gestureSample in gestures)
                {
                    switch (gestureSample.GestureType)
                    {
                        case GestureType.Tap:
                            var rectangle = new AxisAlignedRectangle
                            {
                                Position = new Vector3(gestureSample.Position, 0),
                                Visible = true
                            };

                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Tap");
                            break;
                        case GestureType.FreeDrag:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Drag");
                            break;
                        case GestureType.Pinch:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Pinch");
                            break;
                        case GestureType.DragComplete:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("DragComplete");
                            break;
                        case GestureType.PinchComplete:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("PinchComplete");
                            break;
                    }
                }
            }
        }

        void CustomDestroy()
        {


        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
