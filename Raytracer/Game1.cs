﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Raytracer.Raytracer;
using Raytracer.Source.Lights;
using Raytracer.Source.Shapes;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int Width = 800;
        public static int Height = 400;

        private Texture2D _canvas;
        private World _world;
        private RayTracer _rayTracer;

        bool _needsDraw = false;
        private Camera _camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int) Width;
            _graphics.PreferredBackBufferHeight = (int) Height;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            // Create the canvas pixel array.
       
            // Create the canvas texture.
            _canvas = new Texture2D(_graphics.GraphicsDevice, (int) Width, (int) Height);
           
            Vector3 lookFrom = new Vector3(5f, 8f, 10f);
            Vector3 lookAt = new Vector3(5f, 0f, 0f);
            float aspectRatio = Width / Height;
            float fov =70f;

            //var camera = new Camera(lookFrom, lookAt, new Vector3(0f, 1f, 0f), fov, aspectRatio);
            _camera = new Camera(lookFrom, lookAt, new Vector3(0f,1f,0f), fov, aspectRatio);
            _rayTracer = new RayTracer(_camera, Width, Height, 6);

            _world = CubeScene();

            base.Initialize();
        }
        
        private World CubeScene()
        {
            var spheres = new List<IRaytracable>();

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    for (int z = 0; z < 5; z++)
                    {
                        var position = new Vector3(x, y, -z);

                        var material = new Lambertian(new Vector3(1.0f, 0.0f, 0.0f));
                        var sphere = new Sphere(position, 0.5f, material);
                        spheres.Add(sphere);
                    }
                }
            }

            return new World(spheres, new List<PointLight>());
        }

        private World TestScene()
        {
            // entities.Add(
            //     new Sphere(new Vector3(0f, 1f, 0f), 1.0f, new Dielectric(1.5f)));
            // entities.Add(
            //     new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, new Lambertian(new Vector3(1.0f, 0.0f, 0.0f))));
              // entities.Add(
              //     new Sphere(new Vector3(4f, 1f, 1f), 1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)));

            
            return new World(
                new List<IRaytracable>() {
                   
                    new Sphere(new Vector3(3f, 1f, 0f), 1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)),
                    new Sphere(new Vector3(2f, 1f, 2f), 0.5f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)),
                    new Sphere(new Vector3(1f, 3f, 0f), 0.5f, new Lambertian(new Vector3(1.0f, 0.0f, 0.0f))),

                    //new Plane(new Vector3(0, -2, 0), -Vector3.Up)
                    // new AABBox(new Vector3(-0f, 0.5f,2f), new Vector3(1f, 1.5f, 3f), new Dielectric(0.5f)),
                    // new AABBox(new Vector3(-4f, 0.5f,2f), new Vector3(-3f, 1.5f, 3f),  new Lambertian(new Vector3(1f, 0f, 0f))),
                    //new AabBox(new Vector3(1.5f, 0.5f,1f), new Vector3(2f, 1.5f, 2f), new Dielectric(1.5f)),
                    // new Triangle(
                    //     new Vector3(-2f,0f,0f),
                    //     new Vector3(-2f,0f,-2f),
                    //     new Vector3(0f,0f,-2f),
                    //     new Vector3(-1f,0f,0f),
                    //     new Vector3(1f,2f,-1f)
                    //     , new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)), //new Lambertian(new Vector3(0.9f, 0.7f, 0.2f)))
                    

                },
                new List<PointLight>()
                {
                    new PointLight(new Vector3(3f, 3f, 0f), new Vector3(0f, -1f, 0f), new Vector3(1f, 1f, 1f))
                });
        }

        private World SphereScene()
        {
            // var camera = new Camera(lookFrom, lookAt, new Vector3(0f, 1f, 0f), fov, aspectRatio);
            var entities = new List<Sphere>();
            entities.Add(new Sphere(new Vector3(0f, -1000, 0f), 1000f, new Lambertian(new Vector3(0.5f, 0.5f, 0.5f))));

            
             var random = new Random();
             for (int a = -11; a < 11; a++)
             {
                 for (int b = -11; b < 11; b++)
                 {
                     float chooseMat = (float)random.NextDouble();
                     Vector3 center = new Vector3(a + 0.9f * (float)random.NextDouble(), 0.2f,  b + 0.9f * (float)random.NextDouble());
            
                     if ((center - new Vector3(4f, 0.2f, 0f)).Length() > 0.9)
                     {
                         if (chooseMat < 0.8)
                         {
                             // diffuse
                             entities.Add(new Sphere(center, 0.2f, new Lambertian(new Vector3((float)random.NextDouble()* (float)random.NextDouble(), (float)random.NextDouble()* (float)random.NextDouble(), (float)random.NextDouble()* (float)random.NextDouble()))));
            
                         }else if (chooseMat < 0.95f)
                         {
                             entities.Add(
                                 new Sphere(center, 0.2f, 
                                     new Metal(new Vector3(
                                         0.5f * (1f + (float)random.NextDouble()),
                                         0.5f * (1f + (float)random.NextDouble()),
                                         0.5f * (1f + (float)random.NextDouble())
                                     ),0.5f*(float)random.NextDouble())));
                         }
                         else
                         {
                             entities.Add(
                                 new Sphere(center, 0.2f, new Dielectric(1.5f)));
                         }
                     }
                 }
             }

            entities.Add(
                 new Sphere(new Vector3(0f, 1f, 0f), 1.0f, new Dielectric(1.5f)));
             entities.Add(
                 new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
             entities.Add(
                 new Sphere(new Vector3(4f, 1f, 0f), 1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)));

             return new World(entities.Cast<IRaytracable>().ToList(), new List<PointLight>());
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                   Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Enter))
                _needsDraw = true;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_needsDraw)
            {
                _rayTracer.Run(_world);
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            this.Render();
            var cube = new Cube(Color.Red, GraphicsDevice, _camera);
            cube.Render();

            _spriteBatch.End();

            base.Draw(gameTime);

            _needsDraw = false;
        }

       

        public static Vector3 RandomInUnitSphere()
        {
            Vector3 randomPoint;
            var random = new Random(); 
            do
            {
                randomPoint = 2.0f * new Vector3((float) random.NextDouble(), (float) random.NextDouble(),
                    (float) random.NextDouble()) - new Vector3(1.0f);
            } while (randomPoint.LengthSquared() >= 1.0);

            return randomPoint;
        }
        protected void Render()
        {
            var array = _rayTracer.FrameBuffer.Pixels();
            _canvas.SetData(array, 0, array.Length);

            _spriteBatch.Draw(_canvas, new Rectangle(0, 0, Width, Height),null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
        }

    }
}
