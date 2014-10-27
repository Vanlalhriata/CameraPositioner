using Microsoft.Xna.Framework;

namespace CameraPositioner.LocalComponents
{
    public interface IPositionerService
    {
        Vector3? Position { get; }
    }
}
