using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.NHibernate.Hql
{
    public class HqlRestriction : IRestriction
    {
        private string _restriction;
        private string _typeName;
        IJoin _requiredJoin;

        public HqlRestriction(Type type, string restriction)
        {
            _typeName = type.Name;
            _restriction = restriction;
            _requiredJoin = FormatJoin(_typeName, _restriction);
        }

        public string GetRestriction()
        {
            return _restriction;
        }
        
        public IJoin Join
        {
            get { return _requiredJoin; }
        }

        private HqlJoin FormatJoin(string typeName, string originalRestriction)
        {
            var restriction = RemoveHqlClauses(originalRestriction);
            var splitRestrictions = restriction.Split('.');
            var splittedRestrictions = splitRestrictions.Where((t, i) => (i != 0 || t != typeName)).ToList();
            var requiredJoin = ConcatWithChar(splittedRestrictions, '.');
            return new HqlJoin(_typeName, requiredJoin);
        }

        private string RemoveHqlClauses(string originalRestriction)
        {
            var restriction = originalRestriction.Replace("(", string.Empty).Replace(")", string.Empty);
            if (restriction.Contains(" or "))
            {
                var restrictionLenght = restriction.IndexOf(" or ", StringComparison.InvariantCulture);
                restriction = restriction.Substring(0, restrictionLenght);
            }
            if (restriction.Contains(" and "))
            {
                var restrictionLenght = restriction.IndexOf(" and ", StringComparison.InvariantCulture);
                restriction = restriction.Substring(0, restrictionLenght);
            }
            if (restriction.Contains(" like "))
            {
                var restrictionLenght = restriction.IndexOf(" like ", StringComparison.InvariantCulture);
                restriction = restriction.Substring(0, restrictionLenght);
            }
            if (restriction.Contains(".size"))
            {
                var restrictionLenght = restriction.IndexOf(".size", StringComparison.InvariantCulture);
                restriction = restriction.Substring(0, restrictionLenght);
            }
            return restriction.Replace(" ", string.Empty);
        }

        private static string ConcatWithChar(IEnumerable<string> stringsToConcat, char separatorChar)
        {
            var requiredJoin = new StringBuilder();

            foreach (var stringToConcat in stringsToConcat)
            {
                if (!string.IsNullOrEmpty(requiredJoin.ToString()))
                    requiredJoin.Append(separatorChar);
                requiredJoin.Append(stringToConcat);
            }

            return requiredJoin.ToString();
        }
    }
}
