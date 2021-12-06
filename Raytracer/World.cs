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
          
            PrintQuadTree(OctTree,0);
        }

        void PrintQuadTree(OctTree tree, int quad)
        {
            if (tree == null)
            {
                return;
            }
            Debug.WriteLine("");

            for (int i = 0; i < tree._level; i++)
            {
                Debug.Write("\t");
            }
            Debug.Write(String.Format("[ {0} Level {1}; Objects: {2} min: {3} max: {4}", quad, tree._level, tree._objects.Count, tree._bounds.Min, tree._bounds.Max));

            for (int i = 0; i < tree._nodes.Length; i++)
            {
                PrintQuadTree(tree._nodes[i], i);
            }

            Debug.Write(String.Format(" ]"));
        }

        public CubeBound Bounds => new CubeBound(0, 0, 0, 10, 10, 10);

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
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


            record.T = float.MaxValue;
            return OctTree.Intersects(new Ray(ray.O, ray.D), ref record);
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