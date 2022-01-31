using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
            Min = new Vector3(X, Y, Z);
            Max = Min + new Vector3(Width, Height, Depth);
            Bounds = new BoundingBox(Min, Max);
        }

        public BoundingBox Bounds { get; }
        public Vector3 Min { get; }
        public Vector3 Max { get; } 

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
        public float Depth { get; set; }

        public Vector3 Dimensions => Max - Min;
    }

    public class OctTree
    {
        private readonly int _level;
        private readonly CubeBound _bounds;

        private readonly OctTree[] _nodes;
        private readonly List<IRaytracable> _objects;
        
        private readonly Queue<IRaytracable> _pendingInsertions;
        private bool _isTreeBuild;
        private bool _isTreeReady;

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
            _objects = new List<IRaytracable>();

            var rand = new Random();
            DebugColor = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            Debug = true;
            DepthDebug = 4;
            _pendingInsertions = new Queue<IRaytracable>(objects);
            _isTreeBuild = false;
            _isTreeReady = false;
        }

        public CubeBound[] CalculateOctBounds()
        {
            float halfWidth = _bounds.Width / 2f;
            float halfHeight = _bounds.Height / 2f;
            float halfDepth = _bounds.Depth / 2f;

            if (halfWidth < MinimalCubeSize.X || halfHeight < MinimalCubeSize.Y || halfDepth < MinimalCubeSize.Z)
                return new CubeBound[]{};

            float x = _bounds.X;
            float y = _bounds.Y;
            float z = _bounds.Z;
            
            var octant = new CubeBound[8];

            // bottom
            // 1
            octant[0] = new CubeBound(x, y, z, halfWidth, halfHeight, halfDepth);
            // 2
            octant[1] = new CubeBound(x + halfWidth, y, z, halfWidth, halfHeight, halfDepth);
            // 3
            octant[2] = new CubeBound(x, y, z + halfDepth, halfWidth, halfHeight, halfDepth);
            // 4
            octant[3] = new CubeBound(x+halfWidth, y, z + halfDepth, halfWidth, halfHeight, halfDepth);


            // top
            // 5
            octant[4] = new CubeBound(x, y+halfHeight, z, halfWidth, halfHeight, halfDepth);
            // 6
            octant[5] = new CubeBound(x + halfWidth, y + halfHeight, z, halfWidth, halfHeight, halfDepth);
            // 7
            octant[6] = new CubeBound(x, y + halfHeight, z + halfDepth, halfWidth, halfHeight, halfDepth);
            // 8
            octant[7] = new CubeBound(x + halfWidth, y + halfHeight, z + halfDepth, halfWidth, halfHeight, halfDepth);

            return octant;
        }

        private int _getIndex(CubeBound obj)
        {
            int index = -1;

            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);
            double depthMidpoint = _bounds.Z + (_bounds.Depth / 2);

            bool bottomQuadrant = (obj.Y < horizontalMidpoint && obj.Y + obj.Height < horizontalMidpoint);
            bool topQuadrant = (obj.Y >= horizontalMidpoint);

            bool leftQuadrant = obj.X < verticalMidpoint && obj.X + obj.Width < verticalMidpoint;
            bool rightQuadrant = obj.X >= verticalMidpoint;

            bool frontQuadrant = obj.Z < depthMidpoint && obj.Z + obj.Depth < depthMidpoint;
            bool backQuadrant = obj.Z >= depthMidpoint;

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



        public void LazyInsert(IRaytracable obj)
        {
            _pendingInsertions.Enqueue(obj);
        }

        public void UpdateTree()
        {
            if (!_isTreeBuild)
            {
                while (_pendingInsertions.TryDequeue(out var obj))
                {
                    _objects.Add(obj);
                }
                BuildTree();
            }
            else
            {
                while (_pendingInsertions.TryDequeue(out var obj))
                {
                    //Insert(obj);
                }
            }

            _isTreeReady = true;
        }

        private void BuildTree()
        {
            if (!HasObjects)
                return;
            
            if (!_isValidBounce(_bounds.Dimensions))
            {
                return;
            }

            var octant = CalculateOctBounds();

            if (octant.Length == 0)
                return;

            List<IRaytracable>[] octantObjects = new List<IRaytracable>[8];
            for (int i = 0; i < 8; i++)
                octantObjects[i] = new List<IRaytracable>();

            List<IRaytracable> delist = new List<IRaytracable>();

            foreach (var obj in _objects)
            {
                var index = _getIndex(obj.Bounds);

                if (index != -1)
                {
                    octantObjects[index-1].Add(obj);
                    delist.Add(obj);
                }
            }

            foreach (var obj in delist)
                _objects.Remove(obj);

            //Create child nodes where there are items contained in the bounding region
            for (int a = 0; a < 8; a++)
            {
                _nodes[a] = new OctTree(octantObjects[a], _level + 1, octant[a]);
                _nodes[a].UpdateTree();
            }

            _isTreeBuild = true;
            _isTreeReady = true;
        }

        private bool _isValidBounce(Vector3 bounds)
        {
            if (bounds.X <= MinimalCubeSize.X && bounds.Y <= MinimalCubeSize.Y && bounds.Z <= MinimalCubeSize.Z)
            {
                return false;
            }

            return true;
        }

        public bool Insert(IRaytracable rect)
        {
            if (IsLeaveNode())
            {
                _objects.Add(rect);
                return true;
            }

            if (!_isValidBounce(_bounds.Dimensions))
            {
                _objects.Add(rect);
                return true;
            }

            var nodeIndex = _getIndex(rect.Bounds);

            if (nodeIndex == -1)
            {
                _objects.Add(rect);
                return true;
            } if (nodeIndex >= 1)
            {
                return _nodes[nodeIndex - 1].Insert(rect);
            }

            throw new Exception("Could not insert item into the tree");
        }
        //
        // public List<IRaytracable> Retrieve(List<IRaytracable> returnObjects, CubeBound rect)
        // {
        //     int index = _getIndex(rect);
        //     
        //     if (index != -1 && _nodes[0] != null)
        //     {
        //         _nodes[index].Retrieve(returnObjects, rect);
        //     }
        //
        //     returnObjects.AddRange(_objects);
        //     return returnObjects;
        // }
        //
        // public bool DebugIntersects(Ray ray, ref HitRecord record)
        // {
        //     var box = new BoundingBox(_bounds.Min, _bounds.Max);
        //     var t = box.Intersects(ray);
        //
        //     if (t == null)
        //         return false;
        //
        //     if (DepthDebug == _level)
        //     {
        //         if (t.Value < record.T)
        //         {
        //             record.T = t.Value;
        //
        //             record.Material = new Lambertian(DebugColor);
        //             record.P = new CustomRay(ray.Position, ray.Direction).PointAt(t.Value);
        //             record.Normal = Vector3.One;
        //             return true;
        //         }
        //     }
        //
        //     if (!IsLeaveNode())
        //     {
        //         bool intersects = _nodes[0].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[1].Intersects(ray, ref record);
        //         intersects |= _nodes[2].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[3].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[4].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[5].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[6].DebugIntersects(ray, ref record);
        //         intersects |= _nodes[7].DebugIntersects(ray, ref record);
        //
        //         return intersects;
        //     }
        //
        //     return false;
        // }

        public bool[] FindIntersectingBounds(Ray ray, OctTree root)
        {
            var indexes = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                if (!root._nodes[i]._isTreeBuild || !root._nodes[i]._isTreeReady)
                    indexes[i] = false;
                else
                {
                    var t = root._nodes[i]._bounds.Bounds.Intersects(ray);

                    if (t != null && t != 0)
                    {
                        indexes[i] = true;
                    }
                }
            }

            return indexes;
        }

        public (bool, HitRecord?) Intersects(Ray ray)
        {
            if (!_isTreeBuild)
                return (false, null);

            if (IsLeaveNode() && !HasObjects)
                return (false, null);

            var t = _bounds.Bounds.Intersects(ray);

            if (t == null)
            {
                return (false, null);
            }
            
            var (intersected, record) = NearestIntersection(ray);

            if (!record.HasValue)
            {
                var r = new HitRecord();
                r.T = float.MaxValue;
                record = r;
            }

            for (int i = 0; i < 8; i++)
            {
                var (nodeIntersected, nodeRecord) = _nodes[i].Intersects(ray);

                if (nodeIntersected && nodeRecord.HasValue && nodeRecord.Value.T < record.Value.T)
                {
                    record = nodeRecord;
                    intersected = true;
                }
            }

            return (intersected, record);
        }

        public List<HitRecord> IntersectedObjects(Ray ray)
        {
            if (!_isTreeBuild)
                return new List<HitRecord>();

            if (IsLeaveNode() && !HasObjects)
                return null;

            var t = _bounds.Bounds.Intersects(ray);

            if (t == null)
            {
                return null;
            }
            
            var records  = IntersectingObjects(ray);
            
            for (int i = 0; i < 8; i++)
            {
                var hitList = _nodes[i].IntersectedObjects(ray);

                if (hitList != null && hitList.Count > 0)
                    records.AddRange(hitList);
            }
            
            return records;
        }

        public bool HasObjects => _objects.Count != 0;

        private (bool,HitRecord?) NearestIntersection(Ray ray)
        {
            var lowestRecord = new HitRecord();
            lowestRecord.T = float.MaxValue;

            var intersected = false;

            foreach (var obj in _objects)
            {
                var record = new HitRecord();

                if (obj.Intersects(ray, 0f, 1000f, ref record))
                {
                    if (record.T < lowestRecord.T)
                    {
                        intersected = true;
                        lowestRecord = record;
                    }
                }
            }

            return (intersected, lowestRecord);
        }

        private List<HitRecord> IntersectingObjects(Ray ray)
        {
            var records = new List<HitRecord>();
            
            foreach (var obj in _objects)
            {
                var record = new HitRecord();
                
                if (obj.Intersects(ray, 0f, 1000f, ref record))
                {
                    records.Add(record);
                }
            }

            return records;
        }

        public bool IsRoot()
        {
            return _level == 0;
        }

        public bool IsLeaveNode()
        {
            return _nodes[0] == null;
        }

        public void PrintQuadTree(OctTree tree, int quad)
        {
            if (tree == null)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine("");

            for (int i = 0; i < tree._level; i++)
            {
                System.Diagnostics.Debug.Write("\t");
            }
            System.Diagnostics.Debug.Write(String.Format("[ {0} Level {1}; Objects: {2} min: {3} max: {4}", quad, tree._level, tree._objects.Count, tree._bounds.Min, tree._bounds.Max));

            for (int i = 0; i < tree._nodes.Length; i++)
            {
                PrintQuadTree(tree._nodes[i], i);
            }

            System.Diagnostics.Debug.Write(String.Format(" ]"));
        }

        public OctTree GetNode(int i)
        {
            return _nodes[i];
        }

        public IRaytracable GetObject(int i)
        {
            return _objects[i];
        }
    }
}
