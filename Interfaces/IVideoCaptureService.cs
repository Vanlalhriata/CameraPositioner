using Microsoft.Xna.Framework.Graphics;
using System;

namespace Interfaces
{
    public interface IVideoCaptureService
    {
        Texture2D TextureRGBA { get; }
        byte[] BufferGray { get; }

        int Width { get; }
        int Height { get; }

        int GrayWidth { get; }
        int GrayHeight { get; }

        event EventHandler NewFrame;
    }
}
