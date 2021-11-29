using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Raytracer.Source.Shapes
{
    public struct Plane : IRaytracable
    {
        public Vector3 P0 { get; set; }
        public Vector3 N { get; set; }

        public Plane(Vector3 p0, Vector3 n)
        {
            P0 = p0;
            N = n;
        }

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
        {
            float denom = Vector3.Dot(N, ray.D);

            if (denom > 1e-6)
            {
                var p0l0 = P0 - ray.O;
                record.T = Vector3.Dot(p0l0, N);
                record.Normal = N;
                record.P = ray.PointAt(record.T);
                record.Material = new Lambertian(new Vector3(1f, 0f, 0f));
                return record.T >= 0;
            }

            return false;
        }
    }

    public struct Triangle : IRaytracable
    {
        private static float EPSILON = 0.0000001f;

        private readonly Vector3[][] _piramidCoords;
        public IMaterial Material { get; set; }

        public Triangle(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 topCenter, IMaterial material)
        {
            _piramidCoords = new Vector3[4][];
            _piramidCoords[0] = new Vector3[3];
            _piramidCoords[0][0] =a;
            _piramidCoords[0][1] = b;
            _piramidCoords[0][2] = topCenter;

            _piramidCoords[1] = new Vector3[3];
            _piramidCoords[1][0] = b;
            _piramidCoords[1][1] = c;
            _piramidCoords[1][2] = topCenter;

            _piramidCoords[2] = new Vector3[3];
            _piramidCoords[2][0] = c;
            _piramidCoords[2][1] = d;
            _piramidCoords[2][2] = topCenter;
            
            _piramidCoords[3] = new Vector3[3];
            _piramidCoords[3][0] = d;
            _piramidCoords[3][1] = a;
            _piramidCoords[3][2] = topCenter;

            Material = material;
        }
        static object lockx = new object();

        public bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record)
        {
            
            lock (lockx)
            {
                var n = Vector3.Zero;
                float smallestT = float.MaxValue;

                for (int triangle = 0; triangle < _piramidCoords.Length; triangle++)
                {
                    Vector3 v0 = _piramidCoords[triangle][0];
                    Vector3 v1 = _piramidCoords[triangle][1];
                    Vector3 v2 = _piramidCoords[triangle][2];

                    float a, f, u, v;
                    Vector3 h = new Vector3();
                    Vector3 s = new Vector3();
                    Vector3 q = new Vector3();

                    Vector3 edge1 = v1 - v0;
                    Vector3 edge2 = v2 - v0;
                    h = Vector3.Cross(ray.D, edge2);
                    a = Vector3.Dot(edge1, h);

                    if (a > -EPSILON && a < EPSILON)
                    {
                        continue; // This ray is parallel to this triangle.
                    }

                    f = 1f / a;

                    s = ray.O - v0;
                    u = f * (Vector3.Dot(s, h));

                    if (u < 0.0 || u > 1.0)
                    {
                        continue;
                    }

                    q = Vector3.Cross(s, edge1);
                    v = f * Vector3.Dot(ray.D, q);

                    if (v < 0.0 || u + v > 1.0)
                    {
                        continue;
                    }

                    // At this stage we can compute t to find out where the intersection point is on the line.
                    float t = f * Vector3.Dot(edge2, q);
                    
                    if (t > EPSILON) // ray intersection
                    {
                        if (t < smallestT)
                        {
                            smallestT = t;

                            var v0v1 = v1 - v0;
                            var v0v2 = v2 - v0;
                            // no need to normalize
                            var N = Vector3.Cross(v0v1, v0v2); // N 

                            n = N;
                        }
                    }
                }

                if (smallestT < float.MaxValue)
                {
                    record.T = smallestT;
                    record.P = ray.PointAt(smallestT);
                    record.Normal = -n;
                    record.Material = Material;
                    return true;
                }
            }

            // This means that there is a line intersection but not a ray intersection.
            return false;
        }
        
    }
}

