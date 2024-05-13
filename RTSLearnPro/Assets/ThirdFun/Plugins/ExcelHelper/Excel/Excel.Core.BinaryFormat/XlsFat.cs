using System;
using System.Collections.Generic;
using System.IO;

namespace Excel.Core.BinaryFormat
{
	internal class XlsFat
	{
		private readonly List<uint> m_fat;

		private readonly XlsHeader m_hdr;

		private readonly int m_sectors;

		private readonly int m_sectors_for_fat;

		private readonly int m_sectorSize;

		private readonly bool m_isMini;

		private readonly XlsRootDirectory m_rootDir;

		public int SectorsForFat
		{
			get
			{
				return m_sectors_for_fat;
			}
		}

		public int SectorsCount
		{
			get
			{
				return m_sectors;
			}
		}

		public XlsHeader Header
		{
			get
			{
				return m_hdr;
			}
		}

		public XlsFat(XlsHeader hdr, List<uint> sectors, int sizeOfSector, bool isMini, XlsRootDirectory rootDir)
		{
			m_isMini = isMini;
			m_rootDir = rootDir;
			m_hdr = hdr;
			m_sectors_for_fat = sectors.Count;
			sizeOfSector = hdr.SectorSize;
			uint num = 0u;
			uint num2 = 0u;
			byte[] buffer = new byte[sizeOfSector];
			Stream fileStream = hdr.FileStream;
			using (MemoryStream memoryStream = new MemoryStream(sizeOfSector * m_sectors_for_fat))
			{
				lock (fileStream)
				{
					for (int i = 0; i < sectors.Count; i++)
					{
						num = sectors[i];
						if (num2 == 0 || num - num2 != 1)
						{
							fileStream.Seek((num + 1) * sizeOfSector, SeekOrigin.Begin);
						}
						num2 = num;
						fileStream.Read(buffer, 0, sizeOfSector);
						memoryStream.Write(buffer, 0, sizeOfSector);
					}
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				m_sectors = (int)memoryStream.Length / 4;
				m_fat = new List<uint>(m_sectors);
				for (int j = 0; j < m_sectors; j++)
				{
					m_fat.Add(binaryReader.ReadUInt32());
				}
				binaryReader.Close();
				memoryStream.Close();
			}
		}

		public uint GetNextSector(uint sector)
		{
			if (m_fat.Count <= sector)
			{
				throw new ArgumentException("Error reading as FAT table : There's no such sector in FAT.");
			}
			uint num = m_fat[(int)sector];
			if (num == 4294967293u || num == 4294967292u)
			{
				throw new InvalidOperationException("Error reading stream from FAT area.");
			}
			return num;
		}
	}
}
