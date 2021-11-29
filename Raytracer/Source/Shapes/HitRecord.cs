using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    public struct HitRecord
    {
        public Vector3 Normal { get; set; }
        public Vector3 P { get; set; }
        public float T { get; set; }
        public IMaterial Material { get; set; }
    }
}