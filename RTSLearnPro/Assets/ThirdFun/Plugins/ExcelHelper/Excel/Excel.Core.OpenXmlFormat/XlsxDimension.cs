using System;

namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxDimension
	{
		private int _FirstRow;

		private int _LastRow;

		private int _FirstCol;

		private int _LastCol;

		public int FirstRow
		{
			get
			{
				return _FirstRow;
			}
			set
			{
				_FirstRow = value;
			}
		}

		public int LastRow
		{
			get
			{
				return _LastRow;
			}
			set
			{
				_LastRow = value;
			}
		}

		public int FirstCol
		{
			get
			{
				return _FirstCol;
			}
			set
			{
				_FirstCol = value;
			}
		}

		public int LastCol
		{
			get
			{
				return _LastCol;
			}
			set
			{
				_LastCol = value;
			}
		}

		public XlsxDimension(string value)
		{
			ParseDimensions(value);
		}

		public XlsxDimension(int rows, int cols)
		{
			FirstRow = 1;
			LastRow = rows;
			FirstCol = 1;
			LastCol = cols;
		}

		public void ParseDimensions(string value)
		{
			string[] array = value.Split(new char[1] { ':' });
			int val;
			int val2;
			XlsxDim(array[0], out val, out val2);
			FirstCol = val;
			FirstRow = val2;
			if (array.Length == 1)
			{
				LastCol = FirstCol;
				LastRow = FirstRow;
			}
			else
			{
				XlsxDim(array[1], out val, out val2);
				LastCol = val;
				LastRow = val2;
			}
		}

		public static void XlsxDim(string value, out int val1, out int val2)
		{
			int i = 0;
			val1 = 0;
			int[] array = new int[value.Length - 1];
			for (; i < value.Length && !char.IsDigit(value[i]); i++)
			{
				array[i] = value[i] - 65 + 1;
			}
			for (int j = 0; j < i; j++)
			{
				val1 += (int)((double)array[j] * Math.Pow(26.0, i - j - 1));
			}
			val2 = int.Parse(value.Substring(i));
		}
	}
}
