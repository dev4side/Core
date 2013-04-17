using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Exceptions.Hql;

namespace Core.Data.NHibernate.Hql
{
    public class HqlProjection : IProjection
    {
        private readonly IEnumerable<string> _supportedClauses = new string[] {"count", "min", "max"};
        private readonly string _projectionClause;
        private readonly string _projectionName;
        private readonly string _projectionAlias;
        private readonly string _rootEntityName;
        private readonly string[] _projectionNameSplits = new string[0];
        IJoin reuquiredJoin;
        IGroupBy requiredGroupBy;

        public HqlProjection(Type type, string projectionName)
        {
            _rootEntityName = type.Name;
            _projectionName = projectionName;

            if (_projectionName.Contains("."))
            {
                _projectionNameSplits = _projectionName.Split('.');
                _projectionAlias = _projectionNameSplits[_projectionNameSplits.Length - 1];
            }
            reuquiredJoin = new HqlJoin(_rootEntityName, _projectionName);
            requiredGroupBy = new HqlGroupBy(GetProjectionName(), false);
        }

        public HqlProjection(Type type, string projectionName, string projectionAlias)
        {
            _rootEntityName = type.Name;
            _projectionName = projectionName;
            _projectionAlias = projectionAlias;
            if (_projectionName.Contains("."))
            {
                _projectionNameSplits = _projectionName.Split('.');
            }
            reuquiredJoin = new HqlJoin(_rootEntityName, _projectionName);
            requiredGroupBy = new HqlGroupBy(GetProjectionName(), false);
        }

        public HqlProjection(Type type, string projectionClause, string projectionName, string projectionAlias)
        {
            if(!_supportedClauses.Contains(projectionClause.ToLower()))
                throw new HqlException(string.Format("The clause '{0}' is not supported.", projectionClause));

            _projectionClause = projectionClause;
            _rootEntityName = type.Name;
            _projectionName = projectionName;
            _projectionAlias = projectionAlias;
            if (_projectionName.Contains("."))
            {
                _projectionNameSplits = _projectionName.Split('.');
            }
            reuquiredJoin = new HqlJoin(_rootEntityName, _projectionName);
            requiredGroupBy = new HqlGroupBy(GetProjectionName(), true);
        }

        public IJoin Join
        {
            get { return reuquiredJoin; }
        }

        public IGroupBy GroupBy
        {
            get { return requiredGroupBy; }
        }

        public string ProjectionName
        {
            get
            {
                var projectionName = GetProjectionName();
                return string.IsNullOrEmpty(_projectionClause) ? projectionName : string.Format("{0}({1})", _projectionClause, projectionName);
            }
        }

        private string GetProjectionName()
        {
            if (IsRootProjection)
                return String.Concat(_rootEntityName, ".", _projectionName);
            if (_projectionNameSplits.Length == 2)
                return _projectionName;

            //we add the projection name parts one by one
            string projectionString = _projectionNameSplits[0];
            for (int i = 1; i < _projectionNameSplits.Length; i++)
            {
                projectionString = AddPartToProjectionName(projectionString, i);
            }
            return projectionString;

            //if (_projectionNameSplits.Length == 3)
            //    return String.Format("{0}{1}.{2}", _projectionNameSplits[_projectionNameSplits.Length - 3], _projectionNameSplits[_projectionNameSplits.Length - 2], _projectionNameSplits[_projectionNameSplits.Length - 1]);
            //if (_projectionNameSplits.Length == 4)
            //    return String.Format("{0}{1}{2}.{3}", _projectionNameSplits[_projectionNameSplits.Length - 4], _projectionNameSplits[_projectionNameSplits.Length - 3], _projectionNameSplits[_projectionNameSplits.Length - 2], _projectionNameSplits[_projectionNameSplits.Length - 1]);
            //return String.Format("{0}.{1}", _projectionNameSplits[_projectionNameSplits.Length - 2], _projectionNameSplits[_projectionNameSplits.Length - 1]);
        }

        private string AddPartToProjectionName(string projectionString, int i)
        {
            //the '.' char is added before the last part
            if (i == (_projectionNameSplits.Length - 1))
                projectionString += ".";
            projectionString += _projectionNameSplits[i];
            return projectionString;
        }

        public string ProjectionAlias
        {
            get { return _projectionAlias; }
        }

        private bool IsRootProjection
        {
            get { return !(_projectionName.Contains(".")); }
        }

        //public int PriorityJoins
        //{
        //    get { return _projectionNameSplits.Length; }
        ////}

        //public string[] RequiredJoins()
        //{
        //    if (IsRootProjection)
        //        return new string[0];
        //    IList<string> joins = new List<string>();

        //    var basejoin = string.Format("left join {0}.{1} as {1}", _rootEntityName, _projectionNameSplits[0]);
        //    joins.Add(basejoin);
        //    if (_projectionNameSplits.Length == 2)
        //        return joins.ToArray();

        //    for (int i = 1; i < _projectionNameSplits.Length; i++)
        //    {
        //        if (i != _projectionNameSplits.Length - 1)
        //        {
        //            string additionalRequiredJoins = string.Format("left join {0}.{1} as {1}", _projectionNameSplits[i - 1], _projectionNameSplits[i]);
        //            joins.Add(additionalRequiredJoins);
        //        }
        //    }
        //    return joins.ToArray();
        //} 
    }
}
