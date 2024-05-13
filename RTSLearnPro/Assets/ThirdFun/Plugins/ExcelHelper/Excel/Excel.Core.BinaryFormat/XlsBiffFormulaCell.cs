using System;
using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffFormulaCell : XlsBiffNumberCell
	{
		[Flags]
		public enum FormulaFlags : ushort
		{
			AlwaysCalc = 1,
			CalcOnLoad = 2,
			SharedFormulaGroup = 8
		}

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

		public FormulaFlags Flags
		{
			get
			{
				return (FormulaFlags)ReadUInt16(14);
			}
		}

		public byte FormulaLength
		{
			get
			{
				return ReadByte(15);
			}
		}

		public new object Value
		{
			get
			{
				long num = ReadInt64(6);
				if ((num & -281474976710656L) == -281474976710656L)
				{
					byte b = (byte)(num & 0xFF);
					byte b2 = (byte)((num >> 16) & 0xFF);
					switch (b)
					{
					case 0:
					{
						XlsBiffRecord record = XlsBiffRecord.GetRecord(m_bytes, (uint)(base.Offset + base.Size), reader);
						XlsBiffFormulaString xlsBiffFormulaString = ((record.ID != BIFFRECORDTYPE.SHRFMLA) ? (record as XlsBiffFormulaString) : (XlsBiffRecord.GetRecord(m_bytes, (uint)(base.Offset + base.Size + record.Size), reader) as XlsBiffFormulaString));
						if (xlsBiffFormulaString == null)
						{
							return string.Empty;
						}
						xlsBiffFormulaString.UseEncoding = m_UseEncoding;
						return xlsBiffFormulaString.Value;
					}
					case 1:
						return b2 != 0;
					case 2:
						return (FORMULAERROR)b2;
					default:
						return null;
					}
				}
				return Helpers.Int64BitsToDouble(num);
			}
		}

		public string Formula
		{
			get
			{
				byte[] array = ReadArray(16, FormulaLength);
				return Encoding.Default.GetString(array, 0, array.Length);
			}
		}

		internal XlsBiffFormulaCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
