using System;
using Core.Data.Interfaces.Repository;

namespace Core.Data.Interfaces.Factory
{
    public interface IUnitOfWorkFactoryOrmConfigurated
    {
        IUnitOfWork CreateUnitOfWork(Type type);
    }
}
