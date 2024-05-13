using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Excel.Core;
using Excel.Core.BinaryFormat;
using Excel.Exceptions;
using Excel.Log;

namespace Excel
{
	public class ExcelBinaryReader : IExcelDataReader, IDataReader, IDisposable, IDataRecord
	{
		private const string WORKBOOK = "Workbook";

		private const string BOOK = "Book";

		private const string COLUMN = "Column";

		private Stream m_file;

		private XlsHeader m_hdr;

		private List<XlsWorksheet> m_sheets;

		private XlsBiffStream m_stream;

		private DataSet m_workbookData;

		private XlsWorkbookGlobals m_globals;

		private ushort m_version;

		private bool m_ConvertOADate;

		private Encoding m_encoding;

		private bool m_isValid;

		private bool m_isClosed;

		private readonly Encoding m_Default_Encoding = Encoding.UTF8;

		private string m_exceptionMessage;

		private object[] m_cellsValues;

		private uint[] m_dbCellAddrs;

		private int m_dbCellAddrsIndex;

		private bool m_canRead;

		private int m_SheetIndex;

		private int m_depth;

		private int m_cellOffset;

		private int m_maxCol;

		private int m_maxRow;

		private bool m_noIndex;

		private XlsBiffRow m_currentRowRecord;

		private readonly ReadOption m_ReadOption;

		private bool m_IsFirstRead;

		private bool _isFirstRowAsColumnNames;

		private bool disposed;

		public string ExceptionMessage
		{
			get
			{
				return m_exceptionMessage;
			}
		}

		public string Name
		{
			get
			{
				if (m_sheets != null && m_sheets.Count > 0)
				{
					return m_sheets[m_SheetIndex].Name;
				}
				return null;
			}
		}

		public bool IsValid
		{
			get
			{
				return m_isValid;
			}
		}

		public int Depth
		{
			get
			{
				return m_depth;
			}
		}

		public int ResultsCount
		{
			get
			{
				return m_globals.Sheets.Count;
			}
		}

		public bool IsClosed
		{
			get
			{
				return m_isClosed;
			}
		}

		public int FieldCount
		{
			get
			{
				return m_maxCol;
			}
		}

		public object this[int i]
		{
			get
			{
				return m_cellsValues[i];
			}
		}

		public int RecordsAffected
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object this[string name]
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsFirstRowAsColumnNames
		{
			get
			{
				return _isFirstRowAsColumnNames;
			}
			set
			{
				_isFirstRowAsColumnNames = value;
			}
		}

		public bool ConvertOaDate
		{
			get
			{
				return m_ConvertOADate;
			}
			set
			{
				m_ConvertOADate = value;
			}
		}

		public ReadOption ReadOption
		{
			get
			{
				return m_ReadOption;
			}
		}

		internal ExcelBinaryReader()
		{
			m_encoding = m_Default_Encoding;
			m_version = 1536;
			m_isValid = true;
			m_SheetIndex = -1;
			m_IsFirstRead = true;
		}

		internal ExcelBinaryReader(ReadOption readOption)
			: this()
		{
			m_ReadOption = readOption;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				if (m_workbookData != null)
				{
					m_workbookData.Dispose();
				}
				if (m_sheets != null)
				{
					m_sheets.Clear();
				}
			}
			m_workbookData = null;
			m_sheets = null;
			m_stream = null;
			m_globals = null;
			m_encoding = null;
			m_hdr = null;
			disposed = true;
		}

		~ExcelBinaryReader()
		{
			Dispose(false);
		}

		private int findFirstDataCellOffset(int startOffset)
		{
			XlsBiffRecord xlsBiffRecord = m_stream.ReadAt(startOffset);
			while (!(xlsBiffRecord is XlsBiffDbCell))
			{
				if (m_stream.Position >= m_stream.Size)
				{
					return -1;
				}
				if (xlsBiffRecord is XlsBiffEOF)
				{
					return -1;
				}
				xlsBiffRecord = m_stream.Read();
			}
			XlsBiffDbCell xlsBiffDbCell = (XlsBiffDbCell)xlsBiffRecord;
			XlsBiffRow xlsBiffRow = null;
			int num = xlsBiffDbCell.RowAddress;
			do
			{
				xlsBiffRow = m_stream.ReadAt(num) as XlsBiffRow;
				if (xlsBiffRow == null)
				{
					break;
				}
				num += xlsBiffRow.Size;
			}
			while (xlsBiffRow != null);
			return num;
		}

