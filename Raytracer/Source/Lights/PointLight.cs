using Microsoft.Xna.Framework;

namespace Raytracer.Source.Lights
{
    public class PointLight
    {
        public PointLight(Vector3 position, Vector3 direction, Vector3 intensity)
        {
            Position = position;
            Direction = direction;
            Intensity = intensity;
        }

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Intensity { get; set; }
    }
}