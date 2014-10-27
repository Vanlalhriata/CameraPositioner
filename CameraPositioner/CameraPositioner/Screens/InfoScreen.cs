using CameraPositioner.LocalComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CameraPositioner.Screens
{
    public class InfoScreen : GameStateManagement.GameScreen
    {

        #region Fields

        private SpriteBatch spriteBatch;

        private IPositionerService cameraPositioner;
        private KeyboardState newKeyState;
        private KeyboardState oldKeyState;

        private bool isRecordData;

        private string dataText;
        private string countText;

        private Vector3 meanPosition;
        private int recordCount;

        #endregion Fields

        #region Initialization

        public override void Activate(bool instancePreserved)
        {
            if (instancePreserved)
                return;

            IsPopup = true;

            spriteBatch = ScreenManager.SpriteBatch;
            cameraPositioner = (IPositionerService)ScreenManager.Game.Services.GetService(typeof(IPositionerService));
            oldKeyState = new KeyboardState();
            dataText = "Hold the spacebar to record";
            countText = "";
            
        }

        #endregion Initialization

        #region Loop

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Read the keyboard and set actions to be performed
            newKeyState = Keyboard.GetState();
            if (oldKeyState.IsKeyUp(Keys.Space) && newKeyState.IsKeyDown(Keys.Space))
            {
                // Space was pressed
                isRecordData = true;
                dataText = "Recording data...";
                countText = "";
                meanPosition = Vector3.Zero;

            }
            else if (oldKeyState.IsKeyDown(Keys.Space) && newKeyState.IsKeyUp(Keys.Space))
            {
                // Space was released
                isRecordData = false;
                dataText = String.Format("Camera position: {0:0.00}, {1:0.00}, {2:0.00}", meanPosition.X, meanPosition.Y, meanPosition.Z);
                countText = "Number of instances recorded: " + recordCount;
                ResetData();
            }
            oldKeyState = newKeyState;

            if (isRecordData)
                RecordData();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, dataText, new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(ScreenManager.Font, countText, new Vector2(10, 50), Color.Red);
            spriteBatch.End();
        }

        #endregion Loop

        #region Methods

        private void RecordData()
        {
            if (null == cameraPositioner.Position)
                return;

            recordCount++;

            // Accumulate the average with equal weights for every instance
            meanPosition = ((meanPosition * (recordCount - 1)) + (Vector3)cameraPositioner.Position) / recordCount;
        }

        private void ResetData()
        {
            recordCount = 0;
        }

        #endregion Methods
    }
}
