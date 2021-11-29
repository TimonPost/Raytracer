using System.Collections.Generic;

namespace Raytracer.Source.Shapes
{
    public class SphereList : IRaytracable {
        public List<Sphere> Spheres { get; set; }

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
        {
            float closestHit = tmax;
            bool hasIntersected = false;
            HitRecord tempRecord = new HitRecord();

            foreach (var entity in Spheres)
            {
                if (entity.Intersects(ray, tmin, closestHit, ref tempRecord))
                {
                    closestHit = tempRecord.T;
                    hasIntersected = true;
                    record = tempRecord;
                    record.Material = entity.Material;
                }
            }

            return hasIntersected;
        }
    }
}