		private void readWorkBookGlobals()
		{
			try
			{
				m_hdr = XlsHeader.ReadHeader(m_file);
			}
			catch (HeaderException ex)
			{
				fail(ex.Message);
				return;
			}
			catch (FormatException ex2)
			{
				fail(ex2.Message);
				return;
			}
			XlsRootDirectory xlsRootDirectory = new XlsRootDirectory(m_hdr);
			XlsDirectoryEntry xlsDirectoryEntry = xlsRootDirectory.FindEntry("Workbook") ?? xlsRootDirectory.FindEntry("Book");
			if (xlsDirectoryEntry == null)
			{
				fail("Error: Neither stream 'Workbook' nor 'Book' was found in file.");
				return;
			}
			if (xlsDirectoryEntry.EntryType != STGTY.STGTY_STREAM)
			{
				fail("Error: Workbook directory entry is not a Stream.");
				return;
			}
			m_stream = new XlsBiffStream(m_hdr, xlsDirectoryEntry.StreamFirstSector, xlsDirectoryEntry.IsEntryMiniStream, xlsRootDirectory, this);
			m_globals = new XlsWorkbookGlobals();
			m_stream.Seek(0, SeekOrigin.Begin);
			XlsBiffRecord xlsBiffRecord = m_stream.Read();
			XlsBiffBOF xlsBiffBOF = xlsBiffRecord as XlsBiffBOF;
			if (xlsBiffBOF == null || xlsBiffBOF.Type != BIFFTYPE.WorkbookGlobals)
			{
				fail("Error reading Workbook Globals - Stream has invalid data.");
				return;
			}
			bool flag = false;
			m_version = xlsBiffBOF.Version;
			m_sheets = new List<XlsWorksheet>();
			while ((xlsBiffRecord = m_stream.Read()) != null)
			{
				switch (xlsBiffRecord.ID)
				{
				case BIFFRECORDTYPE.INTERFACEHDR:
					m_globals.InterfaceHdr = (XlsBiffInterfaceHdr)xlsBiffRecord;
					break;
				case BIFFRECORDTYPE.BOUNDSHEET:
				{
					XlsBiffBoundSheet xlsBiffBoundSheet = (XlsBiffBoundSheet)xlsBiffRecord;
					if (xlsBiffBoundSheet.Type == XlsBiffBoundSheet.SheetType.Worksheet)
					{
						xlsBiffBoundSheet.IsV8 = isV8();
						xlsBiffBoundSheet.UseEncoding = m_encoding;
						LogManager.Log(this).Debug("BOUNDSHEET IsV8={0}", xlsBiffBoundSheet.IsV8);
						m_sheets.Add(new XlsWorksheet(m_globals.Sheets.Count, xlsBiffBoundSheet));
						m_globals.Sheets.Add(xlsBiffBoundSheet);
					}
					break;
				}
				case BIFFRECORDTYPE.MMS:
					m_globals.MMS = xlsBiffRecord;
					break;
				case BIFFRECORDTYPE.COUNTRY:
					m_globals.Country = xlsBiffRecord;
					break;
				case BIFFRECORDTYPE.CODEPAGE:
					m_globals.CodePage = (XlsBiffSimpleValueRecord)xlsBiffRecord;
					try
					{
						m_encoding = Encoding.GetEncoding(m_globals.CodePage.Value);
					}
					catch (ArgumentException)
					{
					}
					break;
				case BIFFRECORDTYPE.FONT:
				case BIFFRECORDTYPE.FONT_V34:
					m_globals.Fonts.Add(xlsBiffRecord);
					break;
				case BIFFRECORDTYPE.FORMAT_V23:
				{
					XlsBiffFormatString xlsBiffFormatString2 = (XlsBiffFormatString)xlsBiffRecord;
					xlsBiffFormatString2.UseEncoding = m_encoding;
					m_globals.Formats.Add((ushort)m_globals.Formats.Count, xlsBiffFormatString2);
					break;
				}
				case BIFFRECORDTYPE.FORMAT:
				{
					XlsBiffFormatString xlsBiffFormatString = (XlsBiffFormatString)xlsBiffRecord;
					m_globals.Formats.Add(xlsBiffFormatString.Index, xlsBiffFormatString);
					break;
				}
				case BIFFRECORDTYPE.XF_V2:
				case BIFFRECORDTYPE.XF:
				case BIFFRECORDTYPE.XF_V3:
				case BIFFRECORDTYPE.XF_V4:
					m_globals.ExtendedFormats.Add(xlsBiffRecord);
					break;
				case BIFFRECORDTYPE.SST:
					m_globals.SST = (XlsBiffSST)xlsBiffRecord;
					flag = true;
					break;
				case BIFFRECORDTYPE.CONTINUE:
					if (flag)
					{
						XlsBiffContinue fragment = (XlsBiffContinue)xlsBiffRecord;
						m_globals.SST.Append(fragment);
					}
					break;
				case BIFFRECORDTYPE.EXTSST:
					m_globals.ExtSST = xlsBiffRecord;
					flag = false;
					break;
				case BIFFRECORDTYPE.EOF:
					if (m_globals.SST != null)
					{
						m_globals.SST.ReadStrings();
					}
					return;
				}
			}
		}

