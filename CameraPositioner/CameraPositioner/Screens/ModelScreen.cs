using CameraPositioner.LocalComponents;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CameraPositioner.Screens
{
    public class ModelScreen : GameStateManagement.GameScreen
    {

        #region Fields

        private IConfigurationService config;
        private IArResultService arResult;

        private string modelPath;
        private Model model;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private Vector3 specularColor;
        private float specularPower;

        private Matrix[] unskinnedBoneTransforms;
        private Matrix arTransform;
        private Matrix modelPreTransform;

        private bool isArDetected;

        #endregion Fields

        #region Initialization

        public ModelScreen()
        {
            throw new Exception("ModelScreen class must be initialized with arguments");
        }

        public ModelScreen(string param)
        {
            modelPath = param;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            if (instancePreserved)
                return;

            IsPopup = true;
            config = (IConfigurationService)ScreenManager.Game.Services.GetService(typeof(IConfigurationService));
            arResult = (IArResultService)ScreenManager.Game.Services.GetService(typeof(IArResultService));

            model = ScreenManager.Game.Content.Load<Model>(modelPath);

            CreateWorld();

            unskinnedBoneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(unskinnedBoneTransforms);

            arTransform = Matrix.Identity;
        }

        private void CreateWorld()
        {
            // First transform applied to the model
            modelPreTransform = Matrix.CreateScale(config.GetFloat("ModelPreTransform", "Scale"))
                                * Matrix.CreateRotationX(MathHelper.ToRadians(config.GetFloat("ModelPreTransform", "RotationX")))
                                * Matrix.CreateTranslation(config.GetFloat("ModelPreTransform", "TranslateX"),
                                                           config.GetFloat("ModelPreTransform", "TranslateY"),
                                                           config.GetFloat("ModelPreTransform", "TranslateZ"));

            // The AR component provides transforms assuming the camera is located at the origin and faces towards -z
            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);

            float fieldOfViewY = MathHelper.ToRadians(config.GetFloat("CameraSetup", "FieldOfViewY"));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfViewY,
                                ScreenManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 4000.0f);

            specularColor = new Vector3(0.25f);
            specularPower = 16;
        }

        #endregion Initialization

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (arResult.Transform != null)
            {
                arTransform = (Matrix)arResult.Transform;
                isArDetected = true;
            }
            else
                isArDetected = false;
        }

        public override void Draw(GameTime gameTime)
        {
            // Spritebatch sets GraphicsDevice to best draw textures. Need to reset
            // on every Draw call for 3D
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (isArDetected)
                DrawUnskinned(gameTime);
        }

        private void DrawUnskinned(GameTime gameTime)
        {
            if (null == unskinnedBoneTransforms)
                return;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.SpecularColor = specularColor;
                    effect.SpecularPower = specularPower;
                    effect.Alpha = TransitionAlpha;

                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.World =  unskinnedBoneTransforms[mesh.ParentBone.Index]
                                    * modelPreTransform
                                    * arTransform
                                    ;
                }

                mesh.Draw();
            }
        }

        #endregion Update and Draw
    }
}
