using System;
using System.Collections.Generic;
using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffSST : XlsBiffRecord
	{
		private readonly List<uint> continues = new List<uint>();

		private readonly List<string> m_strings;

		private uint m_size;

		public uint Count
		{
			get
			{
				return ReadUInt32(0);
			}
		}

		public uint UniqueCount
		{
			get
			{
				return ReadUInt32(4);
			}
		}

		internal XlsBiffSST(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
			m_size = base.RecordSize;
			m_strings = new List<string>();
		}

		public void ReadStrings()
		{
			uint num = (uint)(m_readoffset + 8);
			uint num2 = (uint)(m_readoffset + base.RecordSize);
			int num3 = 0;
			uint num4 = UniqueCount;
			while (num < num2)
			{
				XlsFormattedUnicodeString xlsFormattedUnicodeString = new XlsFormattedUnicodeString(m_bytes, num);
				uint headSize = xlsFormattedUnicodeString.HeadSize;
				uint tailSize = xlsFormattedUnicodeString.TailSize;
				uint characterCount = xlsFormattedUnicodeString.CharacterCount;
				uint num5 = headSize + tailSize + characterCount + (xlsFormattedUnicodeString.IsMultiByte ? characterCount : 0);
				if (num + num5 > num2)
				{
					if (num3 >= continues.Count)
					{
						break;
					}
					uint num6 = continues[num3];
					byte @byte = Buffer.GetByte(m_bytes, (int)(num6 + 4));
					byte[] array = new byte[num5 * 2];
					Buffer.BlockCopy(m_bytes, (int)num, array, 0, (int)(num2 - num));
					if (@byte == 0 && xlsFormattedUnicodeString.IsMultiByte)
					{
						characterCount -= (num2 - headSize - num) / 2u;
						string @string = Encoding.Default.GetString(m_bytes, (int)(num6 + 5), (int)characterCount);
						byte[] bytes = Encoding.Unicode.GetBytes(@string);
						Buffer.BlockCopy(bytes, 0, array, (int)(num2 - num), bytes.Length);
						Buffer.BlockCopy(m_bytes, (int)(num6 + 5 + characterCount), array, (int)(num2 - num + characterCount + characterCount), (int)tailSize);
						num = num6 + 5 + characterCount + tailSize;
					}
					else if (@byte == 1 && !xlsFormattedUnicodeString.IsMultiByte)
					{
						characterCount -= num2 - num - headSize;
						string string2 = Encoding.Unicode.GetString(m_bytes, (int)(num6 + 5), (int)(characterCount + characterCount));
						byte[] bytes2 = Encoding.Default.GetBytes(string2);
						Buffer.BlockCopy(bytes2, 0, array, (int)(num2 - num), bytes2.Length);
						Buffer.BlockCopy(m_bytes, (int)(num6 + 5 + characterCount + characterCount), array, (int)(num2 - num + characterCount), (int)tailSize);
						num = num6 + 5 + characterCount + characterCount + tailSize;
					}
					else
					{
						Buffer.BlockCopy(m_bytes, (int)(num6 + 5), array, (int)(num2 - num), (int)(num5 - num2 + num));
						num = num6 + 5 + num5 - num2 + num;
					}
					num2 = num6 + 4 + BitConverter.ToUInt16(m_bytes, (int)(num6 + 2));
					num3++;
					xlsFormattedUnicodeString = new XlsFormattedUnicodeString(array, 0u);
				}
				else
				{
					num += num5;
					if (num == num2)
					{
						if (num3 < continues.Count)
						{
							uint num7 = continues[num3];
							num = num7 + 4;
							num2 = num + BitConverter.ToUInt16(m_bytes, (int)(num7 + 2));
							num3++;
						}
						else
						{
							num4 = 1u;
						}
					}
				}
				m_strings.Add(xlsFormattedUnicodeString.Value);
				num4--;
				if (num4 == 0)
				{
					break;
				}
			}
		}

		public string GetString(uint SSTIndex)
		{
			if (SSTIndex < m_strings.Count)
			{
				return m_strings[(int)SSTIndex];
			}
			return string.Empty;
		}

		public void Append(XlsBiffContinue fragment)
		{
			continues.Add((uint)fragment.Offset);
			m_size += (uint)fragment.Size;
		}
	}
}