		private bool readWorkSheetGlobals(XlsWorksheet sheet, out XlsBiffIndex idx, out XlsBiffRow row)
		{
			idx = null;
			row = null;
			m_stream.Seek((int)sheet.DataOffset, SeekOrigin.Begin);
			XlsBiffBOF xlsBiffBOF = m_stream.Read() as XlsBiffBOF;
			if (xlsBiffBOF == null || xlsBiffBOF.Type != BIFFTYPE.Worksheet)
			{
				return false;
			}
			XlsBiffRecord xlsBiffRecord = m_stream.Read();
			if (xlsBiffRecord == null)
			{
				return false;
			}
			if (xlsBiffRecord is XlsBiffIndex)
			{
				idx = xlsBiffRecord as XlsBiffIndex;
			}
			else if (xlsBiffRecord is XlsBiffUncalced)
			{
				idx = m_stream.Read() as XlsBiffIndex;
			}
			if (idx != null)
			{
				idx.IsV8 = isV8();
				LogManager.Log(this).Debug("INDEX IsV8={0}", idx.IsV8);
			}
			XlsBiffDimensions xlsBiffDimensions = null;
			XlsBiffRecord xlsBiffRecord2;
			do
			{
				xlsBiffRecord2 = m_stream.Read();
				if (xlsBiffRecord2.ID == BIFFRECORDTYPE.DIMENSIONS)
				{
					xlsBiffDimensions = (XlsBiffDimensions)xlsBiffRecord2;
					break;
				}
			}
			while (xlsBiffRecord2 != null && xlsBiffRecord2.ID != BIFFRECORDTYPE.ROW);
			if (xlsBiffRecord2.ID == BIFFRECORDTYPE.ROW)
			{
				row = (XlsBiffRow)xlsBiffRecord2;
			}
			XlsBiffRow xlsBiffRow = null;
			while (xlsBiffRow == null && m_stream.Position < m_stream.Size)
			{
				XlsBiffRecord xlsBiffRecord3 = m_stream.Read();
				LogManager.Log(this).Debug("finding rowRecord offset {0}, rec: {1}", xlsBiffRecord3.Offset, xlsBiffRecord3.ID);
				if (xlsBiffRecord3 is XlsBiffEOF)
				{
					break;
				}
				xlsBiffRow = xlsBiffRecord3 as XlsBiffRow;
			}
			if (xlsBiffRow != null)
			{
				LogManager.Log(this).Debug("Got row {0}, rec: id={1},rowindex={2}, rowColumnStart={3}, rowColumnEnd={4}", xlsBiffRow.Offset, xlsBiffRow.ID, xlsBiffRow.RowIndex, xlsBiffRow.FirstDefinedColumn, xlsBiffRow.LastDefinedColumn);
			}
			row = xlsBiffRow;
			if (xlsBiffDimensions != null)
			{
				xlsBiffDimensions.IsV8 = isV8();
				LogManager.Log(this).Debug("dims IsV8={0}", xlsBiffDimensions.IsV8);
				m_maxCol = xlsBiffDimensions.LastColumn - 1;
				if (m_maxCol <= 0 && xlsBiffRow != null)
				{
					m_maxCol = xlsBiffRow.LastDefinedColumn;
				}
				m_maxRow = (int)xlsBiffDimensions.LastRow;
				sheet.Dimensions = xlsBiffDimensions;
			}
			else
			{
				m_maxCol = 256;
				m_maxRow = (int)idx.LastExistingRow;
			}
			if (idx != null && idx.LastExistingRow <= idx.FirstExistingRow)
			{
				return false;
			}
			if (row == null)
			{
				return false;
			}
			m_depth = 0;
			return true;
		}

