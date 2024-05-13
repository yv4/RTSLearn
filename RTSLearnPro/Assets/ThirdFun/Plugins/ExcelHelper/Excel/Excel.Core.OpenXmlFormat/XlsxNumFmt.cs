namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxNumFmt
	{
		public const string N_numFmt = "numFmt";

		public const string A_numFmtId = "numFmtId";

		public const string A_formatCode = "formatCode";

		private int _Id;

		private string _FormatCode;

		public int Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public string FormatCode
		{
			get
			{
				return _FormatCode;
			}
			set
			{
				_FormatCode = value;
			}
		}

		public XlsxNumFmt(int id, string formatCode)
		{
			_Id = id;
			_FormatCode = formatCode;
		}
	}
}
