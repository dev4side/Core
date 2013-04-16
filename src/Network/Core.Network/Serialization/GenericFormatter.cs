using System.IO;
using System.Runtime.Serialization;

namespace Core.Network.Serialization
{
	public class GenericFormatter<TFormatterType>
		where TFormatterType : IFormatter, new()
	{
		public TFormatterType Formatter;

		public GenericFormatter()
		{
			Formatter = new TFormatterType();
		}
		public void Serialize<TObjectType>(Stream serializationStream, TObjectType serializableObject)
			where TObjectType : class, ISerializable
		{
			Formatter.Serialize(serializationStream, serializableObject);
		}
		public TObjectType Deserialize<TObjectType>(Stream serializationStream)
			where TObjectType : class, ISerializable
		{
			return Formatter.Deserialize(serializationStream) as TObjectType;
		}
	}
}
