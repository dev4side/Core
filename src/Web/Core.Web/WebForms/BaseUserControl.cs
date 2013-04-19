using System;
using System.Web.UI;
using Core.Kernel;
using Core.Log;
using Ninject;

namespace Core.Web.WebForms
{
    public class BaseUserControl : UserControl
    {
        [Inject]
        public ILog<BasePage> Log { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ObjectFactory.ResolveDependencies(this);
        }

        public void RegisterScriptBlock(string key, string script)
        {
            PageClientScriptManager.RegisterClientScriptBlock(this.GetType(), key, script, true);
        }

        public void RegisterClientScriptInclude(string key, string script)
        {
            PageClientScriptManager.RegisterClientScriptInclude(this.GetType(), key, script);
        }

        public void RegisterClientScriptResource(string resourceName)
        {
            PageClientScriptManager.RegisterClientScriptResource(this.GetType(), resourceName);
        }

        private ClientScriptManager PageClientScriptManager
        {
            get { return Page.ClientScript; }
        }
    }
}