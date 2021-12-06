using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Raytracer.Source.Shapes;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer.Source
{
    public struct CubeBound
    {
        public CubeBound(float x, float y, float z, float width, float height, float depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public Vector3 Min => new Vector3(X, Y, Z);
        public Vector3 Max => Min + new Vector3(Width, Height, Depth);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }

    }

    public class OctTree
    {
        private readonly int _level;
        private readonly CubeBound _bounds;

        private readonly OctTree[] _nodes;
        private readonly List<IRaytracable> _objects;

        public const int MaxObjects = 10;
        public const int MaxLevels = 3;

        public Vector3 MinimalCubeSize = Vector3.One;

        public bool Debug;
        public int DepthDebug;
        public Vector3 DebugColor;

        public OctTree(List<IRaytracable> objects, CubeBound bounds) : this(objects, 0, bounds)
        { }

        public OctTree(int level, CubeBound bounds) : this(new List<IRaytracable>(), level, bounds)
        {
        }

        public OctTree(List<IRaytracable> objects, int level, CubeBound bounds)
        {
            _level = level;
            _bounds = bounds;
            _nodes = new OctTree[8];
            _objects = objects;

            var rand = new Random();
            DebugColor = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            Debug = true;
            DepthDebug = 3;
        }

        public void Split()
        {
            float halfWidth = _bounds.Width / 2f;
            float halfHeight = _bounds.Height / 2f;
            float halfDepth = _bounds.Depth / 2f;

            if (halfWidth < MinimalCubeSize.X || halfHeight < MinimalCubeSize.Y || halfDepth < MinimalCubeSize.Z)
                return;

            float x = _bounds.X;
            float y = _bounds.Y;
            float z = _bounds.Z;

            int nextLevel = _level + 1;

            // bottom
            // 1
            _nodes[0] = new OctTree(nextLevel, new CubeBound(x, y, z, halfWidth, halfHeight, halfDepth));
            // 2
            _nodes[1] = new OctTree(nextLevel, new CubeBound(x + halfWidth, y, z, halfWidth, halfHeight, halfDepth));
            // 3
            _nodes[2] = new OctTree(nextLevel, new CubeBound(x, y, z + halfDepth, halfWidth, halfHeight, halfDepth));
            // 4
            _nodes[3] = new OctTree(nextLevel, new CubeBound(x+halfWidth, y, z + halfDepth, halfWidth, halfHeight, halfDepth));


            // top
            // 5
            _nodes[4] = new OctTree(nextLevel, new CubeBound(x, y+halfHeight, z, halfWidth, halfHeight, halfDepth));
            // 6
            _nodes[5] = new OctTree(nextLevel, new CubeBound(x + halfWidth, y + halfHeight, z, halfWidth, halfHeight, halfDepth));
            // 7
            _nodes[6] = new OctTree(nextLevel, new CubeBound(x, y + halfHeight, z + halfDepth, halfWidth, halfHeight, halfDepth));
            // 8
            _nodes[7] = new OctTree(nextLevel, new CubeBound(x + halfWidth, y + halfHeight, z + halfDepth, halfWidth, halfHeight, halfDepth));
        }

        public int _getIndex(CubeBound obj)
        {
            int index = -1;

            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);
            double depthMidpoint = _bounds.Z + (_bounds.Depth / 2);

            bool topQuadrant = (obj.Y < horizontalMidpoint && obj.Y + obj.Height < horizontalMidpoint);
            bool bottomQuadrant = (obj.Y > horizontalMidpoint);

            bool leftQuadrant = obj.X < verticalMidpoint && obj.X + obj.Width < verticalMidpoint;
            bool rightQuadrant = obj.X > verticalMidpoint;

            bool frontQuadrant = obj.Z < depthMidpoint && obj.Z + obj.Depth < depthMidpoint;
            bool backQuadrant = obj.Z > depthMidpoint;

            if (leftQuadrant)
            {
                if (topQuadrant)
                {
                    if (frontQuadrant)
                    {
                        // 5
                        index = 5;
                    } else if (backQuadrant)
                    {
                        // 7
                        index = 7;
                    }
                }
                else if (bottomQuadrant)
                {
                    if (frontQuadrant)
                    {
                        // 1
                        index = 1;
                    }
                    else if (backQuadrant)
                    {
                        // 3
                        index = 3;
                    }
                }
            } else  if (rightQuadrant)
            {
                if (topQuadrant)
                {
                    if (frontQuadrant)
                    {
                        // 6
                        index = 6;
                    }
                    else if (backQuadrant)
                    {
                        // 8
                        index = 8;
                    }
                }
                else if (bottomQuadrant)
                {
                    if (frontQuadrant)
                    {
                        // 2
                        index = 2;
                    }
                    else if (backQuadrant)
                    {
                        // 4
                        index = 4;
                    }
                }
            }

            return index;
        }
        


        public void Insert(IRaytracable rect)
        {
            if (!IsLeaveNode())
            {
                int index = _getIndex(rect.Bounds);

                if (index != -1)
                {
                    _nodes[index].Insert(rect);
                    return;
                }
            }

            _objects.Add(rect);

            if (_objects.Count > MaxObjects && _level < MaxLevels)
            {
                if (IsLeaveNode())
                {
                    Split();
                }

                int i = 0;
                while (i < _objects.Count)
                {
                    int index = _getIndex(_objects[i].Bounds);

                    if (index != -1)
                    {
                        var obj = _objects[i];
                        _objects.RemoveAt(i);
                        _nodes[index].Insert(obj);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public List<IRaytracable> Retrieve(List<IRaytracable> returnObjects, CubeBound rect)
        {
            int index = _getIndex(rect);
            
            if (index != -1 && _nodes[0] != null)
            {
                _nodes[index].Retrieve(returnObjects, rect);
            }

            returnObjects.AddRange(_objects);
            return returnObjects;
        }

        public bool DebugIntersects(Ray ray, ref HitRecord record)
        {
            var box = new BoundingBox(_bounds.Min, _bounds.Max);
            var t = box.Intersects(ray);

            if (t == null)
                return false;

            if (DepthDebug == _level)
            {
                if (t.Value < record.T)
                {
                    record.T = t.Value;

                    record.Material = new Lambertian(DebugColor);
                    record.P = new CustomRay(ray.Position, ray.Direction).PointAt(t.Value);
                    record.Normal = Vector3.One;
                    return true;
                }
            }

            if (!IsLeaveNode())
            {
                bool intersects = _nodes[0].DebugIntersects(ray, ref record);
                intersects |= _nodes[1].Intersects(ray, ref record);
                intersects |= _nodes[2].DebugIntersects(ray, ref record);
                intersects |= _nodes[3].DebugIntersects(ray, ref record);
                intersects |= _nodes[4].DebugIntersects(ray, ref record);
                intersects |= _nodes[5].DebugIntersects(ray, ref record);
                intersects |= _nodes[6].DebugIntersects(ray, ref record);
                intersects |= _nodes[7].DebugIntersects(ray, ref record);

                return intersects;
            }

            return false;
        }

        public bool Intersects(Ray ray, ref HitRecord record)
        {
            var box = new BoundingBox(_bounds.Min, _bounds.Max);
            var t = box.Intersects(ray);

            if (t == null)
                return false;

            if (DepthDebug == _level)
            {
                if (t.Value < record.T)
                {
                    record.T = t.Value;

                    record.Material = new Lambertian(DebugColor);
                    record.P = new CustomRay(ray.Position, ray.Direction).PointAt(t.Value);
                    record.Normal = Vector3.One;
                    return true;
                }
            }

            if (IsLeaveNode())
            {
                return _intersectsWithObjects(ray, ref record, _objects);
            }
            else
            {
                bool intersects = _intersectsWithObjects(ray, ref record, _objects);

                intersects |= _nodes[0].Intersects(ray, ref record);
                intersects |= _nodes[1].Intersects(ray, ref record);
                intersects |= _nodes[2].Intersects(ray, ref record);
                intersects |= _nodes[3].Intersects(ray, ref record);
                intersects |= _nodes[4].Intersects(ray, ref record);
                intersects |= _nodes[5].Intersects(ray, ref record);
                intersects |= _nodes[6].Intersects(ray, ref record);
                intersects |= _nodes[7].Intersects(ray, ref record);
                return intersects;
            }
        }

        private bool _intersectsWithObjects(Ray ray, ref HitRecord record, List<IRaytracable> objects)
        {
            bool intersects = false;
            record.T = float.MaxValue;
            HitRecord hitRecord = record;

            foreach (var obj in _objects)
            {
                var intersected = obj.Intersects(new CustomRay(ray.Position, ray.Direction), 0f, 1000f, ref hitRecord);

                if (intersected && hitRecord.T < record.T)
                {
                    record = hitRecord;
                    intersects = true;
                }
            }

            return intersects;
        }

        public bool IsRoot()
        {
            return _level == 0;
        }

        public bool IsLeaveNode()
        {
            return _nodes[0] == null;
        }
    }
}
