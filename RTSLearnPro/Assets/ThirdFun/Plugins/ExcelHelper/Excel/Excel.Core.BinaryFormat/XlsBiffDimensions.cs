using System;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffDimensions : XlsBiffRecord
	{
		private bool isV8 = true;

		public bool IsV8
		{
			get
			{
				return isV8;
			}
			set
			{
				isV8 = value;
			}
		}

		public uint FirstRow
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(0);
				}
				return ReadUInt32(0);
			}
		}

		public uint LastRow
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(2);
				}
				return ReadUInt32(4);
			}
		}

		public ushort FirstColumn
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(4);
				}
				return ReadUInt16(8);
			}
		}

		public ushort LastColumn
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(6);
				}
				return (ushort)((ReadUInt16(9) >> 8) + 1);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal XlsBiffDimensions(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
