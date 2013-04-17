using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data.NHibernate.Hql
{
    public class HqlJoin : IJoin
    {
        private string _requiredJoin;
        private readonly string[] _projectionNameSplits = new string[0];
        private string _rootEntityName;

        //public HqlJoin(string requiredJoin)
        //{
        //    _requiredJoin = requiredJoin;
        //    _projectionNameSplits = requiredJoin.Split('.');
        //}

        public HqlJoin(string rootEntitiyName, string requiredJoin)
        {
            _rootEntityName = rootEntitiyName;
            _requiredJoin = requiredJoin;
            _projectionNameSplits = requiredJoin.Split('.');
        }

        //public string[]  GetJoin()
        //{
        //    // left join WorkbookProtocol.PlannedContrastProtocols as PlannedContrastProtocols
        //    return String.Format("left join {0} as {1} ", _requiredJoin, );
        //}

        public int PriorityJoins
        {
            get { return _projectionNameSplits.Length; }
        }

        public bool IsRoot
        {
            get { return !(_requiredJoin.Contains(".")); }
        }
        
        public string[] GetJoin()
        {
            if (IsRoot)
                return new string[0];
            IList<string> joins = new List<string>();

            //first join is slightly different because the root entity name is not repeted in the "as" string
            var basejoin = string.Format("left join {0}.{1} as {1}", _rootEntityName, _projectionNameSplits[0]);
            joins.Add(basejoin);

            for (int i = 1; i < _projectionNameSplits.Length; i++)
            {
                if (i == _projectionNameSplits.Length - 1)
                    return joins.ToArray();
                AddJoinString(joins, i);
            }

            //if it gets here something went terribly wrong
            return joins.ToArray();


            //if (_projectionNameSplits.Length == 2)
            //    return joins.ToArray();
            //if (_projectionNameSplits.Length == 3)
            //{
            //    for (int i = 1; i < _projectionNameSplits.Length; i++)
            //    {
            //        if (i != _projectionNameSplits.Length - 1)
            //        {
            //            string additionalRequiredJoins = string.Format("left join {0}.{1} as {0}{1}", _projectionNameSplits[i - 1], _projectionNameSplits[i]);
            //            joins.Add(additionalRequiredJoins);
            //        }
            //    }
            //}
            //if (_projectionNameSplits.Length == 4)
            //{
            //    for (int i = 1; i < _projectionNameSplits.Length; i++)
            //    {
            //        if (i == 1)
            //        {
            //            string additionalRequiredJoins = string.Format("left join {0}.{1} as {0}{1}", _projectionNameSplits[i - 1], _projectionNameSplits[i]);
            //            joins.Add(additionalRequiredJoins);
            //        }
            //        if (i == 2)
            //        {
            //            string additionalRequiredJoins = string.Format("left join {0}{1}.{2} as {0}{1}{2}", _projectionNameSplits[i - 2], _projectionNameSplits[i - 1], _projectionNameSplits[i]);
            //            joins.Add(additionalRequiredJoins);
            //        }
            //    }
            //}
        }

        private void AddJoinString(IList<string> joins, int i)
        {
            string pointed = _projectionNameSplits[0];
            string straight = _projectionNameSplits[0];
            for (int j = 1; j <= i; j++)
            {
                if (j == i)
                {
                    pointed += ".";
                }
                pointed += _projectionNameSplits[j];
                straight += _projectionNameSplits[j];
            }
            joins.Add(String.Format("left join {0} as {1}", pointed, straight));
        }
    }
}
