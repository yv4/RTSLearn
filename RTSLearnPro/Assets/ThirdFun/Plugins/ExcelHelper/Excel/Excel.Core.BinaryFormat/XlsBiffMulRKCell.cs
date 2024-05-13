namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffMulRKCell : XlsBiffBlankCell
	{
		public ushort LastColumnIndex
		{
			get
			{
				return ReadUInt16(base.RecordSize - 2);
			}
		}

		internal XlsBiffMulRKCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		public ushort GetXF(ushort ColumnIdx)
		{
			int num = 4 + 6 * (ColumnIdx - base.ColumnIndex);
			if (num > base.RecordSize - 2)
			{
				return 0;
			}
			return ReadUInt16(num);
		}

		public double GetValue(ushort ColumnIdx)
		{
			int num = 6 + 6 * (ColumnIdx - base.ColumnIndex);
			if (num > base.RecordSize)
			{
				return 0.0;
			}
			return XlsBiffRKCell.NumFromRK(ReadUInt32(num));
		}
	}
}
