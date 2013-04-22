namespace Core.Data.Interfaces.Entity
{
    /// <summary>
    /// contratto per definire un domain object del repository
    /// </summary>
    public interface IDomainEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
