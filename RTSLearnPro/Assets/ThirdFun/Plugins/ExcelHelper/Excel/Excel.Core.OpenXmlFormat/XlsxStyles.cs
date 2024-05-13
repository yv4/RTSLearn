using System.Collections.Generic;

namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxStyles
	{
		private List<XlsxXf> _cellXfs;

		private List<XlsxNumFmt> _NumFmts;

		public List<XlsxXf> CellXfs
		{
			get
			{
				return _cellXfs;
			}
			set
			{
				_cellXfs = value;
			}
		}

		public List<XlsxNumFmt> NumFmts
		{
			get
			{
				return _NumFmts;
			}
			set
			{
				_NumFmts = value;
			}
		}

		public XlsxStyles()
		{
			_cellXfs = new List<XlsxXf>();
			_NumFmts = new List<XlsxNumFmt>();
		}
	}
}
