namespace Core.Common.Constants
{
    public class ExceptionMessagesConstants
    {
        /// <summary>
        /// Represents the System.String error message for disposed UnitOfWork.
        /// </summary>
        public const string UnitOfWorkDisposedMessage =
         "UnitOfWork have been disposed before the conversion has been created! Use The dtoEntityAdapter inside the UnitOfWork using statement, or dispose entities' unitOfWork after the conversion.";
    }
}
