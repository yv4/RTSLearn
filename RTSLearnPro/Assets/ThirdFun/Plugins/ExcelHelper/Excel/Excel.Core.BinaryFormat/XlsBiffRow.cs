namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffRow : XlsBiffRecord
	{
		public ushort RowIndex
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		public ushort FirstDefinedColumn
		{
			get
			{
				return ReadUInt16(2);
			}
		}

		public ushort LastDefinedColumn
		{
			get
			{
				return ReadUInt16(4);
			}
		}

		public uint RowHeight
		{
			get
			{
				return ReadUInt16(6);
			}
		}

		public ushort Flags
		{
			get
			{
				return ReadUInt16(12);
			}
		}

		public ushort XFormat
		{
			get
			{
				return ReadUInt16(14);
			}
		}

		internal XlsBiffRow(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
