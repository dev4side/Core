﻿namespace Core.Common.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionMessagesConstants
    {
        public const string UNIT_OF_WORK_DISPOSED_MESSAGE =
         "UnitOfWork have been disposed before the conversion has been created! Use The dtoEntityAdapter inside the UnitOfWork using statement, or dispose entities' unitOfWork after the conversion";
    }
}