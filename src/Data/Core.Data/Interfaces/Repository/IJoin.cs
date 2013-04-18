namespace Core.Data.Interfaces.Repository
{
    public interface IJoin
    {
        string[] GetJoin();
        int PriorityJoins { get; }
        bool IsRoot { get; }
    }
}
