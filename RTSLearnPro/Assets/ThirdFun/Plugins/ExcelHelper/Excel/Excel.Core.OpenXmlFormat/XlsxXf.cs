namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxXf
	{
		public const string N_xf = "xf";

		public const string A_numFmtId = "numFmtId";

		public const string A_xfId = "xfId";

		public const string A_applyNumberFormat = "applyNumberFormat";

		private int _Id;

		private int _numFmtId;

		private bool _applyNumberFormat;

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

		public int NumFmtId
		{
			get
			{
				return _numFmtId;
			}
			set
			{
				_numFmtId = value;
			}
		}

		public bool ApplyNumberFormat
		{
			get
			{
				return _applyNumberFormat;
			}
			set
			{
				_applyNumberFormat = value;
			}
		}

		public XlsxXf(int id, int numFmtId, string applyNumberFormat)
		{
			_Id = id;
			_numFmtId = numFmtId;
			_applyNumberFormat = applyNumberFormat != null && applyNumberFormat == "1";
		}
	}
}
