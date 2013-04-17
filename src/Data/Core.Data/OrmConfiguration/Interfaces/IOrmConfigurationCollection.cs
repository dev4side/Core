using System;
using System.Collections.Generic;

namespace Core.Data.Configuration
{
    public interface IOrmConfigurationCollection : IList<IOrmConfiguration>
    {
        IOrmConfiguration TryGetOrmConfigration(Type type);
    }
}
