using System.Collections.Generic;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffIndex : XlsBiffRecord
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

		public uint FirstExistingRow
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(4);
				}
				return ReadUInt32(4);
			}
		}

		public uint LastExistingRow
		{
			get
			{
				if (!isV8)
				{
					return ReadUInt16(6);
				}
				return ReadUInt32(8);
			}
		}

		public uint[] DbCellAddresses
		{
			get
			{
				int recordSize = base.RecordSize;
				int num = (isV8 ? 16 : 12);
				if (recordSize <= num)
				{
					return new uint[0];
				}
				List<uint> list = new List<uint>((recordSize - num) / 4);
				for (int i = num; i < recordSize; i += 4)
				{
					list.Add(ReadUInt32(i));
				}
				return list.ToArray();
			}
		}

		internal XlsBiffIndex(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
