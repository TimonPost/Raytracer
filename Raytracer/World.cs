using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Raytracer.Raytracer;
using Raytracer.Source;
using Raytracer.Source.Lights;
using Raytracer.Source.Shapes;

namespace Raytracer
{
    public class World : IRaytracable
    {
        private readonly List<IRaytracable> _objects;
        public List<PointLight> PointLights { get; set; }

        public OctTree OctTree { get; set; }

        public World(List<IRaytracable> objects, List<PointLight> pointLights)
        {
            _objects = objects;
            PointLights = pointLights;
            OctTree = new OctTree(objects, Bounds);
            
            OctTree.UpdateTree();

            OctTree.PrintQuadTree(OctTree, 0);
        }

       
        public CubeBound Bounds => new CubeBound(-15, -15, -15, 30, 30, 30);

        public bool Intersects(Ray ray, float tmin, float tmax, ref HitRecord record)
        {
            var (intersected, hitRecord) = OctTree.Intersects(new Ray(ray.Position, ray.Direction));
            
            if (hitRecord.HasValue)
                record = hitRecord.Value;
            
            return intersected;
        }

        public bool IntersectedObjects(Ray ray, float tmin, float tmax, ref HitRecord record)
        {
            var hits = OctTree.IntersectedObjects(ray);
            
            var lowestT = float.MaxValue;
            var lowestRecord = new HitRecord();
            var intersected = false;

            if (hits == null)
                return false;

            foreach (var hit in hits)
            {
                if (hit.T < lowestT)
                {
                    lowestT = hit.T;
                    intersected = true;
                    lowestRecord = hit;
                }
            }

            record = lowestRecord;
            return intersected;
        }

        public bool HitsLight(Ray ray, Vector3 origin)
        {
            var record = new HitRecord();

            var intersects = false;

            foreach (var pointLight in PointLights)
            {
                var lightRay = new Ray(origin, pointLight.Position - origin);
                
                intersects = Intersects(lightRay, 0f, 1000f, ref record);
            }
        
            return !intersects;
        }
    }
}