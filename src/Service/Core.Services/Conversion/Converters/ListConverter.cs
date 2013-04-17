using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Services.Conversion
{
    /// <summary>
    /// é presente un problema con i generics e lereditarietà:
    /// if you have a class B, that 
    ///inherits from A, then C<B> does NOT inherit from C<A>.  The same goes for 
    ///casting. 
    /// http://groups.google.com/group/microsoft.public.dotnet.languages.csharp/browse_frm/thread/119f8362a9f5ff52?pli=1
    /// questo helper risolve questo problema.
    /// </summary>
    public class ListConverter
    {
        public static IList<TInterface> ConvertToIlistOfInterface<TInterface>(IEnumerable<TInterface> concrateList) 
        {
            var result = new List<TInterface>();
            result.AddRange(concrateList);
            return result;
        }
    }
}
