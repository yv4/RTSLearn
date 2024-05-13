using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffFormulaString : XlsBiffRecord
	{
		private const int LEADING_BYTES_COUNT = 3;

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
				return ReadUInt16(0);
			}
		}

		public string Value
		{
			get
			{
				if (ReadUInt16(1) != 0)
				{
					return Encoding.Unicode.GetString(m_bytes, m_readoffset + 3, Length * 2);
				}
				return m_UseEncoding.GetString(m_bytes, m_readoffset + 3, Length);
			}
		}

		internal XlsBiffFormulaString(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
