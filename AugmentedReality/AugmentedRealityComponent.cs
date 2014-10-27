using Interfaces;
using Microsoft.Xna.Framework;
using SLARToolKit;
using System;
using System.Collections.Generic;

namespace AugmentedReality
{
    /// <summary>
    /// This component exposes a list of detected markers as
    /// DetectionResults. Processing the result is deferred to other
    /// components
    /// </summary>
    public class AugmentedRealityComponent : GameComponent, IAugmentedRealityService
    {

        #region Fields

        private IVideoCaptureService videoCapture;
        private IConfigurationService config;

        private GrayBufferMarkerDetector arDetector;
        private bool isInitialized;
        private bool isDetecting;

        #endregion Fields

        #region IAugmentedRealityService implementation

        public DetectionResults DetectionResults { get; private set; }

        #endregion IAugmentedRealityService implementation

        #region Initialization

        public AugmentedRealityComponent(Game game)
            : base (game)
        {
            game.Services.AddService(typeof(IAugmentedRealityService), this);
        }

        public override void Initialize()
        {
            videoCapture = Game.Services.GetService(typeof(IVideoCaptureService)) as IVideoCaptureService;
            if (null == videoCapture)
                throw new Exception("IVideoCaptureService is not available");

            config = Game.Services.GetService(typeof(IConfigurationService)) as IConfigurationService;
            if (null == config)
                throw new Exception("IConfigurationService is not available");

            InitializeAR(videoCapture.GrayWidth, videoCapture.GrayHeight);

            videoCapture.NewFrame += videoCapture_NewFrame;

            base.Initialize();
        }

        private void InitializeAR(int captureWidth, int captureHeight)
        {
            arDetector = new GrayBufferMarkerDetector();

            // Load the marker pattern. It has 16x16 segments and a width of 80 millimeters
            var markers = new List<Marker>();
            string[] markerNames = config.GetStringArray("MarkerNames", "Names");
            foreach (string markerName in markerNames)
            {
                string markerPatternPath = config.GetString("MarkerPatternPaths", markerName);
                markers.Add(Marker.LoadFromResource(markerPatternPath, 16, 16, 80, markerName));
            }

            // The perspective projection has the near plane at 1 and the far plane at 4000
            arDetector.Initialize(captureWidth, captureHeight, 1, 4000, markers);

            arDetector.Threshold = 150;

            isInitialized = true;
        }

        #endregion Initialization

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (null != videoCapture)
                videoCapture.NewFrame -= videoCapture_NewFrame;

            base.Dispose(disposing);
        }

        #endregion Dispose

        #region Loop

        void videoCapture_NewFrame(object sender, EventArgs e)
        {
            Detect();
        }

        #endregion Loop

        #region AR detection

        private void Detect()
        {
            if (isDetecting || !isInitialized)
                return;

            if (null == videoCapture)
                return;

            isDetecting = true;

            try
            {
                byte[] buffer = null;
                
                if (config.GetBool("CameraSetup", "Mirror"))
                    buffer = videoCapture.BufferGray;   // the gray buffer is mirrored by default
                else
                    buffer = Utils.FlipHorizontalGrayscale(videoCapture.BufferGray, videoCapture.GrayWidth, videoCapture.GrayHeight);

                //Detect the markers
                var results = arDetector.DetectAllMarkers(buffer, videoCapture.GrayWidth, videoCapture.GrayHeight);
                DetectionResults = results;
            }
            finally
            {
                isDetecting = false;
            }
        }

        #endregion AR detection
    }
}
