using Unity.Entities;

namespace Collections.BVH2D
{
    public interface IRayCastResultCollector
    {
        float RayCastCallback(RayCastInput subInput, Entity node);
    }
}