namespace Core.Data.Interfaces.Entity
{
    /// <summary>
    /// contratto per definire un domain object del repository
    /// </summary>
    public interface IDomainEntity<TKey>  // where TKey : IConvertible 
    {
        TKey Id { get; set; }
    }
}
