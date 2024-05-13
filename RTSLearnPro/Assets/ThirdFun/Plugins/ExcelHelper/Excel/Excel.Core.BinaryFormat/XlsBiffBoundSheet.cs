using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffBoundSheet : XlsBiffRecord
	{
		public enum SheetType : byte
		{
			Worksheet = 0,
			MacroSheet = 1,
			Chart = 2,
			VBModule = 6
		}

		public enum SheetVisibility : byte
		{
			Visible,
			Hidden,
			VeryHidden
		}

		private bool isV8 = true;

		private Encoding m_UseEncoding = Encoding.Default;

		public uint StartOffset
		{
			get
			{
				return ReadUInt32(0);
			}
		}

		public SheetType Type
		{
			get
			{
				return (SheetType)ReadByte(4);
			}
		}

		public SheetVisibility VisibleState
		{
			get
			{
				return (SheetVisibility)(ReadByte(5) & 3u);
			}
		}

		public string SheetName
		{
			get
			{
				ushort num = ReadByte(6);
				int num2 = 8;
				if (isV8)
				{
					if (ReadByte(7) == 0)
					{
						return Encoding.Default.GetString(m_bytes, m_readoffset + num2, num);
					}
					return m_UseEncoding.GetString(m_bytes, m_readoffset + num2, Helpers.IsSingleByteEncoding(m_UseEncoding) ? num : (num * 2));
				}
				return Encoding.Default.GetString(m_bytes, m_readoffset + num2 - 1, num);
			}
		}

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

		public bool IsV8
		{
			get
			{
				return isV8;
			}
			set
			{
				isV8 = value;
			}
		}

		internal XlsBiffBoundSheet(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
