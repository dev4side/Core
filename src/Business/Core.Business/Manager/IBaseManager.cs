using System;
using Core.Data.Interfaces.Entity;

namespace Core.Business.Manager
{
    public interface IBaseManager<out TEntity> : IManagerMarker where TEntity : class, IDomainEntity<Guid>
    {
        /// <summary>
        /// Get the <typeparamref name="TEntity"/> domain entity with specified id.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        /// <returns>The <typeparamref name="TEntity"/> domain entity. This method does not return null objects.</returns>
        TEntity GetById(Guid id);

        /// <summary>
        /// Get the <typeparamref name="TEntity"/> domain entity with specified id.
        /// </summary>
        /// <param name="id">The Guid id.</param>
        /// <returns>The <typeparamref name="TEntity"/> domain entity. This method could return null objects.</returns>
        TEntity GetByIdOrNull(Guid id);
    }
}