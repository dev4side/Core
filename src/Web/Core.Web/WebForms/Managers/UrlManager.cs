using System;

namespace Core.Web.WebForms.Managers
{
    public class UrlManager
    {
        private readonly BasePage _page;
        
        public UrlManager(BasePage page)
        {
            _page = page;
        }

        public bool TryGetQueryString(string key, out string value)
        {
            value = _page.Request.QueryString[key];
            return !String.IsNullOrEmpty(value);
        }

        public bool TryGetGuidQueryString(string key, out Guid id)
        {
            id = new Guid();
            string candidate;
            if (!TryGetQueryString(key, out candidate))
                return false;
            return Guid.TryParse(candidate, out id);
        }

        //throws ArgumentNullException if querystring is null (no key in the queystring) and FormatException if the Guid is not valid
        public Guid GetGuidQueryString(string key)
        {
            return Guid.Parse(_page.Request.QueryString[key]);
        }

        public bool TryGetRoute(string key, out string value)
        {
            if (_page.RouteData.Values.ContainsKey(key))
            {
                value = _page.RouteData.Values[key].ToString();
                return true;
            }
            value = String.Empty;
            return false;
        }

        public bool TryGetGuidRoute(string key, out Guid id)
        {
            id = new Guid();
            if (_page.RouteData.Values.ContainsKey(key))
                return Guid.TryParse(_page.RouteData.Values[key].ToString(), out id);
            return false;
        }

        //throws NullReferenceException if route data is null and FormatException if the Guid is not valid
        public Guid GetGuidRoute(string key)
        {
            return Guid.Parse(_page.RouteData.Values[key].ToString());
        }

        public bool TryGetGuidQueryStringIfEntityExist(string key, Func<Guid, bool> getElementById, out Guid id)
        {
            if (TryGetGuidQueryString(key, out id))
            {
                return TryEntityExists(getElementById, id);
            }
            return false;
        }

        public bool TryGetGuidRouteIfEntityExist(string key, Func<Guid, bool> getElementById, out Guid id)
        {
            if (TryGetGuidRoute(key, out id))
            {
                return TryEntityExists(getElementById, id);
            }
            return false;
        }

        private static bool TryEntityExists(Func<Guid, bool> getElementById, Guid id)
        {
            try
            {
                return getElementById.Invoke(id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}