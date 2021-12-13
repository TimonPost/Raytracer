using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Raytracer.Raytracer;
using Raytracer.Source.Shapes;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer
{
    public interface IMaterial
    {
        public bool Scatter(Ray rayIn, HitRecord hitRecords, ref Vector3 attenuation, ref Ray scattered);

        public Vector3 Reflect(Vector3 v, Vector3 n)
        {
            return v - 2 * Vector3.Dot(v, n) * n;
        }

        public bool Refract(Vector3 v, Vector3 n, float niOverNt, ref Vector3 refracted)
        {
            Vector3 uv = Vector3.Normalize(v);
            float dt = Vector3.Dot(uv, n);
            float discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);

            if (discriminant > 0)
            {
                refracted = niOverNt * (uv - n * dt) - n * MathF.Sqrt(discriminant);
                return true;
            }

            return false;
        }

        public float Schlick(float cosine, float ref_idx)
        {
            float r0 = (1f - ref_idx) / (1f + ref_idx);
            r0 = r0 * r0;
            return r0 + (1f - r0) * MathF.Pow(1f - cosine, 5f);
        }
    }
}