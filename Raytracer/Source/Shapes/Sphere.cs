using System;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer.Source.Shapes
{
    public class Sphere : IRaytracable
    {
        public float Radius;
        public Vector3 Center;
        public IMaterial Material { get; set; }

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record) 
        {
            Vector3 oc = ray.Origin() - Center;
            float a = Vector3.Dot(ray.Direction(), ray.Direction());
            float b = 2f* Vector3.Dot(oc, ray.Direction());
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = b * b - 4f * a * c;
            
            if (discriminant > 0.0f)
            {
                float t1 = (-b - MathF.Sqrt(discriminant)) / (2f*a);

                if (t1 < tmax && t1 > tmin)
                {
                    record.T = t1;
                    record.P = ray.PointAt(record.T);
                    record.Normal = (record.P - Center) / Radius;
                    return true;
                }

                float t2 = (-b + MathF.Sqrt(discriminant)) / (2f * a);

                if (t2 < tmax && t2 > tmin)
                {
                    record.T = t2;
                    record.P = ray.PointAt(record.T);
                    record.Normal = (record.P - Center) / Radius;
                    return true;
                }
            }

            return false;
        }
    }
}