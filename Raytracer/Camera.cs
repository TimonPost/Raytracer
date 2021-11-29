using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer.Raytracer
{
    public class Camera
    {
        public Camera(Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float vfov, float aspect)
        {
            float theta = vfov * MathF.PI / 180;
            float halfHeight = MathF.Tan(theta / 2f);
            float halfWidth = aspect * halfHeight;

            Origin = lookFrom;
            Vector3 w = Vector3.Normalize(lookFrom - lookAt);
            Vector3 u = Vector3.Normalize(Vector3.Cross(vup, w));
            Vector3 v = Vector3.Cross(w, u);

            //LowerLeft = new Vector3(-halfWidth, -halfHeight, -1f);
            LowerLeft = Origin - halfWidth * u - halfHeight * v - w;

            Horizontal = 2f * halfWidth * u;
            Vertical = 2f * halfHeight * v;
        }

        public Vector3 Vertical { get;  }
        public Vector3 Horizontal { get; }
        public Vector3 Origin { get; }
        public Vector3 LowerLeft { get; }

        public CustomRay GetRay(float u, float v)
        {
            return new CustomRay(Origin, LowerLeft + u*Horizontal+ v*Vertical - Origin);
        }

        // public int R(int width, int height)
        // {
        //     return width / height;
        // }
        //
        // public Vector3 RasterSpace(int width, int height)
        // {
        //     var r = R(width, height);
        //
        // }
        //
        // public Point NormalisedCoord(Point p)
        // {
        //     var r = R(Game1.Width, Game1.Height);
        //
        //     var nx = (p.X / Game1.Width) * r;
        //     var ny = (p.Y / Game1.Height) * r;
        //
        //     return new Point(nx, ny);
        // }
        //
        //
        // public Point ScreenSpaceCoord(Point p)
        // {
        //     var normalized = NormalisedCoord(p);
        //     var r = R(Game1.Width, Game1.Height);
        //
        //     var sx = (2* normalized.X) - r;
        //     var sy = -(2*p.Y - 1);
        //
        //     return new Point(nx, ny);
        // }
    }
}