using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data.Interfaces.Repository
{
    public interface IUnitOfWork: IDisposable
    {
        /// <summary>
        /// Inserts entity to the storage.
        /// </summary>
        void Insert<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Updates entity in the storage.
        /// </summary>
        void Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Deletes entity in the storage.
        /// </summary>
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Gets entity from the storage by it's Id.
        /// </summary>
        TEntity GetById<TEntity, TKey>(TKey id) where TEntity : class;

        /// <summary>
        /// Gets all entities of the type from the storage. 
        /// </summary>
        IList<TEntity> GetAll<TEntity>() where TEntity : class;

        IQueryable<TEntity> Linq<TEntity>() where TEntity : class;

        IList<TEntity> GetByFilterConstraints<TEntity>(string constraints, string associations) 
            where TEntity : class;

        IList<TEntity> GetByFilterConstraints<TEntity>(string constraints, string associations,
                                                       IEnumerable<string> fetchPropertyEntities)
            where TEntity : class;

        IList<TDto> GetDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections,
                                                        IEnumerable<IRestriction> restrictions,
                                                        IEnumerable<IJoin> joins, bool appendFirstEntityId = true)
            where TEntity : class
            where TDto : class, new();

        IList<TDto> GetDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections,
                                                        IEnumerable<IRestriction> restrictions,
                                                        IEnumerable<IJoin> joins,
                                                        IEnumerable<IOrdination> ordinations,
                                                        bool appendFirstEntityId = true)
            where TEntity : class
            where TDto : class, new();
        
        IList<TDto> GetPagedDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections,
                                                             IEnumerable<IRestriction> restrictions,
                                                             IEnumerable<IJoin> joins, int pageIndex, int pageSize)
            where TEntity : class
            where TDto : class, new();

        IList<TDto> GetPagedDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections,
                                                             IEnumerable<IRestriction> restrictions,
                                                             IEnumerable<IJoin> joins, int pageIndex, int pageSize,
                                                             IEnumerable<IOrdination> ordinations)
            where TEntity : class
            where TDto : class, new();

        /// <summary>
        /// Execute SQL query for reference types
        /// </summary>
        IList<TReturnType> ExecuteSqlQuery<TReturnType>(string sqlQuery) where TReturnType : class;

        /// <summary>
        /// Execute SQL query for value types and reference type
        /// </summary>
        TReturnType ExecuteSqlQueryForField<TReturnType>(string sqlQuery);

        /// <summary>
        /// Gets SQL query from HQL generated with projections, restrictions, joins and ordinations
        /// </summary>
        string GetSqlFromHql<TEntity>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
                                      IEnumerable<IJoin> joins,
                                      IEnumerable<IOrdination> ordinations = null);
    }
}
