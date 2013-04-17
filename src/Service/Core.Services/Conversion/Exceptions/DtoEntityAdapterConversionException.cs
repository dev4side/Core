using System;

namespace Core.Services.Conversion
{
    /// <summary>
    /// Eccezzione usata dall' adapeter durante le conversioni tra entita e dto
    /// </summary>
    public class DtoEntityAdapterConversionException : BaseConvertionException
    {
        private readonly Type _dtoType;
        private readonly Type _entityType;
        private readonly string _baseMassege;

        public DtoEntityAdapterConversionException(Type dtoType, Type entityType, string message) : base()
        {
            _dtoType = dtoType;
            _entityType = entityType;
            _baseMassege = String.Format("Cannot create conversion between  entity type {0} and dto type {1}. More details: ", _entityType, _dtoType);
            _baseMassege += message;
        }

        public override string Message
        {
            get
            {
                return _baseMassege;
            }
        }

        
    }
}
