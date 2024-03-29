﻿using System;
using System.Collections.Generic;
using Core.Validation.Exceptions;
using Ninject.Extensions.Interception;

namespace Core.Validation.AOP
{
    public class DtoValidator<T> : SimpleInterceptor
    {
        protected override void BeforeInvoke(IInvocation invocation)
        {            
            var arugments = invocation.Request.Arguments;
            FindAndValidateArgument(arugments);
        }

        private static void FindAndValidateArgument(IEnumerable<object> arugments)
        {
            if (arugments == null)
            {
                throw new ArgumentNullException("arugments");
            }

            Type typeOfT = typeof (T);
            
            foreach (var obj in arugments)
            {
                if (typeOfT.IsInstanceOfType(obj))
                {
                    var validationResult = ValidationEngine.TryValidate((T) obj);
                    
                    if (validationResult != null)
                    {
                        if (!validationResult.Valid)
                        {
                            throw new ValidationException(validationResult);
                        }
                    }
                }
            }
        }
    }
}
