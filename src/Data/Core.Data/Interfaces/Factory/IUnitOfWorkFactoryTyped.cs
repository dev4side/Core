using System;

namespace Core.Data
{
    public interface IUnitOfWorkFactoryOrmConfigurated
    {
        IUnitOfWork CreateUnitOfWork(Type type);
    }
}
