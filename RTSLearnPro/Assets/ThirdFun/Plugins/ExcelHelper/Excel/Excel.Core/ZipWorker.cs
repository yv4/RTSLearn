using System;
using System.Collections;
using System.IO;
using Excel.Log;
using ICSharpCode.SharpZipLib.Zip;

namespace Excel.Core
{
	public class ZipWorker : IDisposable
	{
		private const string TMP = "TMP_Z";

		private const string FOLDER_xl = "xl";

		private const string FOLDER_worksheets = "worksheets";

		private const string FILE_sharedStrings = "sharedStrings.{0}";

		private const string FILE_styles = "styles.{0}";

		private const string FILE_workbook = "workbook.{0}";

		private const string FILE_sheet = "sheet{0}.{1}";

		private const string FOLDER_rels = "_rels";

		private const string FILE_rels = "workbook.{0}.rels";

		private byte[] buffer;

		private bool disposed;

		private bool _isCleaned;

		private string _tempPath;

		private string _tempEnv;

		private string _exceptionMessage;

		private string _xlPath;

		private string _format = "xml";

		private bool _isValid;

		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}

		public string TempPath
		{
			get
			{
				return _tempPath;
			}
		}

		public string ExceptionMessage
		{
			get
			{
				return _exceptionMessage;
			}
		}

		public ZipWorker()
		{
			_tempEnv = Path.GetTempPath();
		}

		public bool Extract(Stream fileStream)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			if (fileStream == null)
			{
				return false;
			}
			CleanFromTemp(false);
			NewTempPath();
			_isValid = true;
			ZipFile val = null;
			try
			{
				val = new ZipFile(fileStream);
				IEnumerator enumerator = val.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ZipEntry entry = (ZipEntry)enumerator.Current;
					ExtractZipEntry(val, entry);
				}
			}
			catch (Exception ex)
			{
				_isValid = false;
				_exceptionMessage = ex.Message;
				CleanFromTemp(true);
			}
			finally
			{
				fileStream.Close();
				if (val != null)
				{
					val.Close();
				}
			}
			if (!_isValid)
			{
				return false;
			}
			return CheckFolderTree();
		}

		public Stream GetSharedStringsStream()
		{
			return GetStream(Path.Combine(_xlPath, string.Format("sharedStrings.{0}", _format)));
		}

		public Stream GetStylesStream()
		{
			return GetStream(Path.Combine(_xlPath, string.Format("styles.{0}", _format)));
		}

		public Stream GetWorkbookStream()
		{
			return GetStream(Path.Combine(_xlPath, string.Format("workbook.{0}", _format)));
		}

		public Stream GetWorksheetStream(int sheetId)
		{
			return GetStream(Path.Combine(Path.Combine(_xlPath, "worksheets"), string.Format("sheet{0}.{1}", sheetId, _format)));
		}

		public Stream GetWorksheetStream(string sheetPath)
		{
			if (sheetPath.StartsWith("/xl/"))
			{
				sheetPath = sheetPath.Substring(4);
			}
			return GetStream(Path.Combine(_xlPath, sheetPath));
		}

		public Stream GetWorkbookRelsStream()
		{
			return GetStream(Path.Combine(_xlPath, Path.Combine("_rels", string.Format("workbook.{0}.rels", _format))));
		}

		private void CleanFromTemp(bool catchIoError)
		{
			if (string.IsNullOrEmpty(_tempPath))
			{
				return;
			}
			_isCleaned = true;
			try
			{
				if (Directory.Exists(_tempPath))
				{
					Directory.Delete(_tempPath, true);
				}
			}
			catch (IOException)
			{
				if (!catchIoError)
				{
					throw;
				}
			}
		}

		private void ExtractZipEntry(ZipFile zipFile, ZipEntry entry)
		{
			if (!entry.IsCompressionMethodSupported() || string.IsNullOrEmpty(entry.Name))
			{
				return;
			}
			string text = Path.Combine(_tempPath, entry.Name);
			string path = (entry.IsDirectory ? text : Path.GetDirectoryName(Path.GetFullPath(text)));
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (!entry.IsFile)
			{
				return;
			}
			using (FileStream fileStream = File.Create(text))
			{
				if (buffer == null)
				{
					buffer = new byte[4096];
				}
				using (Stream stream = zipFile.GetInputStream(entry))
				{
					int count;
					while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
					{
						fileStream.Write(buffer, 0, count);
					}
				}
				fileStream.Flush();
			}
		}

		private void NewTempPath()
		{
			string text = Guid.NewGuid().ToString("N");
			_tempPath = Path.Combine(_tempEnv, "TMP_Z" + DateTime.Now.ToFileTimeUtc() + text);
			_isCleaned = false;
			LogManager.Log(this).Debug("Using temp path {0}", _tempPath);
			Directory.CreateDirectory(_tempPath);
		}

		private bool CheckFolderTree()
		{
			_xlPath = Path.Combine(_tempPath, "xl");
			if (Directory.Exists(_xlPath) && Directory.Exists(Path.Combine(_xlPath, "worksheets")) && File.Exists(Path.Combine(_xlPath, "workbook.{0}")))
			{
				return File.Exists(Path.Combine(_xlPath, "styles.{0}"));
			}
			return false;
		}

		private static Stream GetStream(string filePath)
		{
			if (File.Exists(filePath))
			{
				return File.Open(filePath, FileMode.Open, FileAccess.Read);
			}
			return null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && !_isCleaned)
				{
					CleanFromTemp(false);
				}
				buffer = null;
				disposed = true;
			}
		}

		~ZipWorker()
		{
			Dispose(false);
		}
	}
}
