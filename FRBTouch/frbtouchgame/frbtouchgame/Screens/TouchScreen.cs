using FlatRedBall;
#if FRB_XNA || SILVERLIGHT

#endif
using FlatRedBall.Input;
using FRBTouch;
using FRBTouch.MultiTouch;
using FRBTouch.MultiTouch.Win32Helper;
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


            //gestureHandler.TouchMove += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite(args.Id.ToString() + " move");

            //gestureHandler.PressAndTap += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite("PressAndTap");
            //gestureHandler.Zoom += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite("Zoom");
            //gestureHandler.Rotate += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite("Rotate");
            //gestureHandler.Pan += (sender, args) =>
            //    FlatRedBall.Debugging.Debugger.CommandLineWrite("Pan");
            //gestureHandler.Begin += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite("Begin");

            //gestureHandler.Pan += (sender, args) => FlatRedBall.Debugging.Debugger.CommandLineWrite("Pan");
        }
        void CustomActivity(bool firstTimeCalled)
		{
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                var gestures = _gestureProvider.GetSamples();

                if (gestures != null)
                {
                    foreach (var gestureSample in gestures)
                    {
                        switch (gestureSample.GestureType)
                        {
                            case GestureType.Tap:
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
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
