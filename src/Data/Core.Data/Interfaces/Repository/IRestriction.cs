namespace Core.Data.Interfaces.Repository
{
    public interface IRestriction
    {
        string GetRestriction();
        IJoin Join { get; }
    }
}
