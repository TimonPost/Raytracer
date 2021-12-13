using System;
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer.Source.Shapes
{
    public class Sphere : IRaytracable
    {
        public IMaterial Material { get; set; }

        public BoundingSphere BoundingSphere;

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            Material = material;
            BoundingSphere = new BoundingSphere(center, radius);
        }

        public Vector3 LeftFrontBottomCorner => BoundingSphere.Center - new Vector3(BoundingSphere.Radius);
        public Vector3 RightBackTopCorner => (BoundingSphere.Center + new Vector3(BoundingSphere.Radius));
        public Vector3 Dimensions => RightBackTopCorner - LeftFrontBottomCorner;

        public CubeBound Bounds => new CubeBound(LeftFrontBottomCorner.X, LeftFrontBottomCorner.Y,LeftFrontBottomCorner.Z, Dimensions.X, Dimensions.Y, Dimensions.Z);


        public bool Intersects(Ray ray, float tmin, float tmax, ref HitRecord record)
        {
            //var t = BoundingSphere.Intersects(ray);
            // var t = ray.Intersects(BoundingSphere);
            //
            // if (t != null)
            // {
            //     record.T = t.Value;
            //     record.P = ray.PointAt(record.T);
            //     record.Normal = (record.P - BoundingSphere.Center) / BoundingSphere.Radius;
            //     record.Material = Material;
            //     return true;
            // }
            //
            // return false;

            Vector3 oc = ray.Position - BoundingSphere.Center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - BoundingSphere.Radius * BoundingSphere.Radius;
            float discriminant = b * b - 4f * a * c;
            
            if (discriminant > 0.0f)
            {
                float t1 = (-b - MathF.Sqrt(discriminant)) / (2f * a);
            
                if (t1 < tmax && t1 > tmin)
                {
                    record.T = t1;
                    record.P = ray.PointAt(record.T);
                    record.Normal = (record.P - BoundingSphere.Center) / BoundingSphere.Radius;
                    record.Material = Material;
                    return true;
                }
            
                float t2 = (-b + MathF.Sqrt(discriminant)) / (2f * a);
            
                if (t2 < tmax && t2 > tmin)
                {
                    record.T = t2;
                    record.P = ray.PointAt(record.T);
                    record.Normal = (record.P - BoundingSphere.Center) / BoundingSphere.Radius;
                    record.Material = Material;
                    return true;
                }
            }
            
            return false;
        }
    }
}