namespace Core.Data
{
    public interface IJoin
    {
        string[] GetJoin();
        int PriorityJoins { get; }
        bool IsRoot { get; }
    }
}
