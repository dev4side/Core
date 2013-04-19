using System;

namespace Core.Test.Constants
{
    public class NUnitCategories
    {
        // indica i test che richiedo l'utilizzo di Nhibernate e quindi per ogni fixture è neccessario ricreare il db.
        public const string NHIBERNATE_SLOW_TEST = "[Slow] NHibernate";
        public const string NHIBERNATE_ENTITY_CRUD_SLOW_TEST = "[Slow] NHibernate entity CRUD";

        // indica i test che per ogni fixture creano di un'istanza di ServiceHost con relativo binding
        public const string WCF_SERVICE_HOST_SLOW_TEST = "[Slow] Wcf ServiceHost";

        public const string SERVICES = "Services";

        // indica i test che utilizzano oggetti creati da IoC
        public const string MOCK_STUB_TEST = "Mock or Stub";
    }
}
