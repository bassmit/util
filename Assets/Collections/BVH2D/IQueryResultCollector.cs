namespace Collections.BVH2D
{
    public interface IQueryResultCollector<in T>
    {
        bool QueryCallback(T node);
    }
}