using Core.Data.Interfaces.Repository;

namespace Core.Data.Interfaces.Factory
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
