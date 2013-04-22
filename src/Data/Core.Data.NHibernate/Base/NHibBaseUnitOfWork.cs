using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Data.Interfaces.Repository;
using Core.Log;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using Ninject;
using NHibernate;
using Core.Data.NHibernate.Hql;
using Core.Data.NHibernate.Transform;
using NHibernate.Transform;
using NHibernate.Linq;

namespace Core.Data.NHibernate.Base
{
    /// <summary>
    ///Concrete class of UnitOfWork using the NHib implementation
    /// </summary>
    public abstract class NHibBaseUnitOfWork : IUnitOfWork
    {
        [Inject]
        public ILog<NHibBaseUnitOfWork> Log { get; set; }

        public ISession Session { get; set; }

        public virtual void Dispose()
        {
            if (this.Session == null)
            {
                return;
            }
            if (this.Session.IsDirty())
            {
                this.Flush();
            }

            this.Session.Dispose();
            this.Session = null;
        }

        public void Flush()
        {
            Session.Flush();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            Session.SaveOrUpdate(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            Session.Update(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            Session.Delete(entity);
        }

        public TEntity GetById<TEntity, TKey>(TKey id) where TEntity : class
        {
            return Session.Get<TEntity>(id);
        }

        public IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return this.Session.CreateCriteria<TEntity>().List<TEntity>();
        }

        public IQueryable<TEntity> Linq<TEntity>() where TEntity : class
        {
            return this.Session.Query<TEntity>();
        }

        public IList<TEntity> GetByFilterConstraints<TEntity>(string constraints, string associations) where TEntity : class
        {
            var hql = HqlGenerator.GetEntitiesQuery<TEntity>(constraints, associations);
            Log.Debug("A Filter constraints is generated and submitted: {0}", hql);
            var result = Session.CreateQuery(hql).List<TEntity>();
            Log.Debug("Filter query has returned {0} items", result.Count.ToString(CultureInfo.InvariantCulture));
            return result;
        }

        public IList<TEntity> GetByFilterConstraints<TEntity>(string constraints, string associations, IEnumerable<string> fetchEntities) where TEntity : class
        {
            var hql = HqlGenerator.GetFetchEntitiesQuery<TEntity>(constraints, associations, fetchEntities);
            Log.Debug("A Filter constraints is generated and submitted: {0}", hql);
            var result = Session.CreateQuery(hql).SetResultTransformer(new DistinctRootEntityResultTransformer()).List<TEntity>();
            Log.Debug("Filter query has returned {0} items", result.Count.ToString(CultureInfo.InvariantCulture));
            return result;
        }
       
        public IList<TDto> GetDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, bool appendFirstEntityId = true)
            where TEntity : class
            where TDto : class, new()
        {
            var hqlCreatedQuery = HqlCreatedQuery<TEntity>(projections, restrictions, joins, null, appendFirstEntityId);
            var result = hqlCreatedQuery.SetResultTransformer(new DistinctRootColumnResultTransformer<TDto>()).List<TDto>();
            return result;
        }

        public IList<TDto> GetDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations, bool appendFirstEntityId = true)
            where TEntity : class
            where TDto : class, new()
        {
            var hqlCreatedQuery = HqlCreatedQuery<TEntity>(projections, restrictions, joins, ordinations, appendFirstEntityId);
            var result = hqlCreatedQuery.SetResultTransformer(new DistinctRootColumnResultTransformer<TDto>()).List<TDto>();
            return result;
        }

        public IList<TDto> GetPagedDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, 
            IEnumerable<IJoin> joins, int pageIndex, int pageSize)
            where TEntity : class
            where TDto : class, new()
        {
            var hqlCreatedQuery = HqlCreatedQuery<TEntity>(projections, restrictions, joins);
            var result = hqlCreatedQuery.SetResultTransformer(new DistinctRootColumnResultTransformerPaged<TDto>((pageIndex * pageSize), pageSize)).List<TDto>();
            return result;
        }

        public IList<TDto> GetPagedDtosByprojections<TEntity, TDto>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions,
            IEnumerable<IJoin> joins, int pageIndex, int pageSize, IEnumerable<IOrdination> ordinations)
            where TEntity : class
            where TDto : class, new()
        {
            var hqlCreatedQuery = HqlCreatedQuery<TEntity>(projections, restrictions, joins, ordinations);
            var result = hqlCreatedQuery.SetResultTransformer(new DistinctRootColumnResultTransformerPaged<TDto>((pageIndex * pageSize), pageSize)).List<TDto>();
            return result;
        }

        private IQuery HqlCreatedQuery<TEntity>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins,
                                                IEnumerable<IOrdination> ordinations = null, bool appendFirstEntityId = true) 
        {
            var hql = HqlGenerator.GetProjectionQuery<TEntity>(projections, restrictions, joins, ordinations, appendFirstEntityId);
            Log.Debug("A Filter constraints with projections is generated and submitted: {0}", hql);
            IQuery hqlCreatedQuery;
            try
            {
                hqlCreatedQuery = Session.CreateQuery(hql);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    String.Format("Cannot create SQL from given HQL. Review your Projections, Restrictions and Joins. The bad hql generated is: [{0}]. more details: {1}",
                    hql, ex.Message));
            }
            return hqlCreatedQuery;
        }

        public IList<TReturnType> ExecuteSqlQuery<TReturnType>(string sqlQuery) where TReturnType : class
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new Exception("Sql query could not be null or empty.");
            }

            var query = Session.CreateSQLQuery(sqlQuery);
            return query.SetResultTransformer(Transformers.AliasToBean<TReturnType>()).List<TReturnType>();
        }

        public TReturnType ExecuteSqlQueryForField<TReturnType>(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new Exception("Sql query could not be null or empty.");
            }

            var query = Session.CreateSQLQuery(sqlQuery);
            return query.UniqueResult<TReturnType>();
        }

        public string GetSqlFromHql<TEntity>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins,
                                             IEnumerable<IOrdination> ordinations = null)
        {
            var hqlCreatedQuery = HqlCreatedQuery<TEntity>(projections, restrictions, joins, ordinations);
            var sessionImplementor = (ISessionImplementor)Session;
            var sessionFactoryImplementor = (ISessionFactoryImplementor)Session.SessionFactory;
            HQLStringQueryPlan convertedHqlQuery = null;

            try
            {
                convertedHqlQuery = new HQLStringQueryPlan(hqlCreatedQuery.ToString(), true, sessionImplementor.EnabledFilters, sessionFactoryImplementor);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    String.Format("Cannot convert SQL from generated HQL. Review your Projections, Restrictions and Joins. The hql generated is: [{0}]. more details: {1}",
                    hqlCreatedQuery, ex.Message));
            }
            
            var sqlCovertedQuery = convertedHqlQuery.SqlStrings[0].ToString(CultureInfo.InvariantCulture);
            
            Log.Debug("HQL converter returned query: {0}", sqlCovertedQuery);
            return sqlCovertedQuery;
        }
    }
}
