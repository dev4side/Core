using System.Collections.Generic;
using System.IO;

namespace Core.Data.Configuration
{
    public interface IOrmConfiguration
    {
        string Name { get; }
        string DatabaseName { get; }
        FileInfo ConfigurationFile { get; }
        string ConfigurationFileName { get; }
        IList<string> ControlTypes { get; } 
    }
}
