using System;
using System.Text;
using Excel.Exceptions;

namespace Excel.Core.BinaryFormat
{
	internal class XlsDirectoryEntry
	{
		public const int Length = 128;

		private readonly byte[] m_bytes;

		private XlsDirectoryEntry m_child;

		private XlsDirectoryEntry m_leftSibling;

		private XlsDirectoryEntry m_rightSibling;

		private XlsHeader m_header;

		public string EntryName
		{
			get
			{
				string @string = Encoding.Unicode.GetString(m_bytes, 0, EntryLength);
				char[] trimChars = new char[1];
				return @string.TrimEnd(trimChars);
			}
		}

		public ushort EntryLength
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, 64);
			}
		}

		public STGTY EntryType
		{
			get
			{
				return (STGTY)Buffer.GetByte(m_bytes, 66);
			}
		}

		public DECOLOR EntryColor
		{
			get
			{
				return (DECOLOR)Buffer.GetByte(m_bytes, 67);
			}
		}

		public uint LeftSiblingSid
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 68);
			}
		}

		public XlsDirectoryEntry LeftSibling
		{
			get
			{
				return m_leftSibling;
			}
			set
			{
				if (m_leftSibling == null)
				{
					m_leftSibling = value;
				}
			}
		}

		public uint RightSiblingSid
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 72);
			}
		}

		public XlsDirectoryEntry RightSibling
		{
			get
			{
				return m_rightSibling;
			}
			set
			{
				if (m_rightSibling == null)
				{
					m_rightSibling = value;
				}
			}
		}

		public uint ChildSid
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 76);
			}
		}

		public XlsDirectoryEntry Child
		{
			get
			{
				return m_child;
			}
			set
			{
				if (m_child == null)
				{
					m_child = value;
				}
			}
		}

		public Guid ClassId
		{
			get
			{
				byte[] array = new byte[16];
				Buffer.BlockCopy(m_bytes, 80, array, 0, 16);
				return new Guid(array);
			}
		}

		public uint UserFlags
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 96);
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return DateTime.FromFileTime(BitConverter.ToInt64(m_bytes, 100));
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return DateTime.FromFileTime(BitConverter.ToInt64(m_bytes, 108));
			}
		}

		public uint StreamFirstSector
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 116);
			}
		}

		public uint StreamSize
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 120);
			}
		}

		public bool IsEntryMiniStream
		{
			get
			{
				return StreamSize < m_header.MiniStreamCutoff;
			}
		}

		public uint PropType
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 124);
			}
		}

		public XlsDirectoryEntry(byte[] bytes, XlsHeader header)
		{
			if (bytes.Length < 128)
			{
				throw new BiffRecordException("Directory Entry error: Array is too small.");
			}
			m_bytes = bytes;
			m_header = header;
		}
	}
}
