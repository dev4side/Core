using System;

namespace Core.Services.Conversion
{
    /// <summary>
    /// eccezzione usata dalle classi che implementano IConvertCommand
    /// </summary>
    public class ConvertCommandException : BaseConvertionException
    {
        private Type _type;
        private readonly object _tryiedObject;
        private readonly string _customMessage;

        public ConvertCommandException(Type converterCommand, object tryiedObject)
        {
            _type = converterCommand;
            _tryiedObject = tryiedObject;
        }


        public ConvertCommandException(Type converterCommand, object tryiedObject, string customMessage)
        {
            _type = converterCommand;
            _tryiedObject = tryiedObject;
            _customMessage = customMessage;
        }

        public override string Message
        {
            get
            {
                if (_customMessage == null)
                    return String.Format("Cannot use {0} for converting the following value: {1}", _type, _tryiedObject);
                return String.Format("Cannot use {0} for converting the following value: {1}. Additional message: {2}", _type, _tryiedObject, _customMessage); 
            }
        }

      
    }

}
