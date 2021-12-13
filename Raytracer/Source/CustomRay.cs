using System.Numerics;
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer
{
    public static class RayExtensions
    {
        public static Vector3 PointAt(this Ray ray, float t)
        {
            return ray.Position + ray.Direction * t;
        }
    }
}