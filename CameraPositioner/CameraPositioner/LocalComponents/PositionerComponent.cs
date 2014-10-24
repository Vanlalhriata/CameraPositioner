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
    /// This component takes in a series of matrices freom the AR detection result and calculates
    /// the location of the camera. This is assuming the center of the detected marker is the origin
    /// </summary>
    public class PositionerComponent : Microsoft.Xna.Framework.GameComponent
    {

        #region Initialization

        public PositionerComponent(Game game)
            : base(game)
        {
            // TODO: Add to service
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        #endregion Initialization

        #region Loop

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        #endregion Loop
    }
}
