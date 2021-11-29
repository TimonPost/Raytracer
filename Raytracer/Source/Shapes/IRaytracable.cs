namespace Raytracer.Source.Shapes
{
    public interface IRaytracable
    {
        bool Intersects(CustomRay ray, float tmin, float tmax, ref HitRecord record);
    }
}