using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.Interfaces
{
    /// <summary>
    /// Crea una copia dell'oggetto con id azzerato,creando una copia per ogni figlio.
    /// </summary>
    public interface IDeepCopy<T> where T : IDomainEntity<Guid>
    {
        T DeepCopy();
    }
}
