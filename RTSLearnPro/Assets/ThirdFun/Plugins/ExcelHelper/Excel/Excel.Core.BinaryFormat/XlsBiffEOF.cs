namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffEOF : XlsBiffRecord
	{
		internal XlsBiffEOF(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
