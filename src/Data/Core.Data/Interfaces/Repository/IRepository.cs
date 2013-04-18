using System.Collections.Generic;
using System.Linq;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;

namespace Core.Data.Interfaces.Repository
{
   /// <summary>
    ///  Interfaccia di un repository generico
   /// </summary>
   /// <typeparam name="TEntity"></typeparam>
   /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        IRepositoryFactoryGeneric<TKey> RepositoryFactory { get; }

        IRepository<T, TKey> GetRepository<T>() where T : class, IDomainEntity<TKey>;

        /// <summary>
        /// Inserisce l' entità nel db
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        /// Update dell' entità nel db
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Delete dell' entità nel db
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// Get dell' elemento tramite id
        /// </summary>
        TEntity GetById(TKey id);

        TEntity GetByIdOrNull(TKey id);

        /// <summary>
        /// Get di tutte le entità
        /// </summary>
        IQueryable<TEntity> GetAll();
       
        IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections,
                                              IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins,
                                              bool appendFirstEntityId = true)
           where TDto : class, new();

       IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections,
                                             IEnumerable<IRestriction> restrictions,
                                             IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations,
                                             bool appendFirstEntityId = true)
           where TDto : class, new();

       IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections,
                                                  IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins,
                                                  int pageIndex, int pageSize) where TDto : class, new();

       IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections,
                                                  IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins,
                                                  int pageIndex, int pageSize, IEnumerable<IOrdination> ordinations)
           where TDto : class, new();

       string GetSqlFromHql(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
                            IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations);

       IList<TReturnType> ExecuteSqlQuery<TReturnType>(string sqlQuery) where TReturnType : class;

       TReturnType ExecuteSqlQueryForField<TReturnType>(string sqlQuery);
    }

    // fix Nhib
    public static class NhibeernateFixes
    {
        public static T ElementAtFIX_NHIB<T>(this IQueryable<T> query, int index)
        {
            return query.ToList()[index];
        }

        public static int CountFIX_NHIB<T>(this IQueryable<T> query)
        {
            return query.ToList().Count;
        }
    }
}