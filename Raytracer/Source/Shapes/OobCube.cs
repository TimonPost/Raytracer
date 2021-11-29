using System;
using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    public struct OobCube : IRaytracable
    {
        public Vector3 BaseX { get; set; }
        public Vector3 BaseZ { get; set; }
        public int Height { get; set; }

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
        {
            throw new NotImplementedException();
        }
    }
}