using System;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;
using Ninject.Parameters;

namespace Core.Validation.AOP
{
    public class DtoValidatorAttribute : InterceptAttribute
    {
        private Type _typeToValidate;
        private const int ORDER = 3;

        public DtoValidatorAttribute(Type typeToValidate)
        {
            _typeToValidate = typeToValidate;
            this.Order = ORDER;
        }

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            var generic = typeof(DtoValidator<>);
            var specific = generic.MakeGenericType(_typeToValidate);
            return (IInterceptor)request.Context.Kernel.Get(specific, new IParameter[] { });
        }
    }
}
