using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraPositioner.LocalComponents
{
    public interface IArResultService
    {
        Matrix? Transform { get; }
    }
}
