using System;
using System.Text;

namespace Excel.Core.BinaryFormat
{
	internal class XlsFormattedUnicodeString
	{
		[Flags]
		public enum FormattedUnicodeStringFlags : byte
		{
			MultiByte = 1,
			HasExtendedString = 4,
			HasFormatting = 8
		}

		protected byte[] m_bytes;

		protected uint m_offset;

		public ushort CharacterCount
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, (int)m_offset);
			}
		}

		public FormattedUnicodeStringFlags Flags
		{
			get
			{
				return (FormattedUnicodeStringFlags)Buffer.GetByte(m_bytes, (int)(m_offset + 2));
			}
		}

		public bool HasExtString
		{
			get
			{
				return false;
			}
		}

		public bool HasFormatting
		{
			get
			{
				return (Flags & FormattedUnicodeStringFlags.HasFormatting) == FormattedUnicodeStringFlags.HasFormatting;
			}
		}

		public bool IsMultiByte
		{
			get
			{
				return (Flags & FormattedUnicodeStringFlags.MultiByte) == FormattedUnicodeStringFlags.MultiByte;
			}
		}

		private uint ByteCount
		{
			get
			{
				return (uint)(CharacterCount * ((!IsMultiByte) ? 1 : 2));
			}
		}

		public ushort FormatCount
		{
			get
			{
				if (!HasFormatting)
				{
					return 0;
				}
				return BitConverter.ToUInt16(m_bytes, (int)(m_offset + 3));
			}
		}

		public uint ExtendedStringSize
		{
			get
			{
				if (!HasExtString)
				{
					return 0u;
				}
				return BitConverter.ToUInt16(m_bytes, (int)m_offset + (HasFormatting ? 5 : 3));
			}
		}

		public uint HeadSize
		{
			get
			{
				return (uint)((HasFormatting ? 2 : 0) + (HasExtString ? 4 : 0) + 3);
			}
		}

		public uint TailSize
		{
			get
			{
				return (uint)(HasFormatting ? (4 * FormatCount) : 0) + (HasExtString ? ExtendedStringSize : 0);
			}
		}

		public uint Size
		{
			get
			{
				uint num = (uint)((HasFormatting ? (2 + FormatCount * 4) : 0) + (int)(HasExtString ? (4 + ExtendedStringSize) : 0) + 3);
				if (!IsMultiByte)
				{
					return num + CharacterCount;
				}
				return num + (uint)(CharacterCount * 2);
			}
		}

		public string Value
		{
			get
			{
				if (!IsMultiByte)
				{
					return Encoding.Default.GetString(m_bytes, (int)(m_offset + HeadSize), (int)ByteCount);
				}
				return Encoding.Unicode.GetString(m_bytes, (int)(m_offset + HeadSize), (int)ByteCount);
			}
		}

		public XlsFormattedUnicodeString(byte[] bytes, uint offset)
		{
			m_bytes = bytes;
			m_offset = offset;
		}
	}
}
