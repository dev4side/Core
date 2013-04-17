
namespace Core.Data
{
    public interface IRestriction
    {
        string GetRestriction();
        IJoin Join { get; }
    }
}
