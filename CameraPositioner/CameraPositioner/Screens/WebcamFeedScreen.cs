using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CameraPositioner.Screens
{
    public class WebcamFeedScreen : GameStateManagement.GameScreen
    {
        private IVideoCaptureService videoCapture;
        private IConfigurationService config;

        private Rectangle fullscreenRectangle;
        private Effect swapRBEffect;

        private SpriteEffects spriteEffects;

        private SpriteBatch spriteBatch;

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                videoCapture = ScreenManager.Game.Services.GetService(typeof(IVideoCaptureService)) as IVideoCaptureService;
                if (null == videoCapture)
                    throw new Exception("IVideoCaptureService is not available");

                config = ScreenManager.Game.Services.GetService(typeof(IConfigurationService)) as IConfigurationService;
                if (null == config)
                    throw new Exception("IConfigurationService is not available");

                spriteBatch = ScreenManager.SpriteBatch;

                swapRBEffect = ScreenManager.Game.Content.Load<Effect>("Effects/SwapRB");
                fullscreenRectangle = new Rectangle(0, 0, videoCapture.Width, videoCapture.Height);

                if (config.GetBool("CameraSetup", "Mirror"))
                    spriteEffects = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
                else
                    spriteEffects = SpriteEffects.FlipVertically;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (null == videoCapture)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, swapRBEffect);
            spriteBatch.Draw(videoCapture.TextureRGBA, fullscreenRectangle, null, Color.White, 0f, Vector2.Zero, spriteEffects, 1f);
            spriteBatch.End();
        }
    }
}
