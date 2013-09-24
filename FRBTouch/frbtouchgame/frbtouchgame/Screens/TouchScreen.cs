using FlatRedBall;
#if FRB_XNA || SILVERLIGHT

#endif
using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using FRBTouch;
using FRBTouch.FlatRedBall;
using FRBTouch.MultiTouch;
using Microsoft.Xna.Framework;

namespace frbtouchgame.Screens
{
    public partial class TouchScreen
    {
        private GestureProvider _gestureProvider;

        void CustomInitialize()
        {
            SpriteManager.Camera.Position.Z += 1900;
            SpriteManager.Camera.Position.Y -= 300;

            SpriteManager.Camera.FarClipPlane = 30000f;

            _gestureProvider = new FRBGestureProvider();
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
                                Position = GetWorldCoordinate(gestureSample.WorldPosition),
                                Visible = true,
                                Width = 100,
                                Height = 100
                            };
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Tap" + gestureSample.WorldPosition);
                            break;
                        case GestureType.FreeDrag:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Drag" + gestureSample.WorldPosition);
                            break;
                        case GestureType.Pinch:
                            FlatRedBall.Debugging.Debugger.CommandLineWrite("Pinch");
                            var rectangle3 = new AxisAlignedRectangle
                            {
                                Position = GetWorldCoordinate(gestureSample.WorldPosition),
                                Visible = true,
                                Width = 100,
                                Height = 100
                            };

                            rectangle3 = new AxisAlignedRectangle
                            {
                                Position = GetWorldCoordinate(gestureSample.WorldPosition2),
                                Visible = true,
                                Width = 100,
                                Height = 100
                            };

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

        private static Vector3 GetWorldCoordinate(Vector2 touchPosition)
        {
            return new Vector3(touchPosition, 0);            
            //var touch3d = new Vector3(touchPosition, Camera.Main.Z);
            //var direction = Camera.Main.Position - touch3d;
            //direction.Normalize();
            //var ray = new Ray(Camera.Main.Position, direction);
            //var ray2 = GuiManager.Cursor.GetRay();
            //var touchPlane = new Plane(Vector3.Zero, new Vector3(0, 1, 0), new Vector3(1, 0, 0));
            //var ray3 = MathFunctions.GetRay(0, 0, 1f, Camera.Main);

            float worldx = 0f, worldy = 0f, worldz = 0f;
            MathFunctions.WindowToAbsolute((int)touchPosition.X, (int)touchPosition.Y, ref worldx, ref worldy, worldz, Camera.Main, Camera.CoordinateRelativity.RelativeToWorld);

            return new Vector3
            {
                X = worldx,
                Y = worldy,
                Z = worldz
            };

            ////Get the distance to the intersetion point
            //var distance = ray.Intersects(touchPlane);

            //var worldCoordinates = Vector3.Zero;
            ////Make sure the cursor intersects
            //if (distance.HasValue)
            //{
            //    //Get the direction vector for the ray
            //    Vector3 translationVector = ray.Direction;
            //    translationVector.Normalize();
            //    //Set the distance of the vector to the distance to the intersection point
            //    translationVector = Vector3.Multiply(translationVector, (float)distance);
            //    //Get the translation matrix
            //    Matrix translationMatrix = Matrix.CreateTranslation(translationVector);
            //    //Translate the mouse cursor point to the intersection point
            //    worldCoordinates = Vector3.Transform(ray.Position, translationMatrix);
            //}
            //return worldCoordinates;
        }

        void CustomDestroy()
        {


        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
