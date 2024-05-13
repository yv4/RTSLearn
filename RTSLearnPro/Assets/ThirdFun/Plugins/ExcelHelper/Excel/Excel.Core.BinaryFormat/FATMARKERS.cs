namespace Excel.Core.BinaryFormat
{
	internal enum FATMARKERS : uint
	{
		FAT_EndOfChain = 4294967294u,
		FAT_FreeSpace = uint.MaxValue,
		FAT_FatSector = 4294967293u,
		FAT_DifSector = 4294967292u
	}
}
