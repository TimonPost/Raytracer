using System;
using Microsoft.Xna.Framework;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public struct CheckerBoard : IMaterial
    {
        private readonly float _size;

        public CheckerBoard(float size)
        {
            _size = size;
        }

        public bool Scatter(CustomRay ray_in, HitRecord record, ref Vector3 attenuation, ref CustomRay scattered)
        {
            float sines = MathF.Sin(_size * record.P.X) * MathF.Sin(_size * record.P.Y) * MathF.Sin(_size * record.P.Z);

            if (sines < 0.0)
                attenuation = new Vector3(0.2f, 0.8f, 0.2f);
            else
                attenuation = new Vector3(1f, 1f, 1f);

            var target = record.P + record.Normal + Game1.RandomInUnitSphere();
            scattered = new CustomRay(record.P, target - record.P);
            return true;
        }
    }
}