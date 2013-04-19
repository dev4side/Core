using System;

namespace Core.Validation.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; private set; }  

        public ValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public override string Message
        {
            get { return ValidationResult.GetFullErrorMessage(); }
        }

        public  string MessageInHtml
        {
            get { return ValidationResult.GetFullErrorMessageInHtml(); }
        }
    }
}
