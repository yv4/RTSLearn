namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffInterfaceHdr : XlsBiffRecord
	{
		public ushort CodePage
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		internal XlsBiffInterfaceHdr(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
