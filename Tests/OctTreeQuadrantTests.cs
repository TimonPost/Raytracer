using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raytracer;
using Raytracer.Source;
using Raytracer.Source.Shapes;
using Xunit;

namespace Tests
{
    public class SphereBoundingBox
    {
        [Fact]
        public void CorrectBoundingBox()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(2, 2, 2);
            var sphere = new Sphere(position, 1.5f, material);

            var expected = new CubeBound(0.5f, 0.5f, 0.5f, 3f, 3f, 3f);
            Assert.Equal(sphere.Bounds, expected);
        }

        [Fact]
        public void CorrectBoundingBoxPiramid()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var a = new Vector3(0, 0, 1);
            var b = new Vector3(0, 0, 0);
            var c = new Vector3(1, 0, 0);
            var d = new Vector3(1, 0, 1);
            var top = new Vector3(0.5f, 1f, 0.5f);

            var piramid = new Triangle(a,b,c,d,top, null);

            var expected = new CubeBound(0,0,0, 1f, 1f, 1f);
            Assert.Equal(piramid.Bounds, expected);
        }
    }

    public class OctTreeQuadrantTests
    {
        private CubeBound cubeBound = new CubeBound(0, 0, 0, 10f, 10f, 10f);
        
        [Fact]
        public void Quadrant1()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));
            
            var position = new Vector3(2, 2, 2);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

           var obj = oct.GetNode(0).GetObject(0);

            Assert.True(sphere == obj);
        }

        [Fact]
        public void Quadrant2()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(7, 2, 2);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(1).GetObject(0);

            Assert.True(sphere == obj);
        }
        
        [Fact]
        public void Quadrant3()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(2, 2, 7);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(2).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void Quadrant4()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(7, 2, 7);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(3).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void Quadrant5()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(2, 7, 2);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(4).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void Quadrant6()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(7, 7, 2);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(5).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void Quadrant7()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(2, 7, 7);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(6).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void Quadrant8()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(7, 7, 7);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetNode(7).GetObject(0);

            Assert.True(sphere == obj);

        }

        [Fact]
        public void OverlappingObjectBoundsQuadrant()
        {
            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));

            var position = new Vector3(5, 5, 5);
            var sphere = new Sphere(position, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, cubeBound);
            oct.UpdateTree();

            var obj = oct.GetObject(0);

            Assert.True(sphere == obj);
        }

        [Fact]
        public void RayShootsInOtherQuadrant()
        {
            var obj2Pos = new Vector3(3f * 3f, 10f * 3f, 5f * 3);

            CubeBound bounds = new CubeBound(0, 0, 0, 30, 30, 30);

            var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));
            
            var sphere = new Sphere(obj2Pos, 1.0f, material);

            OctTree oct = new OctTree(new List<IRaytracable>() { sphere }, bounds);
            oct.UpdateTree();

            var origin = new Vector3(15f, 15f, 31f);
            
            var a = oct.Intersects(new Ray(origin, obj2Pos - origin));

            //Assert.True(sphere);
        }
    }
}
