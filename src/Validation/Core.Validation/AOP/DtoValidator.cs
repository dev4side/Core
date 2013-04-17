using System;
using Ninject.Extensions.Interception;

namespace Core.Validation
{
    public class DtoValidator<T>: SimpleInterceptor
    {
        protected override void BeforeInvoke(IInvocation invocation)
        {            
            var arugments = invocation.Request.Arguments;
            FindAndValidateArgument(arugments);
        }

        private static void FindAndValidateArgument(object[] arugments)
        {
            Type typeOfT = typeof (T);
            foreach (var obj in arugments)
            {
                if (typeOfT.IsInstanceOfType(obj))
                {
                    var validationResult = ValidationEngine.TryValidate((T) obj);
                    if (validationResult != null)
                        if (!validationResult.Valid)
                            throw new ValidationException(validationResult);
                }
            }
        }
    }
}
