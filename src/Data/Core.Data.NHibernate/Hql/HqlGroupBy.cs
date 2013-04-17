using System;

namespace Core.Data.NHibernate.Hql
{
    public class HqlGroupBy : IGroupBy
    {
        private string _requiredGroupBy;
        private bool _stopGroupBy;

        public HqlGroupBy(string groupByName, bool stopGroupBy)
        {
            _requiredGroupBy = groupByName;
            _stopGroupBy = stopGroupBy;
        }

        public string GetGroupBy()
        {
            return _requiredGroupBy;
        }

        public bool StopGroupBy
        {
            get { return _stopGroupBy; }
        }
    }
}
