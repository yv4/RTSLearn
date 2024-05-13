namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffBOF : XlsBiffRecord
	{
		public ushort Version
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		public BIFFTYPE Type
		{
			get
			{
				return (BIFFTYPE)ReadUInt16(2);
			}
		}

		public ushort CreationID
		{
			get
			{
				if (base.RecordSize < 6)
				{
					return 0;
				}
				return ReadUInt16(4);
			}
		}

		public ushort CreationYear
		{
			get
			{
				if (base.RecordSize < 8)
				{
					return 0;
				}
				return ReadUInt16(6);
			}
		}

		public uint HistoryFlag
		{
			get
			{
				if (base.RecordSize < 12)
				{
					return 0u;
				}
				return ReadUInt32(8);
			}
		}

		public uint MinVersionToOpen
		{
			get
			{
				if (base.RecordSize < 16)
				{
					return 0u;
				}
				return ReadUInt32(12);
			}
		}

		internal XlsBiffBOF(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
