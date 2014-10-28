using Microsoft.Xna.Framework;


namespace CameraPositioner.LocalComponents
{
    /// <summary>
    /// This component takes in a series of matrices from the AR detection result and calculates
    /// the location of the camera. This is assuming the center of the detected marker is the origin.
    /// The results are manipulated to avoid wild fluctations and thus may have a slight delay.
    /// </summary>
    public class PositionerComponent : GameComponent, IPositionerService
    {

        #region Fields

        private IArResultService arResult;
        private float lerpAmount;

        #endregion Fields

        #region Properties

        public Vector3? Position { get; private set; }

        #endregion Properties

        #region Initialization

        public PositionerComponent(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPositionerService), this);
        }

        public override void Initialize()
        {
            arResult = (IArResultService)Game.Services.GetService(typeof(IArResultService));
            lerpAmount = 0.01f;

            base.Initialize();
        }

        #endregion Initialization

        #region Loop

        public override void Update(GameTime gameTime)
        {

            if (null == arResult.Transform)
                Position = null;
            else
                Position = LerpPosition(Position, (Matrix)arResult.Transform);

            base.Update(gameTime);
        }

        #endregion Loop

        #region Helper methods

        private Vector3? LerpPosition(Vector3? oldPosition, Matrix transform)
        {
            Vector3 newPosition = GetPosition(transform);

            // Return null for invalid newPosition
            if (float.IsNaN(newPosition.X) || float.IsNaN(newPosition.Y) || float.IsNaN(newPosition.Z))
                return null;
            else if (null == oldPosition)
                return newPosition;
            else
                return Vector3.Lerp((Vector3)oldPosition, newPosition, lerpAmount);
        }

        private static Vector3 GetPosition(Matrix m)
        {
            // The AR transform is Rotation * Translation.
            // Inverse of this is TransformInverse * RotationInverse.
            // This effectively moves Vector3.Zero to the camera position in a coordinate system
            // where the center of the AR marker is the origin and the marker lies along the xy plane (think wall)

            Vector3 result = Vector3.Zero;
            result = Vector3.Transform(Vector3.Zero, Matrix.Invert(m));

            // However, for practical use, the marker will lie along the floor (xz plane) with its normal pointing up (y),
            // and the bottom towards +ve z. This means z is actually y and y is actually -z; or rotateX(90 degrees).
            // Note that the default config does this rotateX(90 degrees) while drawing the axes model

            result = new Vector3(result.X, result.Z, -result.Y);

            return result;
        }

        #endregion Helper methods
    }
}
