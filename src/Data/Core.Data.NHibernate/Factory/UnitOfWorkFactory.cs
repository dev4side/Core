﻿using Core.Data.Interfaces.Factory;
using Core.Data.Interfaces.Repository;
using Ninject;

namespace Core.Data.NHibernate.Factory
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IUnitOfWork CreateUnitOfWork()
        {
            return Kernel.Get<IUnitOfWork>();
        }
    }
}
