using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raytracer.Raytracer;

namespace Raytracer
{
    class FrameBuffer
    {
        private Color[] Colors;

        public FrameBuffer()
        {
            Colors = new Color[Game1.Height * Game1.Width];
        }

        public void SetPixel(int x, int y, Color color)
        {
            var i = (y * Game1.Width) + x;
            Colors[i] = color;
        }
        
        public Color[] Pixels()
        {
            return Colors;
        }
    }
}