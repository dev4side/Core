﻿using System;

namespace Core.Data.NHibernate
{
    public class NHibernateRepositoryOrmConfigurated<TEntity, TKey> : BaseNHibernateRepository<TEntity, TKey>
       where TEntity : class, IDomainEntity<TKey>
    {
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork </param>
        public NHibernateRepositoryOrmConfigurated(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            CheckIsValidType(unitOfWork);
            this.UnitOfWork = unitOfWork;
        }

        private static void CheckIsValidType(IUnitOfWork unitOfWork)
        {
            if (unitOfWork is NhibernateUnitOfWorkOrmConfigurated)
            {
                var nHibConnectorUnitOfWork = unitOfWork as NhibernateUnitOfWorkOrmConfigurated;
                if (!nHibConnectorUnitOfWork.IsValidForType(typeof(TEntity)))
                {
                    throw new RepositoryException(String.Format(
                        "Due to different databases, you cannot use the same UnitoOfwork to commit repositories of different db! {0} is not part of {1}",
                        typeof(TEntity).ToString(), nHibConnectorUnitOfWork.Configuration.Name));
                }
            }
        }

    }
}