namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffUncalced : XlsBiffRecord
	{
		internal XlsBiffUncalced(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
