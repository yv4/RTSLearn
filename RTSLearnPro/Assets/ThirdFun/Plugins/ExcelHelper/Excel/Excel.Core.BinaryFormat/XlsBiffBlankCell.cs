namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffBlankCell : XlsBiffRecord
	{
		public ushort RowIndex
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		public ushort ColumnIndex
		{
			get
			{
				return ReadUInt16(2);
			}
		}

		public ushort XFormat
		{
			get
			{
				return ReadUInt16(4);
			}
		}

		internal XlsBiffBlankCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
