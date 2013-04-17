using System.Text;

namespace Core.Services.Conversion
{
    public class MapToEntityPropertyDeclarationException : BaseConvertionException
    {
        private readonly string _message;

        private const string HEADER_MESSAGE_TEXT =
            "There is an error in the way you have mapped your DTO with MapToEntityProperty attribute! ";

        public MapToEntityPropertyDeclarationException(string message)
        {
            _message = message;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(HEADER_MESSAGE_TEXT);
                sb.AppendLine(_message);
                return sb.ToString();
            }
        }
    }
}
