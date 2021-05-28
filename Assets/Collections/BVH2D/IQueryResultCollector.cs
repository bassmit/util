using Unity.Entities;

namespace Collections.BVH2D
{
    public interface IQueryResultCollector
    {
        bool QueryCallback(Entity node);
    }
}