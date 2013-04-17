using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;

namespace Core.Services.Consinstency
{
    public class WcfExceptionInterceptorAttribute: InterceptAttribute
    {
        // note: Interceptors are invoked in ascending order.
        private const int ORDER = 0;

        public WcfExceptionInterceptorAttribute()
        {
            this.Order = ORDER;
        }

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            return request.Context.Kernel.Get<LastChanceWcfExceptionHandler>();
        }
    }
}
