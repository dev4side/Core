using System;

namespace Core.Data
{
    /// <summary>
    /// Crea una copia dell'oggetto con id azzerato, mantendo tutte le  reference dell' oggetto copiato.
    /// </summary>
    public interface ICopy<T> where T : IDomainEntity<Guid>
    {
        T Copy();
    }
}