		private void DumpBiffRecords()
		{
			XlsBiffRecord xlsBiffRecord = null;
			int position = m_stream.Position;
			do
			{
				xlsBiffRecord = m_stream.Read();
				LogManager.Log(this).Debug(xlsBiffRecord.ID.ToString());
			}
			while (xlsBiffRecord != null && m_stream.Position < m_stream.Size);
			m_stream.Seek(position, SeekOrigin.Begin);
		}

		private bool readWorkSheetRow()
		{
			m_cellsValues = new object[m_maxCol];
			while (m_cellOffset < m_stream.Size)
			{
				XlsBiffRecord xlsBiffRecord = m_stream.ReadAt(m_cellOffset);
				m_cellOffset += xlsBiffRecord.Size;
				if (xlsBiffRecord is XlsBiffDbCell)
				{
					break;
				}
				if (xlsBiffRecord is XlsBiffEOF)
				{
					return false;
				}
				XlsBiffBlankCell xlsBiffBlankCell = xlsBiffRecord as XlsBiffBlankCell;
				if (xlsBiffBlankCell != null && xlsBiffBlankCell.ColumnIndex < m_maxCol)
				{
					if (xlsBiffBlankCell.RowIndex != m_depth)
					{
						m_cellOffset -= xlsBiffRecord.Size;
						break;
					}
					pushCellValue(xlsBiffBlankCell);
				}
			}
			m_depth++;
			return m_depth < m_maxRow;
		}

		private DataTable readWholeWorkSheet(XlsWorksheet sheet)
		{
			XlsBiffIndex idx;
			if (!readWorkSheetGlobals(sheet, out idx, out m_currentRowRecord))
			{
				return null;
			}
			DataTable dataTable = new DataTable(sheet.Name);
			bool triggerCreateColumns = true;
			if (idx != null)
			{
				readWholeWorkSheetWithIndex(idx, triggerCreateColumns, dataTable);
			}
			else
			{
				readWholeWorkSheetNoIndex(triggerCreateColumns, dataTable);
			}
			dataTable.EndLoadData();
			return dataTable;
		}

		private void readWholeWorkSheetWithIndex(XlsBiffIndex idx, bool triggerCreateColumns, DataTable table)
		{
			m_dbCellAddrs = idx.DbCellAddresses;
			for (int i = 0; i < m_dbCellAddrs.Length; i++)
			{
				if (m_depth == m_maxRow)
				{
					break;
				}
				m_cellOffset = findFirstDataCellOffset((int)m_dbCellAddrs[i]);
				if (m_cellOffset < 0)
				{
					break;
				}
				if (triggerCreateColumns)
				{
					if ((_isFirstRowAsColumnNames && readWorkSheetRow()) || (_isFirstRowAsColumnNames && m_maxRow == 1))
					{
						for (int j = 0; j < m_maxCol; j++)
						{
							if (m_cellsValues[j] != null && m_cellsValues[j].ToString().Length > 0)
							{
								Helpers.AddColumnHandleDuplicate(table, m_cellsValues[j].ToString());
							}
							else
							{
								Helpers.AddColumnHandleDuplicate(table, "Column" + j);
							}
						}
					}
					else
					{
						for (int k = 0; k < m_maxCol; k++)
						{
							table.Columns.Add(null, typeof(object));
						}
					}
					triggerCreateColumns = false;
					table.BeginLoadData();
				}
				while (readWorkSheetRow())
				{
					table.Rows.Add(m_cellsValues);
				}
				if (m_depth > 0 && (!_isFirstRowAsColumnNames || m_maxRow != 1))
				{
					table.Rows.Add(m_cellsValues);
				}
			}
		}

