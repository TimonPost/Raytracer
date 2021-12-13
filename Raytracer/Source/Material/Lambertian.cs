﻿using Microsoft.Xna.Framework;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public struct Lambertian : IMaterial
    {
        private readonly Vector3 _albedo;

        public Lambertian(Vector3 albedo)
        {
            _albedo = albedo;
        }

        public bool Scatter(Ray ray_in, HitRecord record, ref Vector3 attenuation, ref Ray scattered)
        {
            var target = record.P + record.Normal + Game1.RandomInUnitSphere();
            scattered = new Ray(record.P, target - record.P);
            attenuation = _albedo;
            return true;
        }
    }
}