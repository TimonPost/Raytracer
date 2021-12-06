using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Raytracer.Raytracer;
using Raytracer.Source.Shapes;

namespace Raytracer
{

    class RayTracer
    {
        public FrameBuffer FrameBuffer { get; }
        private Camera _camera;

        public int ParallelRenderBlockSize { get;}
        public int MaxRenderJobs { get; } = 5;

        public int BlockHeight => Height / ParallelRenderBlockSize;
        public int BlockWidth => Width / ParallelRenderBlockSize;
        public int Width { get; }
        public int Height { get; }
       
        public int PixelSampleCount = 120;
        public int RayDepth = 2;
        public float MinIntersection = 0.001f;
        public float MaxIntersection = 1000;
        private List<Task> _tasks = new List<Task>();
        private List<RenderBlockJob> _jobs;

        public RayTracer(Camera camera, int width, int height, int parallelRenderBlockSize)
        {
            FrameBuffer = new FrameBuffer();
            _camera = camera;
            ParallelRenderBlockSize = parallelRenderBlockSize / 2;
            Width = width;
            Height = height;

            _jobs = new List<RenderBlockJob>();
            QueueRenderWithQuality(RenderQuality.Low);
            var rnd = new Random();
            _jobs = _jobs.Select(x => new { value = x, order = rnd.Next() })
                .OrderBy(x => x.order).Select(x => x.value).ToList();
        }

        private void QueueRenderWithQuality(RenderQuality quality)
        {
            for (int y = 0; y <= Height - BlockHeight; y += BlockHeight)
            for (int x = 0; x <= Width - BlockWidth; x += BlockWidth)
                _jobs.Add(new RenderBlockJob(quality, x, y, x + BlockWidth, y + BlockHeight));
        }

        private static object lockJob = new object();
        private Task RenderTask;

        public void Run(World world)
        {
            var tasks = new Task[MaxRenderJobs];

            Task ScheduleNewTask()
            {
                var (job, success) = RemoveJob();

                if (!success)
                    return Task.CompletedTask;

                return Task.Run(() =>
                {
                    BuildRayTraceTask(world, job.RenderQuality, job.StartX, job.StartY, job.EndX, job.EndY);
                }).ContinueWith((Task a) => ScheduleNewTask()); ;
            }

            for (int i = 0; i < MaxRenderJobs; i++)
            {
                var (job, success) = RemoveJob();

                if (!success)
                    break;

                tasks[i] = Task.Run(() =>
                {
                    BuildRayTraceTask(world, job.RenderQuality, job.StartX, job.StartY, job.EndX, job.EndY);
                }).ContinueWith((Task a) => ScheduleNewTask());
            }
        }

        private (RenderBlockJob, bool) RemoveJob()
        {
            RenderBlockJob job;

            lock (lockJob)
            {
                if (_jobs.Count > 0) {
                    job = _jobs[^1];
                    Debug.WriteLine("Remove job: {0}", _jobs.Count - 1);
                    _jobs.RemoveAt(_jobs.Count - 1);
                }
                else
                {
                    return (default, false);
                }
            }

            return (job, true);
        }

        private int SampleCountFromQuality(RenderQuality quality)
        {
            switch (quality)
            {
                case RenderQuality.Low:
                    return 40;
                case RenderQuality.Medium:
                    return 50;
                case RenderQuality.High:
                    return 100;
                default:
                    throw new ArgumentOutOfRangeException(nameof(quality), quality, null);
            }
        }

        private void BuildRayTraceTask(World world, RenderQuality quality, int startX, int startY, int endX, int endY)
        {
            var rand = new Random();

            for (int r = startY; r < endY; r++)
            {
                for (int c = startX; c < endX; c++)
                {
                    Vector3 color = Vector3.Zero;

                    for (int s = 0; s < SampleCountFromQuality(quality); s++)
                    {
                        var urand = rand.NextDouble();
                        var vrand = rand.NextDouble();

                        if (r == 280 && c == 191)
                        {

                        }

                        float u = (float)(c + urand) / (Width);
                        float v = (float)(r + vrand) / (Height);

                        var ray = _camera.GetRay(u, v);
                        color += Trace(world, ray, 0);
                    }

                    color /= PixelSampleCount;
                    color = new Vector3(MathF.Sqrt(color.X), MathF.Sqrt(color.Y), MathF.Sqrt(color.Z));

                    lock (lockJob)
                    {
                        FrameBuffer.SetPixel((int)c, (int)r, new Color(color));
                    }
                }
            }
        }

        private Vector3 Trace(World world, CustomRay ray, int depth)
        {
            var record = new HitRecord();


            if (world.Intersects(ray, MinIntersection, MaxIntersection, ref record))
            {
                CustomRay scattered = new CustomRay(Vector3.Zero, Vector3.Zero);
                Vector3 attenuation = Vector3.One;
                
                if (depth < RayDepth && record.Material.Scatter(ray, record, ref attenuation, ref scattered))
                {
                    var att = attenuation * Trace(world, scattered, depth + 1);

                    // if (!world.HitsLight(ray, record.P))
                    // {
                    //     att *= 0.2f;
                    // } 
                    
                    return att;
                }
                else
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }


                return attenuation;
            }

            


            var direction = Vector3.Normalize(ray.Direction());
            var t = 0.5f * (direction.Y + 1.0f);
            var blend = (1.0f - t) * new Vector3(1f, 1f, 1f) + t * new Vector3(0.5f, 0.7f, 1f);
            return blend;
        }


    }
    

    enum RenderQuality
    {
        Low,
        Medium,
        High
    }

    struct RenderBlockJob
    {
        public RenderBlockJob(RenderQuality quality, int startX, int startY, int endX, int endY)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            RenderQuality = quality;
        }

        public RenderQuality RenderQuality { get; }
        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;
    }
}