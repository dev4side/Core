using System;
using Core.Kernel;
using Core.Log;
using Ninject;

namespace Core.Web.WebForms
{
    public class BaseMasterPage : System.Web.UI.MasterPage
    {
        [Inject]
        public ILog<BaseMasterPage> Log { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ObjectFactory.ResolveDependencies(this);
        }
    }
}