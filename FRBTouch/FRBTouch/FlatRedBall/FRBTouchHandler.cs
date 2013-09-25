using FlatRedBall;
using FRBTouch.MultiTouch;
using FRBTouch.MultiTouch.Win32Helper;

namespace FRBTouch.FlatRedBall
{
    public static class FRBTouchHandler
    {
        public static TouchHandler CreateFRBWindowHandler()
        {
            return TouchHandler.CreateTouchHandler(new Win32HwndWrapper(FlatRedBallServices.Game.Window.Handle));
        }
    }
}
