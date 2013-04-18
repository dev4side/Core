namespace Core.Data.Interfaces.Repository
{
    public interface IProjection
    {
        string ProjectionName { get; }
        string ProjectionAlias { get; }
        IJoin Join { get; }
        IGroupBy GroupBy { get; }
    }
}
