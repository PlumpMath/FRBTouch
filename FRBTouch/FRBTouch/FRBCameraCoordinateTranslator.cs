using FlatRedBall;
using FlatRedBall.Math;
using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public class FRBCameraCoordinateTranslator : ICoordinateTranslator
    {
        private readonly Camera _main;

        public FRBCameraCoordinateTranslator(Camera main)
        {
            _main = main;
        }

        public Vector2 TranslateCoordinates(Vector2 position)
        {
            float x = 0, y = 0;
            MathFunctions.WindowToAbsolute((int)position.X, (int)position.Y, ref x, ref y, 0, _main, Camera.CoordinateRelativity.RelativeToWorld);
            return new Vector2(x, y);
        }
    }
}