using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffStream : XlsStream
	{
		private readonly ExcelBinaryReader reader;

		private readonly byte[] bytes;

		private readonly int m_size;

		private int m_offset;

		public int Size
		{
			get
			{
				return m_size;
			}
		}

		public int Position
		{
			get
			{
				return m_offset;
			}
		}

		public XlsBiffStream(XlsHeader hdr, uint streamStart, bool isMini, XlsRootDirectory rootDir, ExcelBinaryReader reader)
			: base(hdr, streamStart, isMini, rootDir)
		{
			this.reader = reader;
			bytes = base.ReadStream();
			m_size = bytes.Length;
			m_offset = 0;
		}

		[Obsolete("Use BIFF-specific methods for this stream")]
		public new byte[] ReadStream()
		{
			return bytes;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Seek(int offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				m_offset = offset;
				break;
			case SeekOrigin.Current:
				m_offset += offset;
				break;
			case SeekOrigin.End:
				m_offset = m_size - offset;
				break;
			}
			if (m_offset < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("{0} On offset={1}", "BIFF Stream error: Moving before stream start.", offset));
			}
			if (m_offset > m_size)
			{
				throw new ArgumentOutOfRangeException(string.Format("{0} On offset={1}", "BIFF Stream error: Moving after stream end.", offset));
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public XlsBiffRecord Read()
		{
			if ((uint)m_offset >= bytes.Length)
			{
				return null;
			}
			XlsBiffRecord record = XlsBiffRecord.GetRecord(bytes, (uint)m_offset, reader);
			m_offset += record.Size;
			if (m_offset > m_size)
			{
				return null;
			}
			return record;
		}

		public XlsBiffRecord ReadAt(int offset)
		{
			if ((uint)offset >= bytes.Length)
			{
				return null;
			}
			XlsBiffRecord record = XlsBiffRecord.GetRecord(bytes, (uint)offset, reader);
			if (reader.ReadOption == ReadOption.Strict && m_offset + record.Size > m_size)
			{
				return null;
			}
			return record;
		}
	}
}
