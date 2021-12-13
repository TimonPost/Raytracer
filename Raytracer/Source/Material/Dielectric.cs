using System;
using Microsoft.Xna.Framework;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public struct Dielectric : IMaterial
    {
        private readonly float _ref_idx;

        public Dielectric(float refIdx)
        {
            _ref_idx = refIdx;
        }

        public bool Scatter(Ray ray_in, HitRecord record, ref Vector3 attenuation,
            ref Ray scattered)
        {
            Vector3 outwardNormal;
            Vector3 reflected = ((IMaterial)(this)).Reflect(ray_in.Direction, record.Normal);
            float niOverNt;

            attenuation = new Vector3(1f, 1f, 1f);
            Vector3 refracted = Vector3.Zero;
            float reflectProb;
            float cosine;

            if (Vector3.Dot(ray_in.Direction, record.Normal) > 0)
            {
                outwardNormal = -record.Normal;
                niOverNt = _ref_idx;
                cosine = _ref_idx * Vector3.Dot(ray_in.Direction, record.Normal) / ray_in.Direction.Length();
            }
            else
            {
                outwardNormal = record.Normal;
                niOverNt = 1f / _ref_idx;
                cosine = -Vector3.Dot(ray_in.Direction, record.Normal) / ray_in.Direction.Length();
            }

            if (((IMaterial)(this)).Refract(ray_in.Direction, outwardNormal, niOverNt, ref refracted))
            {
                reflectProb = ((IMaterial)(this)).Schlick(cosine, _ref_idx);
            }
            else
            {
                reflectProb = 1f;
            }

            var rand = new Random();
            if (rand.NextDouble() < reflectProb)
            {
                scattered = new Ray(record.P, reflected);
            }
            else
            {
                scattered = new Ray(record.P, refracted);
            }

            return true;

        }
    }
}