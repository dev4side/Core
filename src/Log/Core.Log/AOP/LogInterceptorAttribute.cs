using System;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;
using Ninject.Parameters;

namespace Core.Log.AOP
{
    public class LogInterceptorAttribute : InterceptAttribute
    {
        private Type _loggerType;
        private const int ORDER = 1;

        public LogInterceptorAttribute(Type loggerType)
        {
            _loggerType = loggerType;
            this.Order = ORDER;
        }

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            var generic = typeof(LogInterceptor<>);
            var specific = generic.MakeGenericType(_loggerType);
            return (IInterceptor)request.Context.Kernel.Get(specific, new IParameter[] { });
        }
    }
}
