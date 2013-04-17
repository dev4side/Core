using System;
using System.Reflection;

namespace Core.Services.Conversion
{
   
    public class ObjectGuidConverter : IConvertCommand
    {
        public object ConvertToDto(object toConvert, Type requestedType)
        {
            try
            {
                var result = toConvert.GetType().InvokeMember("Id", BindingFlags.GetProperty, null, toConvert, null);
                return result;
            }
            catch (Exception ex)
            {
                throw new ConvertCommandException(toConvert.GetType(), toConvert, ex.Message);
            }
        }

        public object ConvertToEntity(object toConvert, Type requestedType)
        {
            if (toConvert.GetType() != typeof (Guid))
                throw new ConvertCommandException(toConvert.GetType(), toConvert, String.Format("Guid is expected but {0} was provided", requestedType.GetType()));

            // se il guid è 0 vuol dire che non è ce nessuna relazione e per questo motivo bisogna tornare un oggetto nullo
            // far tornare un ogetto istanziato con l 'id = 0, comporta da parte di nhibernate l' eccezione di tipo
            //NHibernate.TransientObjectException : object references an unsaved transient instance - save the transient instance before flushing.
            // questo avviene perchè si crea una nuova relazione che per nhibernate non è valida!
            if ((Guid)toConvert == new Guid())
                return null;
           
            try
            {
                var result = Activator.CreateInstance(requestedType);
                result.GetType().InvokeMember("Id", BindingFlags.SetProperty, null, result, new[] {toConvert});
                return result;
            }
            catch (Exception ex)
            {
                throw new ConvertCommandException(toConvert.GetType(), toConvert, ex.Message);
            }
            
        }
    }
}