		private void readWholeWorkSheetNoIndex(bool triggerCreateColumns, DataTable table)
		{
			while (Read() && m_depth != m_maxRow)
			{
				bool flag = false;
				if (triggerCreateColumns)
				{
					if (_isFirstRowAsColumnNames || (_isFirstRowAsColumnNames && m_maxRow == 1))
					{
						for (int i = 0; i < m_maxCol; i++)
						{
							if (m_cellsValues[i] != null && m_cellsValues[i].ToString().Length > 0)
							{
								Helpers.AddColumnHandleDuplicate(table, m_cellsValues[i].ToString());
							}
							else
							{
								Helpers.AddColumnHandleDuplicate(table, "Column" + i);
							}
						}
					}
					else
					{
						for (int j = 0; j < m_maxCol; j++)
						{
							table.Columns.Add(null, typeof(object));
						}
					}
					triggerCreateColumns = false;
					flag = true;
					table.BeginLoadData();
				}
				if (!flag && m_depth > 0 && (!_isFirstRowAsColumnNames || m_maxRow != 1))
				{
					table.Rows.Add(m_cellsValues);
				}
			}
			if (m_depth > 0 && (!_isFirstRowAsColumnNames || m_maxRow != 1))
			{
				table.Rows.Add(m_cellsValues);
			}
		}

		private void pushCellValue(XlsBiffBlankCell cell)
		{
			LogManager.Log(this).Debug("pushCellValue {0}", cell.ID);
			switch (cell.ID)
			{
			case BIFFRECORDTYPE.BOOLERR:
				if (cell.ReadByte(7) == 0)
				{
					m_cellsValues[cell.ColumnIndex] = cell.ReadByte(6) != 0;
				}
				break;
			case BIFFRECORDTYPE.BOOLERR_OLD:
				if (cell.ReadByte(8) == 0)
				{
					m_cellsValues[cell.ColumnIndex] = cell.ReadByte(7) != 0;
				}
				break;
			case BIFFRECORDTYPE.INTEGER_OLD:
			case BIFFRECORDTYPE.INTEGER:
				m_cellsValues[cell.ColumnIndex] = ((XlsBiffIntegerCell)cell).Value;
				break;
			case BIFFRECORDTYPE.NUMBER_OLD:
			case BIFFRECORDTYPE.NUMBER:
			{
				double value2 = ((XlsBiffNumberCell)cell).Value;
				m_cellsValues[cell.ColumnIndex] = ((!ConvertOaDate) ? ((object)value2) : tryConvertOADateTime(value2, cell.XFormat));
				LogManager.Log(this).Debug("VALUE: {0}", value2);
				break;
			}
			case BIFFRECORDTYPE.LABEL_OLD:
			case BIFFRECORDTYPE.RSTRING:
			case BIFFRECORDTYPE.LABEL:
				m_cellsValues[cell.ColumnIndex] = ((XlsBiffLabelCell)cell).Value;
				LogManager.Log(this).Debug("VALUE: {0}", m_cellsValues[cell.ColumnIndex]);
				break;
			case BIFFRECORDTYPE.LABELSST:
			{
				string @string = m_globals.SST.GetString(((XlsBiffLabelSSTCell)cell).SSTIndex);
				LogManager.Log(this).Debug("VALUE: {0}", @string);
				m_cellsValues[cell.ColumnIndex] = @string;
				break;
			}
			case BIFFRECORDTYPE.RK:
			{
				double value2 = ((XlsBiffRKCell)cell).Value;
				m_cellsValues[cell.ColumnIndex] = ((!ConvertOaDate) ? ((object)value2) : tryConvertOADateTime(value2, cell.XFormat));
				LogManager.Log(this).Debug("VALUE: {0}", value2);
				break;
			}
			case BIFFRECORDTYPE.MULRK:
			{
				XlsBiffMulRKCell xlsBiffMulRKCell = (XlsBiffMulRKCell)cell;
				for (ushort num = cell.ColumnIndex; num <= xlsBiffMulRKCell.LastColumnIndex; num = (ushort)(num + 1))
				{
					double value2 = xlsBiffMulRKCell.GetValue(num);
					LogManager.Log(this).Debug("VALUE[{1}]: {0}", value2, num);
					m_cellsValues[num] = ((!ConvertOaDate) ? ((object)value2) : tryConvertOADateTime(value2, xlsBiffMulRKCell.GetXF(num)));
				}
				break;
			}
			case BIFFRECORDTYPE.FORMULA_OLD:
			case BIFFRECORDTYPE.FORMULA:
			{
				object value = ((XlsBiffFormulaCell)cell).Value;
				if (value != null && value is FORMULAERROR)
				{
					value = null;
				}
				else
				{
					m_cellsValues[cell.ColumnIndex] = ((!ConvertOaDate) ? value : tryConvertOADateTime(value, cell.XFormat));
				}
				break;
			}
			}
		}

