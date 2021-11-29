using System;
using System.Collections.Generic;
using Raytracer.Raytracer;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public class World : IRaytracable
    {
        public SphereList Spheres = new SphereList();
        public List<AabBox> Cubes { get; set; }
        public List<Plane> Planes { get; set; }
        public List<Triangle> Triangle { get; set; }
        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
        {
            bool intersects = false;

            record.T = float.MaxValue;
            HitRecord hitRecord = record;

            foreach (var plane in Planes)
            {
                if (plane.Intersects(ray, tmin, tmax, ref hitRecord))
                {
                    if (hitRecord.T < record.T)
                    {
                        intersects = true;
                        record = hitRecord;
                    }
                }
            }


            foreach (var plane in Triangle)
            {
                if (plane.Intersects(ray, tmin, tmax, ref hitRecord))
                {
                    if (hitRecord.T < record.T)
                    {
                        intersects = true;
                        record = hitRecord;
                    }
                }
            }

            foreach (var aabBox in Cubes)
            {
                if (aabBox.Intersects(ray, tmin, tmax, ref hitRecord))
                {
                    if (hitRecord.T < record.T)
                    {
                        intersects = true;
                        record = hitRecord;
                    }
                }
            }

            if (Spheres.Intersects(ray, tmin, tmax, ref hitRecord))
            {
                if (hitRecord.T < record.T)
                {
                    intersects = true;
                    record = hitRecord;
                }
            }

        

            return intersects;
        }
    }
}