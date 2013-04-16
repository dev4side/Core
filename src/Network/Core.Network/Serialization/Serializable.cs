using System;
using System.Runtime.Serialization;
using System.Security.Permissions;


namespace Core.Network.Serialization
{

	[Serializable]
	public class Serializable<T> : IEquatable<Serializable<T>>, ISerializable where T : class
	{

		private T _originalValue;
		private bool _hasSerializableAttr;
		private bool _implementsISerializable;
		private bool _implementsIEquatable;

		public Serializable(T o)
		{
			_originalValue = o;
			_hasSerializableAttr = Attribute.GetCustomAttribute(typeof(T), typeof(SerializableAttribute)) != null;
			_implementsISerializable = typeof(T).GetInterface(typeof(ISerializable).FullName) != null;
			_implementsIEquatable = typeof(T).GetInterface(typeof(IEquatable<T>).FullName) != null;
			if (!_hasSerializableAttr && !_implementsISerializable)
				throw new ArgumentException("SerializableObject<T>: Type passed as generic is not serializable");
		}

		public Serializable(SerializationInfo info, StreamingContext context)
		{
			_hasSerializableAttr = Attribute.GetCustomAttribute(typeof(T), typeof(SerializableAttribute)) != null;
			_implementsISerializable = typeof(T).GetInterface(typeof(ISerializable).FullName) != null;
			_implementsIEquatable = typeof(T).GetInterface(typeof(IEquatable<T>).FullName) != null;

			//deserialization..
			if (_hasSerializableAttr)
			{
				try
				{
					_originalValue = (T)info.GetValue("orig", typeof(T));
				}
				catch (InvalidCastException ex)
				{
					throw new InvalidCastException("SerializableObject<T>: Wrong type passed as generic", ex);
				}
				//catch (SerializationException ex) { }	//da lasciar passare..
			}
		}


		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (_hasSerializableAttr)
			{
				info.AddValue("orig", _originalValue, typeof(T));
			}
			else if (_implementsISerializable)
			{
				((ISerializable)_originalValue).GetObjectData(info, context);
			}
		}

		public bool Equals(Serializable<T> other)
		{
			return (((object)other) != null) && (_originalValue.Equals(other.Value));
		}

		public override bool Equals(Object other)
		{
			return (other != null) && (other is Serializable<T>) && (_originalValue.Equals(((Serializable<T>)other).Value));
		}

		public override int GetHashCode()
		{
			return _originalValue.GetHashCode();
		}

		public static bool operator ==(Serializable<T> left, Serializable<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Serializable<T> left, Serializable<T> right)
		{
			return !(left.Equals(right));
		}

		public T Value
		{
			get { return _originalValue; }
		}

	}


	public static class SerializableExtension
	{

		public static Serializable<T> GetSerializable<T>(this T o) where T : class
		{
			return new Serializable<T>(o);
		}
	}

}
