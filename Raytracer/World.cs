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

            //OctTree.PrintQuadTree(OctTree, 0);
        }

       
        public CubeBound Bounds => new CubeBound(-30, -30, -30, 60, 60, 60);

        public bool Intersects(Ray ray, float tmin, float tmax, ref HitRecord record)
        {
            // bool intersects = false;
            //
            // record.T = float.MaxValue;
            // HitRecord hitRecord = record;
            //
            // foreach (var plane in _objects)
            // {
            //     if (plane.Intersects(ray, tmin, tmax, ref hitRecord))
            //     {
            //         if (hitRecord.T < record.T)
            //         {
            //             intersects = true;
            //             record = hitRecord;
            //         }
            //     }
            // }
            //
            // return intersects;

            var hits = OctTree.Intersects(new Ray(ray.Position, ray.Direction));
            
            var lowestT = float.MaxValue;
            var lowestRecord = new HitRecord();
            var intersected = false;
            
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

        // public bool HitsLight(CustomRay ray, Vector3 origin)
        // {
        //     var record = new HitRecord();
        //
        //     foreach (var pointLight in PointLights)
        //     {
        //         var lightRay = new CustomRay(origin, pointLight.Position - origin);
        //
        //         var nearestObject = float.MaxValue;
        //
        //         if (Intersects(lightRay, 0f, 1000f, ref record))
        //         {
        //             nearestObject = record.T;
        //         }
        //
        //         var intersection = new Sphere(pointLight.Position, 0.9f, null);
        //         
        //         if (intersection.Intersects(lightRay, 0f, 1000f, ref record))
        //         {
        //             return !(nearestObject < record.T);
        //         }
        //     }
        //
        //     return false;
        // }
    }
}