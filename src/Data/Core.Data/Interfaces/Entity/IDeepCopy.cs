using System;

namespace Core.Data.Interfaces.Entity
{
    /// <summary>
    /// Crea una copia dell'oggetto con id azzerato,creando una copia per ogni figlio.
    /// </summary>
    public interface IDeepCopy<out T> where T : IDomainEntity<Guid>
    {
        T DeepCopy();
    }
}
