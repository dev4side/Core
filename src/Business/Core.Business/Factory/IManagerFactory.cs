using Core.Business.Manager;
using Core.Data.Interfaces.Repository;

namespace Core.Business.Factory
{
    public interface IManagerFactory
    {
        /// <summary>
        /// Get the correct manager for the <typeparamref name="TManger"/> type.
        /// </summary>
        /// <typeparam name="TManger">The type of manager.</typeparam>
        /// <param name="unitOfWork">The unit of work </param>
        /// <returns>The instance of <typeparamref name="TManger"/> type.</returns>
        TManger GetManager<TManger>(IUnitOfWork unitOfWork) where TManger : IManagerMarker;
    }
}