using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.IO
{
	public class CRC
	{
		private static bool _lookupInitialized = false;
		private static readonly int[] _xorLookup = new int[256];

		public ushort Hash;

		public CRC(ushort value)
		{
			if (!_lookupInitialized)
				InitLookupTable();
			Hash = value;
		}

		public CRC() : this(0) { }

		public CRC(CRC crc) : this(crc.Hash) { }

		public CRC(byte[] buffer, int offset, int size) : this(GetCRC16(buffer,offset,size)) { }

		private void InitLookupTable()
		{
			int crc;

			for (int i = 0; i < 256; i++)
			{
				crc = i;
				for (int j = 0; j < 8; j++)
				{
					if ((crc & 0x0001) != 0)
					{
						crc = (crc >> 1) ^ 0xA001;
					}
					else
					{
						crc = crc >> 1;
					}
				}
				_xorLookup[i] = crc;
			}

			_lookupInitialized = true;
		}

		public static CRC GetCRC16(byte[] buffer, int offset, int size)
		{
			if (size <= 0)
				return new CRC();

			ushort crc = 0;

			unsafe
			{
				fixed(byte* buf = &buffer[offset])
				{
					byte* ptr = buf;

					while(size > 0)
					{
						//TODO: controllare correttezza..
						int tmp = crc ^ (*ptr & 0xFF);
						crc = (ushort) ((crc >> 8) ^ _xorLookup[tmp & 0xFF]);

						ptr++;
						size--;
					}
				}
			}
			return new CRC(crc);
		}

		//[DllImport("CRCCalc.dll", CharSet = CharSet.Auto, EntryPoint = "fnCRCCalc")]
		//private static extern int fnCRCCalc(int buf, int len);

		//public static short CalculateCRC(byte[] buffer, int length)
		//{
		//    int crc = 0;
		//    unsafe
		//    {
		//        fixed (byte* rb = buffer)
		//        {
		//            IntPtr bufPtr = new IntPtr(rb);
		//            crc = fnCRCCalc(bufPtr.ToInt32(), length);
		//        }
		//    }
		//    return (short)crc;
		//}

	}
}
