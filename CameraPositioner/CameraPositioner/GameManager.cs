using AugmentedReality;
using CameraPositioner.LocalComponents;
using CameraPositioner.Screens;
using DirectShowVideoToXna;
using EasyConfig;
using GameStateManagement;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CameraPositioner
{
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ScreenManager screenManager;
        ConfigurationManager config;

        #endregion Fields

        #region Initialization

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            LoadConfigFile();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;

            AddCustomComponents();
        }

        private void LoadConfigFile()
        {
            ConfigFile configFile = new ConfigFile("Config.ini");
            config = new ConfigurationManager(configFile);
        }

        private void AddCustomComponents()
        {
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            Components.Add(new VideoCaptureComponent(this));
            Components.Add(new AugmentedRealityComponent(this));
            Components.Add(new ArResultComponent(this));
            Components.Add(new PositionerComponent(this));

            Services.AddService(typeof(IConfigurationService), config);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new WebcamFeedScreen(), null);
            screenManager.AddScreen(new InfoScreen(), null);
            screenManager.AddScreen(new ModelScreen("Models/Axes"), null);
        }

        #endregion Initialization

        #region Dispose

        protected override void UnloadContent()
        {
            screenManager.Dispose();
        }

        #endregion Dispose
    }
}
