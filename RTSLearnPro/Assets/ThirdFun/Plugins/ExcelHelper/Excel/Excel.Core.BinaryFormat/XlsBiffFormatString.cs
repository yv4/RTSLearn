using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffFormatString : XlsBiffRecord
	{
		private Encoding m_UseEncoding = Encoding.Default;

		private string m_value;

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
				BIFFRECORDTYPE iD = base.ID;
				if (iD == BIFFRECORDTYPE.FORMAT_V23)
				{
					return ReadByte(0);
				}
				return ReadUInt16(2);
			}
		}

		public string Value
		{
			get
			{
				if (m_value == null)
				{
					switch (base.ID)
					{
					case BIFFRECORDTYPE.FORMAT_V23:
						m_value = m_UseEncoding.GetString(m_bytes, m_readoffset + 1, Length);
						break;
					case BIFFRECORDTYPE.FORMAT:
					{
						int num = m_readoffset + 5;
						byte b = ReadByte(3);
						m_UseEncoding = (((b & 1) == 1) ? Encoding.Unicode : Encoding.Default);
						if ((b & 4) == 1)
						{
							num += 4;
						}
						if ((b & 8) == 1)
						{
							num += 2;
						}
						m_value = (m_UseEncoding.IsSingleByte ? m_UseEncoding.GetString(m_bytes, num, Length) : m_UseEncoding.GetString(m_bytes, num, Length * 2));
						break;
					}
					}
				}
				return m_value;
			}
		}

		public ushort Index
		{
			get
			{
				BIFFRECORDTYPE iD = base.ID;
				if (iD == BIFFRECORDTYPE.FORMAT_V23)
				{
					return 0;
				}
				return ReadUInt16(0);
			}
		}

		internal XlsBiffFormatString(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
