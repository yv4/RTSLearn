namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffLabelSSTCell : XlsBiffBlankCell
	{
		public uint SSTIndex
		{
			get
			{
				return ReadUInt32(6);
			}
		}

		internal XlsBiffLabelSSTCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		public string Text(XlsBiffSST sst)
		{
			return sst.GetString(SSTIndex);
		}
	}
}
