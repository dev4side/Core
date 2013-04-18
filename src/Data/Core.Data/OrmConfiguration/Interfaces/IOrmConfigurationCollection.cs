using System;
using System.Collections.Generic;

namespace Core.Data.OrmConfiguration.Interfaces
{
    public interface IOrmConfigurationCollection : IList<IOrmConfiguration>
    {
        IOrmConfiguration TryGetOrmConfigration(Type type);
    }
}
