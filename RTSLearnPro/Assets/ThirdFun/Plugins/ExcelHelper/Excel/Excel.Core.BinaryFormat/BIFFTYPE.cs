namespace Excel.Core.BinaryFormat
{
	internal enum BIFFTYPE : ushort
	{
		WorkbookGlobals = 5,
		VBModule = 6,
		Worksheet = 16,
		Chart = 32,
		v4MacroSheet = 64,
		v4WorkbookGlobals = 256
	}
}
