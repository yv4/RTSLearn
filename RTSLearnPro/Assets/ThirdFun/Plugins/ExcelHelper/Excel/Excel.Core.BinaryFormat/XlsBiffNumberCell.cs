namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffNumberCell : XlsBiffBlankCell
	{
		public double Value
		{
			get
			{
				return ReadDouble(6);
			}
		}

		internal XlsBiffNumberCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
