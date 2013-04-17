using System;
using System.Collections.Generic;
using System.Collections;

namespace Core.Data.NHibernate.Transform
{
    [Serializable]
    public class DistinctRootColumnResultTransformerPaged<TDto> : BaseDistinctRootColumnResultTransformer<TDto> where TDto : class, new()
    {
        private int _starIndex = 0;
        private int _elementsNumber = Int32.MaxValue;
        private DistinctIdentiesDic distinct = null;

        public DistinctRootColumnResultTransformerPaged(int startIndex, int elementsNumber) : base()
        {
            _starIndex = startIndex;
            _elementsNumber = elementsNumber;
            distinct = new DistinctIdentiesDic(_starIndex, _elementsNumber);
        }

        public override object TransformTuple(object[] tuple, string[] aliases)
        {
            return !distinct.AddNew(tuple[0]) ? null : base.TransformTuple(tuple, aliases);
        }

        public override IList TransformList(IList list)
        {
            var distinctIdentities = new DistinctIdentities();

            foreach (var dtoInList in list)
            {
                if (dtoInList == null) continue;
                var identity = (Identity)dtoInList;
                distinctIdentities.Add(identity.Id, identity);
            }
            return distinctIdentities.ConvertToListWithTuples();
        }

        internal sealed class DistinctIdentities : Dictionary<object, Identity>
        {
            public new void Add(object key, Identity identity)
            {
                if (key == null) return;

                Identity candidateIdentityInDictionary;
                if (base.TryGetValue(key, out candidateIdentityInDictionary))
                    candidateIdentityInDictionary.MergeTupleProperties(identity);
                else base.Add(key, identity);
            }

            public IList ConvertToListWithTuples()
            {
                IList result = new List<object>();
                foreach (var value in Values)
                    result.Add(value.Dto);
                return result;
            }
        }

        internal sealed class DistinctIdentiesDic : HashSet<object>
        {
            private int _starIndex;
            private int _elementsNumber;
            private HashSet<string> _refusedToAdd;

            public DistinctIdentiesDic(int startIndex = 0, int elementsNumber = 0)
            {
                _starIndex = startIndex;
                _elementsNumber = elementsNumber;
                _refusedToAdd = new HashSet<string>();
            }
            
            public bool AddNew(object key)
            {
                if (base.Contains(key)) return true;
                if ((CanInsertNewKey() && !base.Contains(key)))
                {
                    base.Add(key);
                    return true;
                }
                _refusedToAdd.Add(key.ToString());
                return false;
            }

            private bool CanInsertNewKey()
            {
                if (_refusedToAdd.Count >= _starIndex)
                    return base.Count - _elementsNumber < 0;
                return false;
            }
        }
    }
}