using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace CameraPositioner.LocalComponents
{
    /// <summary>
    /// This component takes in a series of matrices from the AR detection result and calculates
    /// the location of the camera. This is assuming the center of the detected marker is the origin.
    /// The results are manipulated to avoid wild fluctations and thus may have a slight delay.
    /// </summary>
    public class PositionerComponent : GameComponent
    {

        #region Fields

        private IArResultService arResult;
        private float lerpAmount;

        #endregion Fields

        #region Properties

        public Vector3? Position;

        #endregion Properties

        #region Initialization

        public PositionerComponent(Game game)
            : base(game)
        {
            // TODO: Add to service
        }

        public override void Initialize()
        {
            arResult = (IArResultService)Game.Services.GetService(typeof(IArResultService));
            lerpAmount = 0.1f;

            base.Initialize();
        }

        #endregion Initialization

        #region Loop

        public override void Update(GameTime gameTime)
        {

            if (null == arResult.Transform)
                Position = null;
            else if (null == Position)
                Position = GetPosition((Matrix)arResult.Transform);
            else
                Position = Vector3.Lerp((Vector3)Position, GetPosition((Matrix)arResult.Transform), lerpAmount);

            base.Update(gameTime);
        }

        #endregion Loop

        #region Helper methods

        private static Vector3 GetPosition(Matrix m)
        {
            // The AR transform is Rotation * Translation.
            // Inverse of this is TransformInverse * RotationInverse.
            // This effectively moves Vector3.Zero to the camera position in a coordinate system
            // where the center of the AR marker is the origin and the marker lies along the xy plane (think wall)

            Vector3 result = Vector3.Zero;
            result = Vector3.Transform(Vector3.Zero, Matrix.Invert(m));

            // However, for practical use, the marker will lie along the floor (xz plane) with its normal pointing up,
            // and the bottom towards +ve z. This means z is actually y and y is actually -z

            // TODO: The video feed is mirrored from previous implementations.
            // Unmirroring it will reverse x here and the marker pattern files

            result = new Vector3(-result.X, result.Z, -result.Y);

            return result;
        }

        #endregion Helper methods
    }
}
