using System;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffRecord
	{
		protected byte[] m_bytes;

		protected readonly ExcelBinaryReader reader;

		protected int m_readoffset;

		internal byte[] Bytes
		{
			get
			{
				return m_bytes;
			}
		}

		internal int Offset
		{
			get
			{
				return m_readoffset - 4;
			}
		}

		public BIFFRECORDTYPE ID
		{
			get
			{
				return (BIFFRECORDTYPE)BitConverter.ToUInt16(m_bytes, m_readoffset - 4);
			}
		}

		public ushort RecordSize
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, m_readoffset - 2);
			}
		}

		public int Size
		{
			get
			{
				return 4 + RecordSize;
			}
		}

		public bool IsCell
		{
			get
			{
				bool result = false;
				switch (ID)
				{
				case BIFFRECORDTYPE.MULRK:
				case BIFFRECORDTYPE.MULBLANK:
				case BIFFRECORDTYPE.LABELSST:
				case BIFFRECORDTYPE.BLANK:
				case BIFFRECORDTYPE.NUMBER:
				case BIFFRECORDTYPE.BOOLERR:
				case BIFFRECORDTYPE.RK:
				case BIFFRECORDTYPE.FORMULA:
					result = true;
					break;
				}
				return result;
			}
		}

		protected XlsBiffRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
		{
			if (bytes.Length - offset < 4)
			{
				throw new ArgumentException("Error: Buffer size is less than minimum BIFF record size.");
			}
			m_bytes = bytes;
			this.reader = reader;
			m_readoffset = (int)(4 + offset);
			if (reader.ReadOption == ReadOption.Strict && bytes.Length < offset + Size)
			{
				throw new ArgumentException("BIFF Stream error: Buffer size is less than entry length.");
			}
		}

		public static XlsBiffRecord GetRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
		{
			if (offset >= bytes.Length)
			{
				return null;
			}
			uint num = BitConverter.ToUInt16(bytes, (int)offset);
			switch ((ushort)num)
			{
			case 9:
			case 521:
			case 1033:
			case 2057:
				return new XlsBiffBOF(bytes, offset, reader);
			case 10:
				return new XlsBiffEOF(bytes, offset, reader);
			case 225:
				return new XlsBiffInterfaceHdr(bytes, offset, reader);
			case 252:
				return new XlsBiffSST(bytes, offset, reader);
			case 523:
				return new XlsBiffIndex(bytes, offset, reader);
			case 520:
				return new XlsBiffRow(bytes, offset, reader);
			case 215:
				return new XlsBiffDbCell(bytes, offset, reader);
			case 1:
			case 5:
			case 513:
			case 517:
				return new XlsBiffBlankCell(bytes, offset, reader);
			case 190:
				return new XlsBiffMulBlankCell(bytes, offset, reader);
			case 4:
			case 214:
			case 516:
				return new XlsBiffLabelCell(bytes, offset, reader);
			case 253:
				return new XlsBiffLabelSSTCell(bytes, offset, reader);
			case 2:
			case 514:
				return new XlsBiffIntegerCell(bytes, offset, reader);
			case 3:
			case 515:
				return new XlsBiffNumberCell(bytes, offset, reader);
			case 638:
				return new XlsBiffRKCell(bytes, offset, reader);
			case 189:
				return new XlsBiffMulRKCell(bytes, offset, reader);
			case 6:
			case 1030:
				return new XlsBiffFormulaCell(bytes, offset, reader);
			case 30:
			case 1054:
				return new XlsBiffFormatString(bytes, offset, reader);
			case 519:
				return new XlsBiffFormulaString(bytes, offset, reader);
			case 60:
				return new XlsBiffContinue(bytes, offset, reader);
			case 512:
				return new XlsBiffDimensions(bytes, offset, reader);
			case 133:
				return new XlsBiffBoundSheet(bytes, offset, reader);
			case 61:
				return new XlsBiffWindow1(bytes, offset, reader);
			case 66:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 156:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 34:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 218:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 64:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 141:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 352:
				return new XlsBiffSimpleValueRecord(bytes, offset, reader);
			case 94:
				return new XlsBiffUncalced(bytes, offset, reader);
			case 2048:
				return new XlsBiffQuickTip(bytes, offset, reader);
			default:
				return new XlsBiffRecord(bytes, offset, reader);
			}
		}

		public byte ReadByte(int offset)
		{
			return Buffer.GetByte(m_bytes, m_readoffset + offset);
		}

		public ushort ReadUInt16(int offset)
		{
			return BitConverter.ToUInt16(m_bytes, m_readoffset + offset);
		}

		public uint ReadUInt32(int offset)
		{
			return BitConverter.ToUInt32(m_bytes, m_readoffset + offset);
		}

		public ulong ReadUInt64(int offset)
		{
			return BitConverter.ToUInt64(m_bytes, m_readoffset + offset);
		}

		public short ReadInt16(int offset)
		{
			return BitConverter.ToInt16(m_bytes, m_readoffset + offset);
		}

		public int ReadInt32(int offset)
		{
			return BitConverter.ToInt32(m_bytes, m_readoffset + offset);
		}

		public long ReadInt64(int offset)
		{
			return BitConverter.ToInt64(m_bytes, m_readoffset + offset);
		}

		public byte[] ReadArray(int offset, int size)
		{
			byte[] array = new byte[size];
			Buffer.BlockCopy(m_bytes, m_readoffset + offset, array, 0, size);
			return array;
		}

		public float ReadFloat(int offset)
		{
			return BitConverter.ToSingle(m_bytes, m_readoffset + offset);
		}

		public double ReadDouble(int offset)
		{
			return BitConverter.ToDouble(m_bytes, m_readoffset + offset);
		}
	}
}
