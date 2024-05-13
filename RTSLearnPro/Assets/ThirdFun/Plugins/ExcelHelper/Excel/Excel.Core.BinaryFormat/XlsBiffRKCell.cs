namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffRKCell : XlsBiffBlankCell
	{
		public double Value
		{
			get
			{
				return NumFromRK(ReadUInt32(6));
			}
		}

		internal XlsBiffRKCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		public static double NumFromRK(uint rk)
		{
			double num = (((rk & 2) != 2) ? Helpers.Int64BitsToDouble((long)((ulong)(uint)((int)rk & -4) << 32)) : ((double)((int)(rk >> 2) | (((rk & 0x80000000u) != 0) ? (-1073741824) : 0))));
			if ((rk & 1) == 1)
			{
				num /= 100.0;
			}
			return num;
		}
	}
}
