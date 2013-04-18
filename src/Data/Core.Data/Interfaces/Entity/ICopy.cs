using System;

namespace Core.Data.Interfaces.Entity
{
    /// <summary>
    /// Crea una copia dell'oggetto con id azzerato, mantendo tutte le  reference dell' oggetto copiato.
    /// </summary>
    public interface ICopy<out T> where T : IDomainEntity<Guid>
    {
        T Copy();
    }
}
