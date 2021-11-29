using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Raytracer.Source.Lights
{
    class DirectionalLight
    {
        public DirectionalLight(Vector3 direction, Vector3 color)
        {
            Direction = direction;
            Color = color;
        }
        
        public Vector3 Direction { get; set; }
        public Vector3 Color { get; set; }
    }
}
