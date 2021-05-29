namespace Collections.BVH2D
{
    public interface IRayCastResultCollector<in T>
    {
        float RayCastCallback(RayCastInput subInput, T node);
    }
}