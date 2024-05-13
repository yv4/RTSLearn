using System;
using System.Collections.Generic;
using System.IO;
using Excel.Exceptions;

namespace Excel.Core.BinaryFormat
{
	internal class XlsHeader
	{
		private readonly byte[] m_bytes;

		private readonly Stream m_file;

		private XlsFat m_fat;

		private XlsFat m_minifat;

		public ulong Signature
		{
			get
			{
				return BitConverter.ToUInt64(m_bytes, 0);
			}
		}

		public bool IsSignatureValid
		{
			get
			{
				return Signature == 16220472316735377360uL;
			}
		}

		public Guid ClassId
		{
			get
			{
				byte[] array = new byte[16];
				Buffer.BlockCopy(m_bytes, 8, array, 0, 16);
				return new Guid(array);
			}
		}

		public ushort Version
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, 24);
			}
		}

		public ushort DllVersion
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, 26);
			}
		}

		public ushort ByteOrder
		{
			get
			{
				return BitConverter.ToUInt16(m_bytes, 28);
			}
		}

		public int SectorSize
		{
			get
			{
				return 1 << (int)BitConverter.ToUInt16(m_bytes, 30);
			}
		}

		public int MiniSectorSize
		{
			get
			{
				return 1 << (int)BitConverter.ToUInt16(m_bytes, 32);
			}
		}

		public int FatSectorCount
		{
			get
			{
				return BitConverter.ToInt32(m_bytes, 44);
			}
		}

		public uint RootDirectoryEntryStart
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 48);
			}
		}

		public uint TransactionSignature
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 52);
			}
		}

		public uint MiniStreamCutoff
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 56);
			}
		}

		public uint MiniFatFirstSector
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 60);
			}
		}

		public int MiniFatSectorCount
		{
			get
			{
				return BitConverter.ToInt32(m_bytes, 64);
			}
		}

		public uint DifFirstSector
		{
			get
			{
				return BitConverter.ToUInt32(m_bytes, 68);
			}
		}

		public int DifSectorCount
		{
			get
			{
				return BitConverter.ToInt32(m_bytes, 72);
			}
		}

		public Stream FileStream
		{
			get
			{
				return m_file;
			}
		}

		public XlsFat FAT
		{
			get
			{
				if (m_fat != null)
				{
					return m_fat;
				}
				int sectorSize = SectorSize;
				List<uint> list = new List<uint>(FatSectorCount);
				int num = 76;
				while (true)
				{
					if (num < sectorSize)
					{
						uint num2 = BitConverter.ToUInt32(m_bytes, num);
						if (num2 == uint.MaxValue)
						{
							break;
						}
						list.Add(num2);
						num += 4;
						continue;
					}
					int difSectorCount;
					if ((difSectorCount = DifSectorCount) == 0)
					{
						break;
					}
					lock (m_file)
					{
						uint num3 = DifFirstSector;
						byte[] array = new byte[sectorSize];
						uint num4 = 0u;
						while (difSectorCount > 0)
						{
							list.Capacity += 128;
							if (num4 == 0 || num3 - num4 != 1)
							{
								m_file.Seek((num3 + 1) * sectorSize, SeekOrigin.Begin);
							}
							num4 = num3;
							m_file.Read(array, 0, sectorSize);
							int num5 = 0;
							while (true)
							{
								uint num2;
								if (num5 < 508)
								{
									num2 = BitConverter.ToUInt32(array, num5);
									if (num2 == uint.MaxValue)
									{
										break;
									}
									list.Add(num2);
									num5 += 4;
									continue;
								}
								num2 = BitConverter.ToUInt32(array, 508);
								if (num2 != uint.MaxValue)
								{
									if (difSectorCount-- > 1)
									{
										num3 = num2;
									}
									else
									{
										list.Add(num2);
									}
									goto IL_0117;
								}
								break;
							}
							break;
							IL_0117:;
						}
					}
					break;
				}
				m_fat = new XlsFat(this, list, SectorSize, false, null);
				return m_fat;
			}
		}

		private XlsHeader(Stream file)
		{
			m_bytes = new byte[512];
			m_file = file;
		}

		public XlsFat GetMiniFAT(XlsRootDirectory rootDir)
		{
			if (m_minifat != null)
			{
				return m_minifat;
			}
			if (MiniFatSectorCount == 0 || MiniSectorSize == 4294967294u)
			{
				return null;
			}
			int miniSectorSize = MiniSectorSize;
			List<uint> list = new List<uint>(MiniFatSectorCount);
			uint item = BitConverter.ToUInt32(m_bytes, 60);
			list.Add(item);
			m_minifat = new XlsFat(this, list, MiniSectorSize, true, rootDir);
			return m_minifat;
		}

		public static XlsHeader ReadHeader(Stream file)
		{
			XlsHeader xlsHeader = new XlsHeader(file);
			lock (file)
			{
				file.Seek(0L, SeekOrigin.Begin);
				file.Read(xlsHeader.m_bytes, 0, 512);
			}
			if (!xlsHeader.IsSignatureValid)
			{
				throw new HeaderException("Error: Invalid file signature.");
			}
			if (xlsHeader.ByteOrder != 65534)
			{
				throw new FormatException("Error: Invalid byte order specified in header.");
			}
			return xlsHeader;
		}
	}
}
