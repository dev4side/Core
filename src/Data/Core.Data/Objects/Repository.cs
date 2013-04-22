using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Data.Exceptions;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Core.Log;
using Ninject;

namespace Core.Data.Objects
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        #region services

        [Inject]
        public ILog<Repository<TEntity, TKey>> Log { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IRepositoryFactoryGeneric<TKey> RepositoryFactory { get; set; }

        public IUnitOfWork UnitOfWork { get; protected set; }

        #endregion

        private string _currentTypeName;

        public string CurrentTypeName
        {
            get { return _currentTypeName ?? (_currentTypeName = typeof (TEntity).Name); }
        }

        public Repository(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public IRepository<T, TKey> GetRepository<T>() where T : class, IDomainEntity<TKey>
        {
            return RepositoryFactory.GetRepository<T>(this.UnitOfWork);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                Log.Debug("Insert {0} with id: {1}", this.CurrentTypeName, entity.Id.ToString());
                this.UnitOfWork.Insert(entity);
                Log.Debug("Insered entity id: {0}", entity.Id.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Log.Debug("Update {0} with id: {1}", this.CurrentTypeName, entity.Id.ToString());
            this.UnitOfWork.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Log.Debug("Delete {0} id: {1}", this.CurrentTypeName, entity.Id.ToString());
            this.UnitOfWork.Delete(entity);
        }

        public TEntity GetById(TKey id)
        {
            Log.Debug("GetById {0} id: {1}", this.CurrentTypeName, id.ToString());
            TEntity result = this.UnitOfWork.GetById<TEntity, TKey>(id);
            if (result == null)
                throw new RepositoryException(String.Format("There is no {0} with id {1}", typeof(TEntity).Name, id));
            return result;
        }

        public TEntity GetByIdOrNull(TKey id)
        {
            Log.Debug("GetByIdOrNull {0} id: {1}", this.CurrentTypeName, id.ToString());
            return this.UnitOfWork.GetById<TEntity, TKey>(id);
        }

        public IQueryable<TEntity> GetAll()
        {
            var result = this.UnitOfWork.Linq<TEntity>();

            // è neccessario fare result.ToList().Count perche usando NHibenrate con il driver Jet per access, viene generata exception
#if DEBUG
            if(Log.IsDebugEnabled())
            {
                var resultAsList = result.ToList();
                Log.Debug("GetAll {0} returned {1} entities", this.CurrentTypeName, resultAsList.Count.ToString(CultureInfo.InvariantCulture));
            }
#endif
            
            return result;
        }

        public IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, bool appendFirstEntityId = true) where TDto : class, new()
        {
            if (Log.IsDebugEnabled())
                Log.Debug("GetDtoByProjections projections: #{0}, restrictions: #{1}, joins: #{2}",
                    projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                    restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                    joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetDtosByprojections<TEntity, TDto>(projections, restrictions, joins, appendFirstEntityId);
        }

        public IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations, bool appendFirstEntityId = true) where TDto : class, new()
        {
            if (Log.IsDebugEnabled())
                Log.Debug(
                    "GetDtoByProjections projections: #{0}, restrictions: #{1}, joins: #{2}, ordinations: #{3}",
                    projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                    restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                    joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                    ordinations != null ? ordinations.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetDtosByprojections<TEntity, TDto>(projections, restrictions, joins, ordinations, appendFirstEntityId);
        }

        public IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, 
            IEnumerable<IJoin> joins, int pageIndex, int pageSize) where TDto : class, new()
        {
            if (Log.IsDebugEnabled())
                Log.Debug(
                    "GetPagedDtoByProjections projections: #{0}, restrictions: #{1}, joins: #{2} with pageIndex: #{3}, pageSize: #{4}",
                    projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                    restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                    joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                    pageIndex.ToString(CultureInfo.InvariantCulture), pageSize.ToString(CultureInfo.InvariantCulture));
            return this.UnitOfWork.GetPagedDtosByprojections<TEntity, TDto>(projections, restrictions, joins, pageIndex, pageSize);
        }

        public IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, int pageIndex, int pageSize, IEnumerable<IOrdination> ordinations) where TDto : class, new()
        {
            if (Log.IsDebugEnabled())
                Log.Debug(
                    "GetPagedDtoByProjections projections: #{0}, restrictions: #{1}, joins: #{2} with pageIndex: #{3}, pageSize: #{4}, ordinations: #{5}",
                    projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                    restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                    joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                    pageIndex.ToString(CultureInfo.InvariantCulture), pageSize.ToString(CultureInfo.InvariantCulture),
                    ordinations != null ? ordinations.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetPagedDtosByprojections<TEntity, TDto>(projections, restrictions, joins, pageIndex, pageSize, ordinations);
        }

        public string GetSqlFromHql(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations)
        {
            if (Log.IsDebugEnabled())
                Log.Debug(
                    "GetSqlFromHql projections: #{0}, restrictions: #{1}, joins: #{2}, ordinations: #{3}",
                    projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                    restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                    joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                    ordinations != null ? ordinations.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetSqlFromHql<TEntity>(projections, restrictions, joins, ordinations);
        }

        public IList<TReturnType> ExecuteSqlQuery<TReturnType>(string sqlQuery) where TReturnType : class
        {
            if (Log.IsDebugEnabled())
                Log.Debug("ExecuteSqlQuery SQL query: #{0}", sqlQuery);
            return this.UnitOfWork.ExecuteSqlQuery<TReturnType>(sqlQuery);
        }

        public TReturnType ExecuteSqlQueryForField<TReturnType>(string sqlQuery)
        {
            if (Log.IsDebugEnabled())
                Log.Debug("ExecuteSqlQuery SQL query: #{0}", sqlQuery);
            return this.UnitOfWork.ExecuteSqlQueryForField<TReturnType>(sqlQuery);
        }
    }
}
