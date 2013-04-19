using System;

namespace Core.IO
{
	public class CRCElement16 : BaseElement<CRC>
	{
		public IElement TargetElement { get; set; }
		public BufferDirection ElementDirection = BufferDirection.Backward;
		private CRC _referenceCRC = new CRC();

		public CRCElement16(string elementName, int bufferPosition)
			: base(elementName, bufferPosition, 2) { Value = new CRC(); }

		public override void UpdateValueFromBuffer(byte[] buffer)
		{
			if (TargetElement == null)
				return;
			//targetElement MUST be updated first!..
			_referenceCRC.Hash = CRC.GetCRC16(buffer, TargetElement.Position, TargetElement.Size).Hash;

			//simply read and store existing CRC from buffer..
			Value.Hash = BufferHelper.GetUshort(buffer[Position], buffer[Position + 1], ElementDirection);
		}

		public override void UpdateBufferFromValue(byte[] buffer)
		{
			if (TargetElement == null)
				return;

			//update Value first and leave dirty job to base..
			Value.Hash = _referenceCRC.Hash = CRC.GetCRC16(buffer, TargetElement.Position, TargetElement.Size).Hash;
			buffer[Position] = BufferHelper.GetMSB(Value.Hash, ElementDirection);
			buffer[Position + 1] = BufferHelper.GetLSB(Value.Hash, ElementDirection);
		}

		public override bool IsValid()
		{
			return (TargetElement != null) && (_referenceCRC.Hash == Value.Hash);
		}
	}
}
