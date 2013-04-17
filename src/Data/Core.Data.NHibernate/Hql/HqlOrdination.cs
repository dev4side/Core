using System;

namespace Core.Data.NHibernate.Hql
{
    public class HqlOrdination : IOrdination
    {
        private string _ordinationName;
        private string _ordinationType;

        public HqlOrdination(string ordinationName, string ordinationType)
        {
            _ordinationName = ordinationName;
            _ordinationType = ordinationType;
        }

        public string GetOrdination()
        {
            return string.Format("{0} {1}", _ordinationName, _ordinationType);
        }
    }
}
