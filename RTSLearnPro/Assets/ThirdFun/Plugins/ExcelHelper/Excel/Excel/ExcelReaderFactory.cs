using System.IO;

namespace Excel
{
	public static class ExcelReaderFactory
	{
		public static IExcelDataReader CreateBinaryReader(Stream fileStream)
		{
			IExcelDataReader excelDataReader = new ExcelBinaryReader();
			excelDataReader.Initialize(fileStream);
			return excelDataReader;
		}

		public static IExcelDataReader CreateBinaryReader(Stream fileStream, ReadOption option)
		{
			IExcelDataReader excelDataReader = new ExcelBinaryReader(option);
			excelDataReader.Initialize(fileStream);
			return excelDataReader;
		}

		public static IExcelDataReader CreateBinaryReader(Stream fileStream, bool convertOADate)
		{
			IExcelDataReader excelDataReader = CreateBinaryReader(fileStream);
			((ExcelBinaryReader)excelDataReader).ConvertOaDate = convertOADate;
			return excelDataReader;
		}

		public static IExcelDataReader CreateBinaryReader(Stream fileStream, bool convertOADate, ReadOption readOption)
		{
			IExcelDataReader excelDataReader = CreateBinaryReader(fileStream, readOption);
			((ExcelBinaryReader)excelDataReader).ConvertOaDate = convertOADate;
			return excelDataReader;
		}

		public static IExcelDataReader CreateOpenXmlReader(Stream fileStream)
		{
			IExcelDataReader excelDataReader = new ExcelOpenXmlReader();
			excelDataReader.Initialize(fileStream);
			return excelDataReader;
		}
	}
}
