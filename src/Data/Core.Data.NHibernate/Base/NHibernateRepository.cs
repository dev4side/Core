using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Exceptions;
using Core.Data.Interfaces.Entity;
using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Ninject;
using Core.Log;
using System.Globalization;

namespace Core.Data.NHibernate
{
    /// <summary>
    /// base repository di Nhibernate. contiente tutte le implentazioni ad hoc di NHIB con pattern IUnitOfWork
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseNHibernateRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IDomainEntity<TKey>
    {
        private string _currentTypeName;

        [Inject]
        public ILog<BaseNHibernateRepository<TEntity, TKey>> Log { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IRepositoryFactoryGeneric<TKey> RepositoryFactory { get; set; }


        public IUnitOfWork UnitOfWork { get; protected set; }

        public string CurrentTypeName
        {
            get { return _currentTypeName ?? (_currentTypeName = typeof (TEntity).Name); }
        }

        public BaseNHibernateRepository(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        IRepository<T, TKey> IRepository<TEntity, TKey>.GetRepository<T>()
        {
            return RepositoryFactory.GetRepository<T>(this.UnitOfWork);
        }

        public void Insert(TEntity entity)
        {
            try
            {
                Log.Debug("Insert {0} with id: {1}", this.CurrentTypeName, entity.Id.ToString());
                this.UnitOfWork.Insert(entity);
                Log.Debug("Insered entity id: {0}", entity.Id.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        public void Update(TEntity entity)
        {
            Log.Debug("Update {0} with id: {1}", this.CurrentTypeName, entity.Id.ToString());
            this.UnitOfWork.Update(entity);
        }

        public void Delete(TEntity entity)
        {
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
            // ReSharper disable RemoveToList.2
            // è neccessario fare result.ToList().Count perche usando NHibenrate con il driver Jet per access, viene generata exception
#if DEGUB
            Log.Debug("GetAll {0} returned {1} entities", this.CurrentTypeName, result.ToList().Count.ToString(CultureInfo.InvariantCulture));
#endif
            // ReSharper restore RemoveToList.2
            return result;
        }


        public IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, bool appendFirstEntityId = true) where TDto : class, new()
        {
            Log.Debug("GetDtosByFilterConstraintsAndProjetions projections: #{0}, restrictions: #{1}, joins: #{2}",
                projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetDtosByprojections<TEntity, TDto>(projections, restrictions, joins, appendFirstEntityId);
        }

        public IList<TDto> GetDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations, bool appendFirstEntityId = true) where TDto : class, new()
        {
            Log.Debug(
                "GetDtosByFilterConstraintsAndProjetions projections: #{0}, restrictions: #{1}, joins: #{2}, ordinations: #{3}",
                projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                ordinations != null ? ordinations.Count().ToString(CultureInfo.InvariantCulture) : "");
            return this.UnitOfWork.GetDtosByprojections<TEntity, TDto>(projections, restrictions, joins, ordinations, appendFirstEntityId);
        }

        public IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, 
            IEnumerable<IJoin> joins, int pageIndex, int pageSize) where TDto : class, new()
        {
            Log.Debug(
                "GetDtosByFilterConstraintsAndProjetions projections: #{0}, restrictions: #{1}, joins: #{2}, pageIndex: #{3}, pageSize: #{4}",
                projections != null ? projections.Count().ToString(CultureInfo.InvariantCulture) : "",
                restrictions != null ? restrictions.Count().ToString(CultureInfo.InvariantCulture) : "",
                joins != null ? joins.Count().ToString(CultureInfo.InvariantCulture) : "",
                pageIndex.ToString(), pageSize.ToString(CultureInfo.InvariantCulture));
            return this.UnitOfWork.GetPagedDtosByprojections<TEntity, TDto>(projections, restrictions, joins, pageSize, pageIndex);
        }

        public IList<TDto> GetPagedDtoByProjections<TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, 
            IEnumerable<IJoin> joins, int pageIndex, int pageSize, IEnumerable<IOrdination> ordinations) 
            where TDto : class, new()
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
                    "GetDtosByFilterConstraintsAndProjetions projections: #{0}, restrictions: #{1}, joins: #{2}, ordinations: #{3}",
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
