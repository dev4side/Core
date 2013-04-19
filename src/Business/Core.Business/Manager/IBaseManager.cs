using System;
using Core.Data.Interfaces.Entity;

namespace Core.Business.Manager
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseManager<out TEntity> : IManagerMarker where TEntity : class, IDomainEntity<Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(Guid id);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByIdOrNull(Guid id);
    }
}