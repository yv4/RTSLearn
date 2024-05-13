using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UExcelReader
{
    public class ExcelReaderEditorWindow : EditorWindow
    {
        [Serializable]
        public class ExcelReaderPrefs
        {
            public string prefsName;
            public string readPath;
            public string writePath;
            public string titleName;
            public int keyRow = 1;
            public int startRow = 2;
            public string sheetName = "Sheet1";


            public bool DataPerfect
            {
                get
                {
                    return !string.IsNullOrEmpty(readPath) && !string.IsNullOrEmpty(writePath);
                }
            }

            public void Export()
            {
                if (string.IsNullOrEmpty(readPath) || string.IsNullOrEmpty(writePath) || string.IsNullOrEmpty(titleName))
                {
                    string err = string.Format("��ȡ·��:{0} ;д��·��:{1} ;������������:{2} ;����һ��Ϊ��", readPath, writePath, titleName);
                    Debug.LogError(err);
                    EditorUtility.DisplayDialog("", err, "ʧ��");
                    return;
                }
                if (!File.Exists(readPath))
                {
                    string err = string.Format("��ǰ·�������ڣ�{0}", readPath);
                    Debug.LogError(err);
                    EditorUtility.DisplayDialog("", err, "ʧ��");
                    return;
                }
                FileStream stream = null;
                try
                {
                    stream = File.Open(readPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (System.Exception ex)
                {
                    string err = string.Format("��ȡ���ñ�[{0}]ʧ��, ����Ƿ���Excel����������ñ�", readPath);
                    EditorUtility.DisplayDialog("", err, "ʧ��");
                    Debug.LogError(err);
                    return;
                }
                //string writePath1 = string.Format("{0}/{1}.csv", writePath, titleName);


                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet result = excelReader.AsDataSet();

                System.Data.DataTable wookSheet = result.Tables[sheetName];
                if (wookSheet == null)
                {
                    Debug.LogError("��ȡ������Ϊ��:" + titleName);
                    return;
                }
                int columnCount = wookSheet.Columns.Count;
                int endRow = wookSheet.Rows.Count;

                StringBuilder builder = new StringBuilder();

                for (int index = 0; index < columnCount; index++)
                {
                    string keyText = wookSheet.Rows[keyRow][index].ToString();
                    if (!string.IsNullOrEmpty(keyText))
                    {
                        builder.Append(keyText);
                        if (index < columnCount - 1)
                        {
                            builder.Append(',');
                        }
                    }
                }
                builder.Append('\n');
                ReadBuilder(endRow, columnCount, wookSheet, ref builder);
                FileStream fileStream = new FileStream(writePath, FileMode.Create, FileAccess.Write);
                StreamWriter sr = new StreamWriter(fileStream);
                sr.WriteLine(builder.ToString());
                sr.Close();
                fileStream.Close();
                Debug.Log("�ɹ���ȡ�ļ���" + readPath);
            }
            private void ReadBuilder(int endRow, int columnCount, DataTable wookSheet, ref StringBuilder builder)
            {
                for (int row = startRow; row < endRow; row++)
                {
                    for (int line = 0; line < columnCount; line++)
                    {
                        string value = wookSheet.Rows[row][line].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            builder.Append(value);
                        }
                        if (line < columnCount - 1)
                        {
                            builder.Append(',');
                        }
                    }
                    int nextRow = row + 1;
                    if (nextRow < endRow - 1)
                    {
                        string value = wookSheet.Rows[nextRow][0].ToString();
                        if (string.IsNullOrEmpty(value))
                        {
                            return;
                        }
                    }
                    if (row < endRow - 1)
                    {
                        builder.Append('\n');
                    }
                }
            }
        }
        [Serializable]
        public class ExcelReaderPrefsFile
        {
            public List<ExcelReaderPrefs> list = new List<ExcelReaderPrefs>();
            /// <summary>
            /// ����Ŀ¼
            /// </summary>
            public string CacheInputDirectory;
            /// <summary>
            /// ���Ŀ¼
            /// </summary>
            public string CacheOutputDirectory;

            public void SortByName()
            {
                if (list != null)
                {
                    list.Sort(delegate (ExcelReaderPrefs a, ExcelReaderPrefs b) {
                        return String.Compare(a.titleName, b.titleName);
                    });
                }
            }
        }
        /// <summary>
        /// �ļ�����Ŀ¼
        /// </summary>
        private string SAVE_PATH;

        private string DEFAULT_ROOT_PATH
        {
            get { return Application.dataPath; }
        }
        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        private string InputDirectory
        {
            get
            {
                return !string.IsNullOrEmpty(mPrefsFile.CacheInputDirectory) ? Path.GetFullPath(mPrefsFile.CacheInputDirectory) : DEFAULT_ROOT_PATH;
            }
            set
            {
                mPrefsFile.CacheInputDirectory = value;
            }
        }
        //���Ŀ¼
        private string DataOutputDirectory
        {
            get
            {
                return !string.IsNullOrEmpty(mPrefsFile.CacheOutputDirectory) ? Path.GetFullPath(mPrefsFile.CacheOutputDirectory) : DEFAULT_ROOT_PATH;
            }
            set { mPrefsFile.CacheOutputDirectory = value; }
        }

        private ExcelReaderPrefsFile mPrefsFile;
        private List<ExcelReaderPrefs> mPrefsDatas { get { return mPrefsFile.list; } }
        private List<string> mPrefsNames;
        private ExcelReaderPrefs mSelectedPrefs;
        private int mSelectedPrefsIndex = -1;
        private bool mIsDirty = false;

        private Vector2 mScrollViewPos;

        private string outputStr = string.Empty;
        [MenuItem("Tools/Excel��񵼱���", priority = 100)]
        static void ShowExcelReaderWindow()
        {
            ExcelReaderEditorWindow window = (ExcelReaderEditorWindow)EditorWindow.GetWindow(typeof(ExcelReaderEditorWindow), true);
            window.Show();
        }

        private void OnEnable()
        {
            //string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            //path = Path.GetDirectoryName(path);
            string path = Application.persistentDataPath;
            Debug.Log("path:" + path);
            SAVE_PATH = Path.Combine(path, "ExcelInfo.json");

            mPrefsNames = new List<string>();
            mSelectedPrefs = null;
            mSelectedPrefsIndex = -1;
            if (File.Exists(SAVE_PATH))
            {
                mPrefsFile = (ExcelReaderPrefsFile)JsonUtility.FromJson(File.ReadAllText(SAVE_PATH), typeof(ExcelReaderPrefsFile));
                mPrefsFile.SortByName();
                foreach (ExcelReaderPrefs item in mPrefsFile.list)
                {
                    if (string.IsNullOrEmpty(item.titleName))
                    {
                        item.titleName = item.prefsName + "_" + item.sheetName;
                    }
                    mPrefsNames.Add(item.titleName);
                }
            }
            else
            {
                mPrefsFile = new ExcelReaderPrefsFile();
            }
        }
        private void SavePrefs()
        {
            File.WriteAllText(SAVE_PATH, JsonUtility.ToJson(mPrefsFile, true));
        }
        private void OnGUI()
        {
            mIsDirty = false;
            mScrollViewPos = GUILayout.BeginScrollView(mScrollViewPos);
            GUILayout.Label("Excel Reader", EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("��·��", GUILayout.Width(120));
            GUILayout.Box(Path.GetFullPath(DEFAULT_ROOT_PATH + "/../"));
            GUILayout.EndHorizontal();
            GUILayout.Label("Excel�ļ�����·��");

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            mSelectedPrefsIndex = EditorGUILayout.Popup("ѡ��һ�����ñ�", mSelectedPrefsIndex, mPrefsNames.ToArray(), GUILayout.Width(300));
            if (mSelectedPrefsIndex >= 0 && mPrefsDatas.Count > mSelectedPrefsIndex)
            {
                mSelectedPrefs = mPrefsDatas[mSelectedPrefsIndex];
            }

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                mSelectedPrefs = new ExcelReaderPrefs();
                mSelectedPrefs.titleName = "NewConfig" + mPrefsDatas.Count;
                mSelectedPrefs.prefsName = mSelectedPrefs.titleName;
                mPrefsDatas.Add(mSelectedPrefs);
                mPrefsNames.Add(mSelectedPrefs.titleName);
                mIsDirty = true;
                mSelectedPrefsIndex = mPrefsDatas.Count - 1;
            }

            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                if (mSelectedPrefs != null)
                {
                    mPrefsDatas.Remove(mSelectedPrefs);
                    mPrefsNames.Remove(mSelectedPrefs.titleName);
                    mSelectedPrefs = null;
                    mIsDirty = true;
                }
            }
            GUILayout.EndHorizontal();

            if (mSelectedPrefs != null)
            {
                EditSelectPrefs();
            }

            GUILayout.Space(5);
            //ȫ������
            if (GUILayout.Button("����ȫ�������õ��б��ļ�", GUILayout.Width(200)))
            {
                ExportAll();
            }
            GUILayout.Space(5);

            if (!string.IsNullOrEmpty(outputStr))
            {
                GUILayout.Label("���:");
                EditorGUILayout.HelpBox(outputStr, MessageType.Info);
            }

            GUILayout.EndScrollView();

            if (mIsDirty)
            {
                SavePrefs();
            }
        }

        private void EditSelectPrefs()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            string classname = EditorGUILayout.TextField("���ñ����ƣ�", mSelectedPrefs.titleName, GUILayout.Width(300));
            if (!string.IsNullOrEmpty(classname) && !classname.Equals(mSelectedPrefs.titleName))
            {
                mSelectedPrefs.titleName = classname;
                mSelectedPrefs.prefsName = AdaptPrefsName(mSelectedPrefsIndex, classname);
                mPrefsNames[mSelectedPrefsIndex] = mSelectedPrefs.titleName;
                mIsDirty = true;
            }
            GUILayout.Space(5);
            string startIndex = EditorGUILayout.TextField("������ʼ��:", mSelectedPrefs.startRow.ToString(), GUILayout.Width(300));
            if (!string.IsNullOrEmpty(startIndex) && int.TryParse(startIndex, out int startRow) && startRow != mSelectedPrefs.startRow)
            {
                mSelectedPrefs.startRow = startRow;
                mIsDirty = true;
            }
            GUILayout.Space(5);

            string keyIndex = EditorGUILayout.TextField("�ֶ�������:", mSelectedPrefs.keyRow.ToString(), GUILayout.Width(300));
            if (!string.IsNullOrEmpty(keyIndex) && int.TryParse(keyIndex, out int keyRow) && keyRow != mSelectedPrefs.keyRow)
            {
                mSelectedPrefs.keyRow = keyRow;
                mIsDirty = true;
            }
            GUILayout.Space(5);

            string sheetName = EditorGUILayout.TextField("����������:", mSelectedPrefs.sheetName, GUILayout.Width(300));
            if (!string.IsNullOrEmpty(sheetName) && !sheetName.Equals(mSelectedPrefs.sheetName))
            {
                mSelectedPrefs.sheetName = sheetName;
                mIsDirty = true;
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ѡ������Excel��", GUILayout.Width(150));
            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                string filePath = EditorUtility.OpenFilePanel("Select Excel File", InputDirectory, "xlsx");
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = MakeRelativePath(filePath, DEFAULT_ROOT_PATH);
                    mSelectedPrefs.readPath = filePath;
                    InputDirectory = Path.GetDirectoryName(filePath);
                    mIsDirty = true;
                }
            }

            if (!string.IsNullOrEmpty(mSelectedPrefs.readPath))
            {
                GUILayout.Box(mSelectedPrefs.readPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ѡ��.csv���·��", GUILayout.Width(150));
            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                string filePath = EditorUtility.SaveFilePanel("Select Data Output", DataOutputDirectory, mSelectedPrefs.titleName, "csv");
                if (!string.IsNullOrEmpty(filePath))
                {
                    mSelectedPrefs.writePath = MakeRelativePath(filePath, DEFAULT_ROOT_PATH);
                    DataOutputDirectory = Path.GetDirectoryName(mSelectedPrefs.writePath);
                    mIsDirty = true;
                }
            }

            if (!string.IsNullOrEmpty(mSelectedPrefs.writePath))
            {
                GUILayout.Box(mSelectedPrefs.writePath);
            }
            GUILayout.EndHorizontal();


            if (GUILayout.Button("����*.*", GUILayout.Width(100)))
            {
                if (mSelectedPrefs.DataPerfect)
                {
                    outputStr = "";
                    mSelectedPrefs.Export();
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("", "���������ݣ�", "OK");
                }
            }

            GUILayout.EndVertical();
        }
        private string AdaptPrefsName(int index, string prefsName)
        {
            Regex rgx = new Regex(@"^" + prefsName + @"-\d*$");
            int i = -1;

            for (int j = 0; j < mPrefsNames.Count; j++)
            {
                if (j == index) continue;
                string s = mPrefsNames[j];
                if (s.Equals(prefsName))
                {
                    if (i == -1)
                        i = 0;
                }
                else if (rgx.IsMatch(s))
                {
                    int suffixid = int.Parse(s.Substring(prefsName.Length + 1));
                    if (suffixid > i) i = suffixid;
                }
            }
            if (i >= 0)
            {
                ++i;
                return prefsName + "-" + i;
            }
            return prefsName;
        }

        private void ExportAll()
        {
            outputStr = "";
            foreach (ExcelReaderPrefs item in mPrefsDatas)
            {
                if (item.DataPerfect)
                {
                    item.Export();
                }
                else
                {
                    EditorUtility.DisplayDialog("", "���������ݣ�" + item.titleName, "OK");
                }
            }
            AssetDatabase.Refresh();
        }
        static string MakeRelativePath(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            Uri referenceUri = new Uri(referencePath);
            string relativePath = Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            return relativePath;
        }
    }
}
