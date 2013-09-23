using Microsoft.Xna.Framework;

namespace FRBTouch
{
    public interface ICoordinateTranslator
    {
        Vector2 TranslateCoordinates(Vector2 position);
    }
}