		private bool moveToNextRecord()
		{
			if (m_noIndex)
			{
				LogManager.Log(this).Debug("No index");
				return moveToNextRecordNoIndex();
			}
			if (m_dbCellAddrs == null || m_dbCellAddrsIndex == m_dbCellAddrs.Length || m_depth == m_maxRow)
			{
				return false;
			}
			m_canRead = readWorkSheetRow();
			if (!m_canRead && m_depth > 0)
			{
				m_canRead = true;
			}
			if (!m_canRead && m_dbCellAddrsIndex < m_dbCellAddrs.Length - 1)
			{
				m_dbCellAddrsIndex++;
				m_cellOffset = findFirstDataCellOffset((int)m_dbCellAddrs[m_dbCellAddrsIndex]);
				if (m_cellOffset < 0)
				{
					return false;
				}
				m_canRead = readWorkSheetRow();
			}
			return m_canRead;
		}

		private bool moveToNextRecordNoIndex()
		{
			XlsBiffRow xlsBiffRow = m_currentRowRecord;
			if (xlsBiffRow == null)
			{
				return false;
			}
			if (xlsBiffRow.RowIndex < m_depth)
			{
				m_stream.Seek(xlsBiffRow.Offset + xlsBiffRow.Size, SeekOrigin.Begin);
				do
				{
					if (m_stream.Position >= m_stream.Size)
					{
						return false;
					}
					XlsBiffRecord xlsBiffRecord = m_stream.Read();
					if (xlsBiffRecord is XlsBiffEOF)
					{
						return false;
					}
					xlsBiffRow = xlsBiffRecord as XlsBiffRow;
				}
				while (xlsBiffRow == null || xlsBiffRow.RowIndex < m_depth);
			}
			m_currentRowRecord = xlsBiffRow;
			XlsBiffBlankCell xlsBiffBlankCell = null;
			do
			{
				if (m_stream.Position >= m_stream.Size)
				{
					return false;
				}
				XlsBiffRecord xlsBiffRecord2 = m_stream.Read();
				if (xlsBiffRecord2 is XlsBiffEOF)
				{
					return false;
				}
				if (xlsBiffRecord2.IsCell)
				{
					XlsBiffBlankCell xlsBiffBlankCell2 = xlsBiffRecord2 as XlsBiffBlankCell;
					if (xlsBiffBlankCell2 != null && xlsBiffBlankCell2.RowIndex == m_currentRowRecord.RowIndex)
					{
						xlsBiffBlankCell = xlsBiffBlankCell2;
					}
				}
			}
			while (xlsBiffBlankCell == null);
			m_cellOffset = xlsBiffBlankCell.Offset;
			m_canRead = readWorkSheetRow();
			return m_canRead;
		}

