using System;

namespace Core.Validation
{
    public sealed class ValidationResult
    {
        private ValidationMessages _messages;

        public bool Valid
        {
            get
            {
                if (_messages.Count > 0)
                    return false;
                return true;
            }
        }

        public ValidationResult()
        {
            _messages = new ValidationMessages();
        }

        public ValidationMessages Messages
        {
            get { return _messages; }
            internal set
            {
                _messages = value;
            }
        }

        /// <summary>
        /// Aggiunge un errore. 
        /// </summary>
        public void AddError(string errorMessage)
        {
            Messages.Add(new ValidationMessage { Message = errorMessage });
        }
        
        /// <summary>
        /// Aggiunge un errore. 
        /// </summary>
        public void AddError(params string[] stringToFormat)
        {
            AddError(BuildMessageFromParams(stringToFormat));
        }

        public string GetFullErrorMessage()
        {
            return Messages.GetFullErrorMessage();
        }

        public string GetFullErrorMessageInHtml()
        {
            return Messages.GetFullErrorMessageInHtml();
        }

        // codice copiato da Core.Log.Helper.LogFormatter
        //per non creare dipendenze inutili è stato solo copiato.
        private static string BuildMessageFromParams(params string[] message)
        {
            if (message != null)
                switch (message.Length)
                {
                    case 0:
                        return string.Empty;
                    case 1:
                        return message[0];
                    default:
                        return FormatLogMessage(message);
                }
            throw  new NullReferenceException("Please provide a message in for the validator");
        }

        private static string FormatLogMessage(string[] message)
        {
            var formatString = message[0];
            var stringParams = new string[message.Length - 1];
            for (var i = 1; i < message.Length; i++)
                stringParams[i - 1] = message[i];
            return String.Format(formatString, stringParams);
        }

        public void Merge(ValidationResult validationResult)
        {
            if (validationResult == null)
                return;
            foreach (var validationMessage in validationResult.Messages)
            {
                Messages.Add(validationMessage);
            }
        }
        
        public void Merge(ValidationResult validationResult, string prefixMessgae)
        {
            if (validationResult == null)
                return;
            foreach (var validationMessage in validationResult.Messages)
            {
                Messages.Add(new ValidationMessage(){ Message = String.Format("{0}: {1}", prefixMessgae, validationMessage.Message)});
            }
        }
    }
}

