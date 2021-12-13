using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    public interface IRaytracable
    {
        public CubeBound Bounds { get; }


        bool Intersects(Ray ray, float tmin, float tmax, ref HitRecord record);
    }
}