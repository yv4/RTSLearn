namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffSimpleValueRecord : XlsBiffRecord
	{
		public ushort Value
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		internal XlsBiffSimpleValueRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
