using System.Text.RegularExpressions;

namespace Excel.Core
{
	public static class ReferenceHelper
	{
		public static int[] ReferenceToColumnAndRow(string reference)
		{
			Regex regex = new Regex("([a-zA-Z]*)([0-9]*)");
			string text = regex.Match(reference).Groups[1].Value.ToUpper();
			string value = regex.Match(reference).Groups[2].Value;
			int num = 0;
			int num2 = 1;
			for (int num3 = text.Length - 1; num3 >= 0; num3--)
			{
				int num4 = text[num3] - 65 + 1;
				num += num2 * num4;
				num2 *= 26;
			}
			return new int[2]
			{
				int.Parse(value),
				num
			};
		}
	}
}
