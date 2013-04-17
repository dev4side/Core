using Core.Services.Encoding.AOP;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;

namespace Core.Services.Encoding
{
    public class EncodingInterceptorAttribute : InterceptAttribute
    {
        // note: Interceptors are invoked in ascending order.
        private const int ORDER = 1;

        private EncodingType _encodingType;

        public EncodingInterceptorAttribute(EncodingType encodingType)
        {
            this.Order = ORDER;
            this._encodingType = encodingType;
        }

        #region InterceptAttribute Members

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            var encodingHandler = request.Context.Kernel.Get<EncodingHandler>();
            encodingHandler.EncodingType = this._encodingType;
            return encodingHandler;
        }

        #endregion
    }
}