using System;
using System.IO;

namespace Excel.Core.BinaryFormat
{
	internal class XlsStream
	{
		protected XlsFat m_fat;

		protected XlsFat m_minifat;

		protected Stream m_fileStream;

		protected XlsHeader m_hdr;

		protected uint m_startSector;

		protected bool m_isMini;

		protected XlsRootDirectory m_rootDir;

		public uint BaseOffset
		{
			get
			{
				return (uint)((m_startSector + 1) * m_hdr.SectorSize);
			}
		}

		public uint BaseSector
		{
			get
			{
				return m_startSector;
			}
		}

		public XlsStream(XlsHeader hdr, uint startSector, bool isMini, XlsRootDirectory rootDir)
		{
			m_fileStream = hdr.FileStream;
			m_fat = hdr.FAT;
			m_hdr = hdr;
			m_startSector = startSector;
			m_isMini = isMini;
			m_rootDir = rootDir;
			CalculateMiniFat(rootDir);
		}

		public void CalculateMiniFat(XlsRootDirectory rootDir)
		{
			m_minifat = m_hdr.GetMiniFAT(rootDir);
		}

		public byte[] ReadStream()
		{
			uint num = m_startSector;
			uint num2 = 0u;
			int num3 = (m_isMini ? m_hdr.MiniSectorSize : m_hdr.SectorSize);
			XlsFat xlsFat = (m_isMini ? m_minifat : m_fat);
			long num4 = 0L;
			if (m_isMini && m_rootDir != null)
			{
				num4 = (m_rootDir.RootEntry.StreamFirstSector + 1) * m_hdr.SectorSize;
			}
			byte[] buffer = new byte[num3];
			using (MemoryStream memoryStream = new MemoryStream(num3 * 8))
			{
				lock (m_fileStream)
				{
					while (true)
					{
						if (num2 == 0 || num - num2 != 1)
						{
							uint num5 = (m_isMini ? num : (num + 1));
							m_fileStream.Seek(num5 * num3 + num4, SeekOrigin.Begin);
						}
						if (num2 != 0 && num2 == num)
						{
							throw new InvalidOperationException("The excel file may be corrupt. We appear to be stuck");
						}
						num2 = num;
						m_fileStream.Read(buffer, 0, num3);
						memoryStream.Write(buffer, 0, num3);
						num = xlsFat.GetNextSector(num);
						switch (num)
						{
						case 0u:
							throw new InvalidOperationException("Next sector cannot be 0. Possibly corrupt excel file");
						case 4294967294u:
							goto end_IL_0097;
						}
						continue;
						end_IL_0097:
						break;
					}
				}
				return memoryStream.ToArray();
			}
		}
	}
}
