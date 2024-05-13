namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffIntegerCell : XlsBiffBlankCell
	{
		public uint Value
		{
			get
			{
				return ReadUInt16(6);
			}
		}

		internal XlsBiffIntegerCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
