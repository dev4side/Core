using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Core.IO
{
	public enum BufferDirection
	{
		Forward,
		Backward
	}

	//TODO:
	public enum BufferEndianess
	{
		BigEndian,
		LittleEndian
	}

	public static class BufferHelper
	{
		public static BufferDirection DefaultDirection = BufferDirection.Forward;
		public static BufferEndianess DefaultEndianess = BufferEndianess.LittleEndian;//TODO
		public static string DefaultStringEncoding = "us-ascii";

		public static byte GetMSB(short val, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (byte)((val >> 8) & 0xFF);
				case BufferDirection.Backward:
					return (byte)(val & 0xFF);
				default:
					throw new NotSupportedException();
			}
			
		}

		public static byte GetLSB(short val, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (byte)(val & 0xFF);
				case BufferDirection.Backward:
					return (byte)((val >> 8) & 0xFF);
				default:
					throw new NotSupportedException();
			}
		}

		public static short GetShort(byte msb, byte lsb, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (short)((msb << 8) | lsb);
				case BufferDirection.Backward:
					return (short)((lsb << 8) | msb);
				default:
					throw new NotSupportedException();
			}
			
		}

		public static byte GetMSB(ushort val, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (byte)((val >> 8) & 0xFF);
				case BufferDirection.Backward:
					return (byte)(val & 0xFF);
				default:
					throw new NotSupportedException();
			}

		}

		public static byte GetLSB(ushort val, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (byte)(val & 0xFF);
				case BufferDirection.Backward:
					return (byte)((val >> 8) & 0xFF);
				default:
					throw new NotSupportedException();
			}
		}

		public static ushort GetUshort(byte msb, byte lsb, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (ushort)((msb << 8) | lsb);
				case BufferDirection.Backward:
					return (ushort)((lsb << 8) | msb);
				default:
					throw new NotSupportedException();
			}

		}

		public static int GetInt(byte msb4, byte msb3, byte lsb2, byte lsb1, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			switch (direction)
			{
				case BufferDirection.Forward:
					return (((((msb4 << 8) | msb3) << 8) | lsb2) << 8) | lsb1;
				case BufferDirection.Backward:
					return (((((lsb1 << 8) | lsb2) << 8) | msb3) << 8) | msb4;
				default:
					throw new NotSupportedException();
			}
		}

		public static byte[] GetBytes(int val, BufferDirection direction = BufferDirection.Forward, BufferEndianess endianess = BufferEndianess.LittleEndian)
		{
			var ret = new byte[4];
			switch (direction)
			{
				case BufferDirection.Forward:
					ret[3] = (byte) (val & 0xFF);
					ret[2] = (byte)((val >> 8) & 0xFF);
					ret[1] = (byte)((val >> 16) & 0xFF);
					ret[0] = (byte)((val >> 24) & 0xFF);
					break;
				case BufferDirection.Backward:
					ret[0] = (byte) (val & 0xFF);
					ret[1] = (byte)((val >> 8) & 0xFF);
					ret[2] = (byte)((val >> 16) & 0xFF);
					ret[3] = (byte)((val >> 24) & 0xFF);
					break;
				default:
					throw new NotSupportedException();
			}
			return ret;
		}

		public static int RawFind(byte[] pattern, byte[] buffer, int bufferOffset, int bufferCount, int module = 1)
		{
			if (buffer.Length < (bufferOffset + bufferCount))
				throw new ArgumentException("No enough bytes in buffer as specified by other parameters");

			int i, j, patternCount = pattern.Length, bufferLimit = bufferOffset + bufferCount - patternCount;
			for (j = bufferOffset; j <= bufferLimit; j += module)
			{
				for (i = 0; (i < patternCount) && (pattern[i] == buffer[i + j]); i++);
				if (i >= patternCount)
				{
					return j;
				}
			}
			return -1;
		}

		public static byte[] CutUntil(byte[] pattern, byte[] buffer, int bufferOffset, int bufferCount, int module = 1, bool includePattern = false)
		{
			int patternIndex = RawFind(pattern, buffer, bufferOffset, bufferCount, module);

			int substringCount = patternIndex != -1 ? patternIndex : bufferCount;
			Byte[] substring = new Byte[substringCount + (includePattern ? pattern.Length : 0)];

			Buffer.BlockCopy(buffer, bufferOffset, substring, 0, substringCount);
			if(includePattern)
				Buffer.BlockCopy(pattern, 0, substring, substringCount, pattern.Length);
			
			return substring;
		}

	}
}
