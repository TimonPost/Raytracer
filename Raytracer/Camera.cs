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
            Vfov = theta;
            Aspect = aspect;
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
        public float Vfov { get; }
        public float Aspect { get; }
        public Vector3 LowerLeft { get; }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeft + u*Horizontal+ v*Vertical - Origin);
        }
    }
}