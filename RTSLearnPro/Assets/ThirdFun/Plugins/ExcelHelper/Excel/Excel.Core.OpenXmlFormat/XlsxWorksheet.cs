namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxWorksheet
	{
		public const string N_dimension = "dimension";

		public const string N_worksheet = "worksheet";

		public const string N_row = "row";

		public const string N_col = "col";

		public const string N_c = "c";

		public const string N_v = "v";

		public const string N_t = "t";

		public const string A_ref = "ref";

		public const string A_r = "r";

		public const string A_t = "t";

		public const string A_s = "s";

		public const string N_sheetData = "sheetData";

		public const string N_inlineStr = "inlineStr";

		private XlsxDimension _dimension;

		private string _Name;

		private int _id;

		private string _rid;

		private string _path;

		public bool IsEmpty { get; set; }

		public XlsxDimension Dimension
		{
			get
			{
				return _dimension;
			}
			set
			{
				_dimension = value;
			}
		}

		public int ColumnsCount
		{
			get
			{
				if (!IsEmpty)
				{
					if (_dimension != null)
					{
						return _dimension.LastCol;
					}
					return -1;
				}
				return 0;
			}
		}

		public int RowsCount
		{
			get
			{
				if (_dimension != null)
				{
					return _dimension.LastRow - _dimension.FirstRow + 1;
				}
				return -1;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public string RID
		{
			get
			{
				return _rid;
			}
			set
			{
				_rid = value;
			}
		}

		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		public XlsxWorksheet(string name, int id, string rid)
		{
			_Name = name;
			_id = id;
			_rid = rid;
		}
	}
}
