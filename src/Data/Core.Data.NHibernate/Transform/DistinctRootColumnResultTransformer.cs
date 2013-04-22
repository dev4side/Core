using System;
using System.Collections.Generic;
using System.Collections;

namespace Core.Data.NHibernate.Transform
{
    [Serializable]
    public class DistinctRootColumnResultTransformer<TDto> : BaseDistinctRootColumnResultTransformer<TDto> where TDto : class, new()
    {
        public override IList TransformList(IList list)
        {
            var distinct = new DistinctIdentiesDic();
            foreach (var dtoInList in list)
            {
                var identity = (Identity)dtoInList;
                distinct.Add(identity.Id, identity);
            }
            return distinct.ConvertToListWithTuples();
        }

        internal sealed class DistinctIdentiesDic : Dictionary<object, Identity>
        {
            public new void Add(object key, Identity identity)
            {
                if (key == null)
                {
                    return;
                }
                    
                Identity candidateIdentityInDictionary;
                if (base.TryGetValue(key, out candidateIdentityInDictionary))
                {
                    candidateIdentityInDictionary.MergeTupleProperties(identity);
                }
                else
                {
                    base.Add(key, identity);
                }
            }

            public IList ConvertToListWithTuples()
            {
                IList result = new List<object>();
                foreach (var value in Values)
                {
                    result.Add(value.Dto);
                }
                return result;
            }
        }
    }
}
