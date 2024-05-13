namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffContinue : XlsBiffRecord
	{
		internal XlsBiffContinue(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
