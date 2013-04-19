using System;
using System.ServiceModel;
using System.Text;
using Core.Business.Exceptions;
using Core.Kernel;
using Core.Log;
using Core.Validation.Exceptions;
using Ninject.Extensions.Interception;

namespace Core.Services.Consinstency.AOP
{
    public class LastChanceWcfExceptionHandler : IInterceptor
    {
        private readonly ILog<LastChanceWcfExceptionHandler> _log;

        public LastChanceWcfExceptionHandler()
        {
            _log = ObjectFactory.Get<ILog<LastChanceWcfExceptionHandler>>();
        }

        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (MessageException ext)
            {
                ThrowFaultException(invocation, ext);
            }
            catch (ManagerException ex)
            {
                ThrowFaultException(invocation, new ManagerException(ex.Message));
            }
            catch (ValidationException ex)
            {
                ThrowValidationExceptionAsFaultException(invocation, ex);
            }
            catch (Exception ex)
            {
                // loggo solo le Exception gravi, le altre sono gestite
                _log.Error(ex, "[###Important###] A LastChance WCF Exception has been throw on service {0}.{1}",
                           invocation.Request.Target.GetType().Name, invocation.Request.Method.Name);
                
                ThrowUnexpectedErrorFaultException(invocation, ex);
            }
        }

        #endregion

        private void ThrowValidationExceptionAsFaultException(IInvocation invocation, ValidationException ex)
        {
            var sb = new StringBuilder();
            sb.Append(ex.MessageInHtml);
            throw new FaultException(sb.ToString(), new FaultCode("service fault"));
        }

        private static void ThrowUnexpectedErrorFaultException(IInvocation invocation, Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append("Unexpected error. Please contact the administrator. Details: ");
            sb.Append(ex.Message);
            throw new FaultException(sb.ToString(), new FaultCode("service fault"));
        }

        private static void ThrowFaultException(IInvocation invocation, Exception ex)
        {
            throw new FaultException(ex.Message, new FaultCode("message"));
        }
    }
}