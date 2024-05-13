using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffLabelCell : XlsBiffBlankCell
	{
		private Encoding m_UseEncoding = Encoding.Default;

		public Encoding UseEncoding
		{
			get
			{
				return m_UseEncoding;
			}
			set
			{
				m_UseEncoding = value;
			}
		}

		public ushort Length
		{
			get
			{
				return ReadUInt16(6);
			}
		}

		public string Value
		{
			get
			{
				byte[] array = ((!reader.isV8()) ? ReadArray(2, Length * (Helpers.IsSingleByteEncoding(m_UseEncoding) ? 1 : 2)) : ReadArray(9, Length * (Helpers.IsSingleByteEncoding(m_UseEncoding) ? 1 : 2)));
				return m_UseEncoding.GetString(array, 0, array.Length);
			}
		}

		internal XlsBiffLabelCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
