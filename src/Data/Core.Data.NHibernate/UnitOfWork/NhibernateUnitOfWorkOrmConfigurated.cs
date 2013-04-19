using System;
using System.Linq;
using Core.Data.OrmConfiguration.Interfaces;
using NHibernate;

namespace Core.Data.NHibernate.UnitOfWork
{
    public class NhibernateUnitOfWorkOrmConfigurated : NHibBaseUnitOfWork
    {
        public IOrmConfiguration Configuration { get; set; }

        public NhibernateUnitOfWorkOrmConfigurated(ISessionFactory sessionFactory, IOrmConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Session = sessionFactory.OpenSession();
        }

        public bool IsValidForType(Type candidateType)
        {
            var candidateTypeAsString = candidateType.ToString();
            return Configuration.ControlTypes.Any(candidateTypeAsString.Equals);
        }
    }
}