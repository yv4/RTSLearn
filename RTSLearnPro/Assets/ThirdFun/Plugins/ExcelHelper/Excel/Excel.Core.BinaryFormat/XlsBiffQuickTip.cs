namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffQuickTip : XlsBiffRecord
	{
		internal XlsBiffQuickTip(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
