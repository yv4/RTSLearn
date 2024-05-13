using System;
using System.Data;
using System.IO;

namespace Excel
{
	public interface IExcelDataReader : IDataReader, IDisposable, IDataRecord
	{
		bool IsValid { get; }

		string ExceptionMessage { get; }

		string Name { get; }

		int ResultsCount { get; }

		bool IsFirstRowAsColumnNames { get; set; }

		void Initialize(Stream fileStream);

		DataSet AsDataSet();

		DataSet AsDataSet(bool convertOADateTime);
	}
}
