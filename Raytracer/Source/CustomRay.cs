using System.Numerics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Raytracer
{
    public struct CustomRay
    {
        public CustomRay(Vector3 o, Vector3 d)
        {
            this.O = o;
            this.D =d;
        }

        public Vector3 D { get; set; }

        public Vector3 O { get; set; }

        public Vector3 Origin()
        {
            return O;
        }

        public Vector3 Direction()
        {
            return D;
        }

        public Vector3 PointAt(float t)
        {
            return O + D * t;
        }
    }
}