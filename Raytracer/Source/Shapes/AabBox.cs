using System;
using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    //public struct AabBox: IRaytracable
    //{
    //    private readonly IMaterial _material;
    //    public Vector3 VMin { get; set; }
    //    public Vector3 VMax { get; set; }

    //    public Vector3 Center { get; }

    //    public AabBox(Vector3 vmin, Vector3 vMax, IMaterial material)
    //    {
    //        _material = material;
    //        VMin = vmin;
    //        VMax = vMax;
    //        Center = (VMin / 2f + VMax / 2f);
    //    }

    //    public bool Intersects(CustomRay ray, float a_, float b, ref HitRecord record)
    //    {
    //        // X
    //        float txmin = (VMin.X - ray.O.X) / ray.D.X;
    //        float txmax = (VMax.X - ray.O.X) / ray.D.X;

    //        if (txmax < txmin)
    //            (txmin, txmax) = (txmax, txmin);

    //        // Y
    //        float tymin = (VMin.Y - ray.O.Y) / ray.D.Y;
    //        float tymax = (VMax.Y - ray.O.Y) / ray.D.Y;

    //        if (tymax < tymin)
    //            (tymin, tymax) = (tymax, tymin);

    //        // Z
    //        float tzmin = (VMin.Z - ray.O.Z) / ray.D.Z;
    //        float tzmax = (VMax.Z - ray.O.Z) / ray.D.Z;

    //        if (tzmax < tzmin)
    //            (tzmin, tzmax) = (tzmax, tzmin);

    //        var tmin = (txmin > tymin) ? txmin : tymin;
    //        var tmax = (txmax < tymax) ? txmax : tymax;


    //        if (txmin > tymax || tymin > txmax) return false;
    //        if (tmin > tzmax || tzmin > tmax) return false;
    //        if (tzmin > tmin) tmin = tzmin;
    //        if (tzmax < tmax) tmax = tzmax;

    //        record.T = tmin;
    //        record.P = ray.PointAt(record.T);


    //        var p = record.P - Center;

    //        var dx = MathF.Abs(VMin.X - VMax.X) / 2;
    //        var dy = MathF.Abs(VMin.Y - VMax.Y) / 2;
    //        var dz = MathF.Abs(VMin.Z - VMax.Z) / 2;

    //        var bias = 1.00001f;
    //        record.Normal = new Vector3(p.X / dx * bias, p.Y / dy * bias, p.Z / dz * bias);

    //        if (Math.Round(MathF.Abs(record.Normal.X), 4) == 1f)
    //            record.Normal = new Vector3(record.Normal.X * 1f, 0f, 0f);
    //        if (Math.Round(MathF.Abs(record.Normal.Y), 4) == 1f)
    //            record.Normal = new Vector3(0f, record.Normal.Y * 1f, 0f);
    //        if (Math.Round(MathF.Abs(record.Normal.Z), 4) == 1f)
    //            record.Normal = new Vector3(0f, 0f, record.Normal.Z * 1f);

    //        record.Normal = record.Normal;

    //        record.Material = _material;// new Lambertian(new Vector3(0f, 0f, 1f));

    //        return true;
    //    }
    //}
}