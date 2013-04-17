using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data
{
    public interface IClone<T>
    {
        T CloneAsNew();
    }
}
