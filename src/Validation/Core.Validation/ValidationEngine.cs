using System;
using System.Collections.Generic;
using Core.Kernel;

namespace Core.Validation
{
    public static class ValidationEngine
    {
        /// <summary>
        /// Prova a validare un oggetto. Se non è presnete nessuna regola di validazione torna null
        /// </summary>
        public static ValidationResult TryValidate<T>(T objectToValidate)
        {
            var validator = ObjectFactory.TryGet<IValidator<T>>();
            //se non è possibile validare l'oggetto
            if (validator == null)
                return null;
            try
            {
                return validator.Validate(objectToValidate);
            }
            catch (Exception ex)
            {
                return CreateValidationResultFromException(objectToValidate, ex);
            }
        }

        /// <summary>
        /// Valida un oggetto secondo le regole definite
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToValidate"></param>
        /// <returns></returns>
        public static ValidationResult Validate<T>(T objectToValidate)
        {
            try
            {
                var validator = ObjectFactory.Get<IValidator<T>>();
                return validator.Validate(objectToValidate);
            }
            catch (Exception ex)
            {
                return CreateValidationResultFromException(objectToValidate, ex);
            }
        }


        private static IEnumerable<ValidationMessage> FlattenError(Exception exception)
        {
            var messages = new List<ValidationMessage>();
            Exception currentException = exception;
            do
            {
                messages.Add(new ValidationMessage {Message = exception.Message});
                currentException = currentException.InnerException;
            } while (currentException != null);

            return messages;
        }


        private static ValidationResult CreateValidationResultFromException(object objectToValidate, Exception ex)
        {
            var messages = new ValidationMessages
                               {
                                   new ValidationMessage
                                       {
                                           Message = string.Format("Error validating {0}", objectToValidate)
                                       }
                               };
            messages.AddRange(FlattenError(ex));
            var result = new ValidationResult {Messages = messages};
            return result;
        }
    }
}