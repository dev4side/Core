namespace Core.Data.Interfaces.Repository
{
    public interface IGroupBy
    {
        bool StopGroupBy { get; }
        string GetGroupBy();
    }
}
