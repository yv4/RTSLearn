namespace Excel.Core
{
	public class FormatReader
	{
		private const char escapeChar = '\\';

		public string FormatString { get; set; }

		public bool IsDateFormatString()
		{
			char[] array = new char[10] { 'y', 'm', 'd', 's', 'h', 'Y', 'M', 'D', 'S', 'H' };
			if (FormatString.IndexOfAny(array) >= 0)
			{
				char[] array2 = array;
				foreach (char c in array2)
				{
					for (int num = FormatString.IndexOf(c); num > -1; num = FormatString.IndexOf(c, num + 1))
					{
						if (!IsSurroundedByBracket(c, num) && !IsPrecededByBackSlash(c, num) && !IsSurroundedByQuotes(c, num))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsSurroundedByQuotes(char dateChar, int pos)
		{
			if (pos == FormatString.Length - 1)
			{
				return false;
			}
			int num = NumberOfUnescapedOccurances('"', FormatString.Substring(pos + 1));
			int num2 = NumberOfUnescapedOccurances('"', FormatString.Substring(0, pos));
			if (num % 2 == 1)
			{
				return num2 % 2 == 1;
			}
			return false;
		}

		private bool IsPrecededByBackSlash(char dateChar, int pos)
		{
			if (pos == 0)
			{
				return false;
			}
			if (FormatString[pos - 1].CompareTo('\\') == 0)
			{
				return true;
			}
			return false;
		}

		private bool IsSurroundedByBracket(char dateChar, int pos)
		{
			if (pos == FormatString.Length - 1)
			{
				return false;
			}
			int num = NumberOfUnescapedOccurances('[', FormatString.Substring(0, pos));
			int num2 = NumberOfUnescapedOccurances(']', FormatString.Substring(0, pos));
			num -= num2;
			int num3 = NumberOfUnescapedOccurances('[', FormatString.Substring(pos + 1));
			int num4 = NumberOfUnescapedOccurances(']', FormatString.Substring(pos + 1));
			num4 -= num3;
			if (num % 2 == 1)
			{
				return num4 % 2 == 1;
			}
			return false;
		}

		private int NumberOfUnescapedOccurances(char value, string src)
		{
			int num = 0;
			char c = '\0';
			foreach (char c2 in src)
			{
				if (c2 == value && (c == '\0' || c.CompareTo('\\') != 0))
				{
					num++;
					c = c2;
				}
			}
			return num;
		}
	}
}
