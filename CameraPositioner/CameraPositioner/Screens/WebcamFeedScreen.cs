using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CameraPositioner.Screens
{
    public class WebcamFeedScreen : GameStateManagement.GameScreen
    {
        private IVideoCaptureService videoCapture;
        private Rectangle fullscreenRectangle;
        private Effect swapRBEffect;

        private SpriteBatch spriteBatch;

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                videoCapture = ScreenManager.Game.Services.GetService(typeof(IVideoCaptureService)) as IVideoCaptureService;
                if (null == videoCapture)
                    throw new Exception("IVideoCaptureService is not available");

                spriteBatch = ScreenManager.SpriteBatch;

                swapRBEffect = ScreenManager.Game.Content.Load<Effect>("Effects/SwapRB");
                fullscreenRectangle = new Rectangle(0, 0, videoCapture.Width, videoCapture.Height);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (null == videoCapture)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, swapRBEffect);
            spriteBatch.Draw(videoCapture.TextureRGBA, fullscreenRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 1f);
            spriteBatch.End();
        }
    }
}
