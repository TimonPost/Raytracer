using System;
using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    public struct AabBox : IRaytracable
    {
        private readonly IMaterial _material;
        
        public Vector3 Center { get; }
        public BoundingBox BoundingBox;

        public AabBox(Vector3 vmin, Vector3 vMax, IMaterial material)
        {
            _material = material;
            BoundingBox = new BoundingBox(vmin, vMax);
            Center = BoundingBox.Min / 2f + BoundingBox.Max/ 2f;
            Size = vMax - vmin;
        }

        public Vector3 Size { get; }

        public CubeBound Bounds =>
            new CubeBound(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z, Size.X, Size.Y, Size.Z);

        public bool Intersects(Ray ray, float a_, float b, ref HitRecord record)
        {
            var t = BoundingBox.Intersects(ray);

            if (!t.HasValue)
                return false;

            record.T = t.Value;
            record.P = ray.PointAt(record.T);
            
            var p = record.P - Center;

            var dx = MathF.Abs(BoundingBox.Min.X - BoundingBox.Max.X) / 2;
            var dy = MathF.Abs(BoundingBox.Min.Y - BoundingBox.Max.Y) / 2;
            var dz = MathF.Abs(BoundingBox.Min.Z - BoundingBox.Max.Z) / 2;

            var bias = 1.00001f;
            record.Normal = new Vector3(p.X / dx * bias, p.Y / dy * bias, p.Z / dz * bias);

            if (Math.Round(MathF.Abs(record.Normal.X), 4) == 1f)
                record.Normal = new Vector3(record.Normal.X * 1f, 0f, 0f);
            if (Math.Round(MathF.Abs(record.Normal.Y), 4) == 1f)
                record.Normal = new Vector3(0f, record.Normal.Y * 1f, 0f);
            if (Math.Round(MathF.Abs(record.Normal.Z), 4) == 1f)
                record.Normal = new Vector3(0f, 0f, record.Normal.Z * 1f);
            
            record.Material = _material;// new Lambertian(new Vector3(0f, 0f, 1f));

            return true;
        }
    }
}