using System;

namespace Excel.Core.BinaryFormat
{
	internal class XlsBiffWindow1 : XlsBiffRecord
	{
		[Flags]
		public enum Window1Flags : ushort
		{
			Hidden = 1,
			Minimized = 2,
			HScrollVisible = 8,
			VScrollVisible = 0x10,
			WorkbookTabs = 0x20
		}

		public ushort Left
		{
			get
			{
				return ReadUInt16(0);
			}
		}

		public ushort Top
		{
			get
			{
				return ReadUInt16(2);
			}
		}

		public ushort Width
		{
			get
			{
				return ReadUInt16(4);
			}
		}

		public ushort Height
		{
			get
			{
				return ReadUInt16(6);
			}
		}

		public Window1Flags Flags
		{
			get
			{
				return (Window1Flags)ReadUInt16(8);
			}
		}

		public ushort ActiveTab
		{
			get
			{
				return ReadUInt16(10);
			}
		}

		public ushort FirstVisibleTab
		{
			get
			{
				return ReadUInt16(12);
			}
		}

		public ushort SelectedTabCount
		{
			get
			{
				return ReadUInt16(14);
			}
		}

		public ushort TabRatio
		{
			get
			{
				return ReadUInt16(16);
			}
		}

		internal XlsBiffWindow1(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
	}
}
