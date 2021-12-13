using Microsoft.Xna.Framework;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public struct Metal : IMaterial
    {
        private readonly Vector3 _albedo;
        private readonly float _fuzz;

        public Metal(Vector3 albedo, float f)
        {
            _albedo = albedo;
            _fuzz = f;
        }

        public bool Scatter(Ray ray_in, HitRecord record, ref Vector3 attenuation, ref Ray scattered)
        {
            var reflected = ((IMaterial)(this)).Reflect(Vector3.Normalize(ray_in.Direction), record.Normal);
            scattered = new Ray(record.P, reflected+_fuzz*Game1.RandomInUnitSphere());
            attenuation = _albedo;
            return Vector3.Dot(scattered.Direction, record.Normal) > 0;
        }
    }
}