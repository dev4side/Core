using Core.Business.Manager;
using Core.Data.Interfaces.Repository;

namespace Core.Business.Factory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IManagerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TManger"></typeparam>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IManagerMarker;
    }
}