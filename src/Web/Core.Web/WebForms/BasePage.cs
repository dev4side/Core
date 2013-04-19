using System;
using System.Web.UI;
using Core.Kernel;
using Core.Log;
using Core.Web.WebForms.Managers;
using Ninject;

namespace Core.Web.WebForms
{
    public abstract class BasePage : Page
    {
        #region Services

        [Inject]
        public ILog<BasePage> Log { get; set; }

        #endregion

        private UrlManager _urlManager;

        public UrlManager UrlManager
        {
            get { return _urlManager ?? (_urlManager = new UrlManager(this)); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ObjectFactory.ResolveDependencies(this);
        }
    }
}