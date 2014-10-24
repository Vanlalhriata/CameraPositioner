using AugmentedReality;
using Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace CameraPositioner.LocalComponents
{
    /// <summary>
    /// This class will expose the most confident AR detection result's Transformation Matrix
    /// </summary>
    public class ArResultComponent : GameComponent, IArResultService
    {

        #region Fields

        private IAugmentedRealityService augmentedReality;
        private IConfigurationService config;
        private double confidenceThreshold;    // [0,1]

        #endregion Fields

        #region Properties

        public Matrix? Transform { get; private set; }

        #endregion Properties

        #region Initialization

        public ArResultComponent(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IArResultService), this);
        }

        public override void Initialize()
        {
            augmentedReality = Game.Services.GetService(typeof(IAugmentedRealityService)) as IAugmentedRealityService;
            if (null == augmentedReality)
                throw new Exception("IAugmentedRealityService is not available");

            config = Game.Services.GetService(typeof(IConfigurationService)) as IConfigurationService;
            if (null == config)
                throw new Exception("IConfigurationService is not available");

            confidenceThreshold = config.GetFloat("AugmentedReality", "ConfidenceThreshold");

            base.Initialize();
        }

        #endregion Initialization

        #region Loop

        public override void Update(GameTime gameTime)
        {
            if (null == augmentedReality || null == augmentedReality.DetectionResults)
                return;

            var filteredResults = augmentedReality.DetectionResults.Where(r => r.Confidence > confidenceThreshold).ToList();

            if (filteredResults.Count > 0)
                Transform = Utils.ToXnaMatrix(filteredResults.OrderByDescending(r => r.Confidence).First().Transformation);
            else
                Transform = null;

            base.Update(gameTime);
        }

        #endregion Loop
    }
}
