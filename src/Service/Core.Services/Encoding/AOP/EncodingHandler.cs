using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Core.Kernel;
using Core.Log;
using Core.Services.Encoding.Attributes;
using Ninject.Extensions.Interception;

namespace Core.Services.Encoding.AOP
{
    public class EncodingHandler : SimpleInterceptor
    {
        private readonly ILog<EncodingHandler> _log;

        public EncodingType EncodingType { get; set; }

        public EncodingHandler()
        {
            _log = ObjectFactory.Get<ILog<EncodingHandler>>();
        }

        protected override void BeforeInvoke(IInvocation invocation)
        {
            if (EncodingType != EncodingType.Encode)
            {
                return;
            }
                
            object[] arguments = invocation.Request.Arguments;

            foreach (object argument in arguments.Where(x => x != null))
            {
                EncodeDecodeEntity(argument, EncodingType);
            }
        }

        protected override void AfterInvoke(IInvocation invocation)
        {
            if (EncodingType != EncodingType.Decode)
            {
                return;
            }

            object returnValue = invocation.ReturnValue;
            EncodeDecodeEntity(returnValue, EncodingType);
        }

        private static void EncodeDecodeEntity(object entityToEncode, EncodingType encodingType)
        {
            Type entityType = entityToEncode.GetType();
            PropertyInfo[] props = entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                if (!propertyInfo.GetCustomAttributes(true).Contains(new EncodeHtmlAttribute()))
                {
                    continue;
                }

                object propertyValue = propertyInfo.GetValue(entityToEncode, null);
                string encodedPropertyValue = string.Empty;

                switch (encodingType)
                {
                    case EncodingType.Encode:
                        encodedPropertyValue = HttpUtility.HtmlEncode(string.Format("{0}", propertyValue));
                        break;
                    case EncodingType.Decode:
                        encodedPropertyValue = HttpUtility.HtmlDecode(string.Format("{0}", propertyValue));
                        break;
                }

                propertyInfo.SetValue(entityToEncode, encodedPropertyValue, null);
            }
        }
    }
}