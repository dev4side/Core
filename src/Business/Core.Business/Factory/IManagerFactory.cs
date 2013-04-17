
using Core.Data;

namespace Core.Business
{
    public interface IManagerFactory
    {
        TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IMangerMarker;
    }
}