using System;
using Core.Data;

namespace Core.Business
{
    // marker
    public interface IMangerMarker
    {

    }

    public interface IBaseManager<TEntity> : IMangerMarker where TEntity : class, IDomainEntity<Guid>
    {
        TEntity GetById(Guid id);
        TEntity GetByIdOrNull(Guid id);
    }
}