		private void initializeSheetRead()
		{
			if (m_SheetIndex == ResultsCount)
			{
				return;
			}
			m_dbCellAddrs = null;
			m_IsFirstRead = false;
			if (m_SheetIndex == -1)
			{
				m_SheetIndex = 0;
			}
			XlsBiffIndex idx;
			if (!readWorkSheetGlobals(m_sheets[m_SheetIndex], out idx, out m_currentRowRecord))
			{
				m_SheetIndex++;
				initializeSheetRead();
				return;
			}
			if (idx == null)
			{
				m_noIndex = true;
				return;
			}
			m_dbCellAddrs = idx.DbCellAddresses;
			m_dbCellAddrsIndex = 0;
			m_cellOffset = findFirstDataCellOffset((int)m_dbCellAddrs[m_dbCellAddrsIndex]);
			if (m_cellOffset < 0)
			{
				fail("Badly formed binary file. Has INDEX but no DBCELL");
			}
		}

		private void fail(string message)
		{
			m_exceptionMessage = message;
			m_isValid = false;
			m_file.Close();
			m_isClosed = true;
			m_workbookData = null;
			m_sheets = null;
			m_stream = null;
			m_globals = null;
			m_encoding = null;
			m_hdr = null;
		}

		private object tryConvertOADateTime(double value, ushort XFormat)
		{
			ushort num = 0;
			if (XFormat >= 0 && XFormat < m_globals.ExtendedFormats.Count)
			{
				XlsBiffRecord xlsBiffRecord = m_globals.ExtendedFormats[XFormat];
				switch (xlsBiffRecord.ID)
				{
				case BIFFRECORDTYPE.XF_V2:
					num = (ushort)(xlsBiffRecord.ReadByte(2) & 0x3Fu);
					break;
				case BIFFRECORDTYPE.XF_V3:
					if ((xlsBiffRecord.ReadByte(3) & 4) == 0)
					{
						return value;
					}
					num = xlsBiffRecord.ReadByte(1);
					break;
				case BIFFRECORDTYPE.XF_V4:
					if ((xlsBiffRecord.ReadByte(5) & 4) == 0)
					{
						return value;
					}
					num = xlsBiffRecord.ReadByte(1);
					break;
				default:
					if ((xlsBiffRecord.ReadByte(m_globals.Sheets[m_globals.Sheets.Count - 1].IsV8 ? 9 : 7) & 4) == 0)
					{
						return value;
					}
					num = xlsBiffRecord.ReadUInt16(2);
					break;
				}
			}
			else
			{
				num = XFormat;
			}
			switch (num)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 48:
				return value;
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 45:
			case 46:
			case 47:
				return Helpers.ConvertFromOATime(value);
			case 49:
				return value.ToString();
			default:
			{
				XlsBiffFormatString value2;
				if (m_globals.Formats.TryGetValue(num, out value2))
				{
					string value3 = value2.Value;
					FormatReader formatReader = new FormatReader();
					formatReader.FormatString = value3;
					FormatReader formatReader2 = formatReader;
					if (formatReader2.IsDateFormatString())
					{
						return Helpers.ConvertFromOATime(value);
					}
				}
				return value;
			}
			}
		}

		private object tryConvertOADateTime(object value, ushort XFormat)
		{
			double result;
			if (double.TryParse(value.ToString(), out result))
			{
				return tryConvertOADateTime(result, XFormat);
			}
			return value;
		}

		public bool isV8()
		{
			return m_version >= 1536;
		}

		public void Initialize(Stream fileStream)
		{
			m_file = fileStream;
			readWorkBookGlobals();
			m_SheetIndex = 0;
		}

		public DataSet AsDataSet()
		{
			return AsDataSet(false);
		}

		public DataSet AsDataSet(bool convertOADateTime)
		{
			if (!m_isValid)
			{
				return null;
			}
			if (m_isClosed)
			{
				return m_workbookData;
			}
			ConvertOaDate = convertOADateTime;
			m_workbookData = new DataSet();
			for (int i = 0; i < ResultsCount; i++)
			{
				DataTable dataTable = readWholeWorkSheet(m_sheets[i]);
				if (dataTable != null)
				{
					m_workbookData.Tables.Add(dataTable);
				}
			}
			m_file.Close();
			m_isClosed = true;
			m_workbookData.AcceptChanges();
			Helpers.FixDataTypes(m_workbookData);
			return m_workbookData;
		}

		public void Close()
		{
			m_file.Close();
			m_isClosed = true;
		}

		public bool NextResult()
		{
			if (m_SheetIndex >= ResultsCount - 1)
			{
				return false;
			}
			m_SheetIndex++;
			m_IsFirstRead = true;
			return true;
		}

		public bool Read()
		{
			if (!m_isValid)
			{
				return false;
			}
			if (m_IsFirstRead)
			{
				initializeSheetRead();
			}
			return moveToNextRecord();
		}

		public bool GetBoolean(int i)
		{
			if (IsDBNull(i))
			{
				return false;
			}
			return bool.Parse(m_cellsValues[i].ToString());
		}

		public DateTime GetDateTime(int i)
		{
			if (IsDBNull(i))
			{
				return DateTime.MinValue;
			}
			object obj = m_cellsValues[i];
			if (obj is DateTime)
			{
				return (DateTime)obj;
			}
			string s = obj.ToString();
			double d;
			try
			{
				d = double.Parse(s);
			}
			catch (FormatException)
			{
				return DateTime.Parse(s);
			}
			return DateTime.FromOADate(d);
		}

		public decimal GetDecimal(int i)
		{
			if (IsDBNull(i))
			{
				return decimal.MinValue;
			}
			return decimal.Parse(m_cellsValues[i].ToString());
		}

		public double GetDouble(int i)
		{
			if (IsDBNull(i))
			{
				return double.MinValue;
			}
			return double.Parse(m_cellsValues[i].ToString());
		}

		public float GetFloat(int i)
		{
			if (IsDBNull(i))
			{
				return float.MinValue;
			}
			return float.Parse(m_cellsValues[i].ToString());
		}

		public short GetInt16(int i)
		{
			if (IsDBNull(i))
			{
				return short.MinValue;
			}
			return short.Parse(m_cellsValues[i].ToString());
		}

		public int GetInt32(int i)
		{
			if (IsDBNull(i))
			{
				return int.MinValue;
			}
			return int.Parse(m_cellsValues[i].ToString());
		}

		public long GetInt64(int i)
		{
			if (IsDBNull(i))
			{
				return long.MinValue;
			}
			return long.Parse(m_cellsValues[i].ToString());
		}

		public string GetString(int i)
		{
			if (IsDBNull(i))
			{
				return null;
			}
			return m_cellsValues[i].ToString();
		}

		public object GetValue(int i)
		{
			return m_cellsValues[i];
		}

		public bool IsDBNull(int i)
		{
			if (m_cellsValues[i] != null)
			{
				return DBNull.Value == m_cellsValues[i];
			}
			return true;
		}

		public DataTable GetSchemaTable()
		{
			throw new NotSupportedException();
		}

		public byte GetByte(int i)
		{
			throw new NotSupportedException();
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotSupportedException();
		}

		public char GetChar(int i)
		{
			throw new NotSupportedException();
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotSupportedException();
		}

		public IDataReader GetData(int i)
		{
			throw new NotSupportedException();
		}

		public string GetDataTypeName(int i)
		{
			throw new NotSupportedException();
		}

		public Type GetFieldType(int i)
		{
			throw new NotSupportedException();
		}

		public Guid GetGuid(int i)
		{
			throw new NotSupportedException();
		}

		public string GetName(int i)
		{
			throw new NotSupportedException();
		}

		public int GetOrdinal(string name)
		{
			throw new NotSupportedException();
		}

		public int GetValues(object[] values)
		{
			throw new NotSupportedException();
		}
	}
}
