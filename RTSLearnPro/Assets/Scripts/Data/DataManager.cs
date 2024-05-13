#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace G13Kit
{
    [Serializable]
    public class DataFileItem
    {
        public enum CreateDatasMethod
        {
            Dont,
            Proxy,
            Deserialize
        }


        public TextAsset textAsset;

        public string tableName;

        public Type CastType
        {
            get
            {
                if (!string.IsNullOrEmpty(m_TypeName))
                {
                    return Type.GetType($"{m_TypeName},{m_AssemblyName}");
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    m_AssemblyName = m_TypeName = null;
                }
                else
                {
                    string typeName = value.FullName.ToString();
                    if (m_TypeName != typeName)
                    {
                        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        foreach (var a in assemblies)
                        {
                            if (a.GetType(typeName) != null)
                            {
                                string exeAssemblyName = a.GetName().Name;
                                m_AssemblyName = exeAssemblyName + ".dll";
                                break;
                            }
                        }

                        m_TypeName = typeName;
                    }
                }
            }
        }

        public CreateDatasMethod createDatas;
        [HideInInspector, SerializeField]
        private string m_TypeName;

        [SerializeField]
        private string m_AssemblyName;

#if UNITY_EDITOR
        private void HandleOnClickShowInCSVView()
        {
            var path = AssetDatabase.GetAssetPath(textAsset);
            DataManagerUtils.OpenSelectedFile(path);
        }

        private static IEnumerable GetTypes()
        {
            var type = typeof(IData);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != typeof(IData));

            return types;
        }
#endif
    }

    public partial class DataManager : PrefabSingleton<DataManager>
    {
        [SerializeField]
        private List<DataFileItem> m_DataFiles;

        private const string DefaultKey = "ID";
        private readonly Dictionary<string, JArray> m_Tables = new Dictionary<string, JArray>();
        private readonly Dictionary<Type, List<IData>> m_Datas = new Dictionary<Type, List<IData>>();

        protected override void Awake()
        {
            base.Awake();

            foreach (var item in m_DataFiles)
            {
                JArray jArray;

                if (IsValidJson(item.textAsset.text))
                {
                    jArray = JsonConvert.DeserializeObject<JArray>(item.textAsset.text);
                }
                else
                {
                    jArray = ConvertCsvFileToJArrayByText(item.textAsset.text);
                }

                if (item.CastType != null && item.createDatas != DataFileItem.CreateDatasMethod.Dont)
                {
                    if (m_Datas.ContainsKey(item.CastType))
                    {
                        throw new Exception("Duplicated type: " + item.CastType);
                    }

                    m_Datas[item.CastType] = new List<IData>();

                    if (item.createDatas == DataFileItem.CreateDatasMethod.Proxy)
                    {
                        foreach (var token in jArray)
                        {
                            var obj = Activator.CreateInstance(item.CastType, (JObject)token);
                            m_Datas[item.CastType].Add((IData)obj);
                        }
                    }
                    else if (item.createDatas == DataFileItem.CreateDatasMethod.Deserialize)
                    {
                        foreach (var token in jArray)
                        {
                            var obj = token.ToObject(item.CastType);
                            m_Datas[item.CastType].Add((IData)obj);
                        }
                    }

                    Log($"Deserialized {item.CastType} for {m_Datas[item.CastType].Count} records");
                }

                m_Tables.Add(item.tableName, jArray);
                Log($"Add table: {item.tableName}, {jArray.Count} records");
            }

            ReleaseTextAssets();
        }

        public List<T> RetrieveDatas<T>() where T : IData
        {
            Type type = typeof(T);
            return m_Datas[type].Cast<T>().ToList();
        }

        private JArray ConvertCsvFileToJArrayByText(string text)
        {
            JArray jArray = new JArray();
            string[][] grid = CsvParser2.Parse(text);
            
            List<string> properties = grid[0].ToList();
            properties.RemoveAll((obj) => string.IsNullOrEmpty(obj));

            for (int i = 1; i < grid.Length; i++)
            {
                string[] row = grid[i];
                JObject obj = new JObject();

                for (int j = 0; j < properties.Count; j++)
                {
                    obj.Add(properties[j], row[j]);
                }

                jArray.Add(obj);
            }

            return jArray;
        }

        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();

            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]")))
            {
                return true;
            }

            return false;
        }

        public JArray GetTableByName(string name)
        {
            if (string.IsNullOrEmpty(name) || !m_Tables.ContainsKey(name))
            {
                throw new Exception(name + " is null");
            }

            return m_Tables[name];
        }

        public void GetDataByID(string tableName, int id)
        {
            //if (string.IsNullOrEmpty(tableName) || !m_Tables.ContainsKey(tableName))
            //{
            //    throw new Exception(tableName + " is null");
            //}

            //var item = m_Tables[tableName].FirstOrDefault(i => i["ID"] != null && (int)i["ID"] == id);

            //if (item == null)
            //{
            //    throw new Exception($"Data is null table: " + tableName + ", Id: " + id);
            //}

            //return (T)Activator.CreateInstance(typeof(T), item as JObject);
            Debug.Log("GetDataByID");
        }

        public T GetDataFromTableById<T>(string tableName, int id, string idKey = DefaultKey) where T : ProxyJObject
        {
            if (string.IsNullOrEmpty(tableName) || !m_Tables.ContainsKey(tableName))
            {
                throw new Exception(tableName + " is null");
            }

            var item = m_Tables[tableName].FirstOrDefault(i => i[idKey] != null && (int)i[idKey] == id);

            if (item == null)
            {
                throw new Exception($"Data is null table: " + tableName + ", Id: " + id);
            }

            return (T)Activator.CreateInstance(typeof(T), item as JObject);
        }

        public bool TryGetDataFromTableById(string tableName, int id, out JObject jObj, string idKey = DefaultKey)
        {
            jObj = null;

            if (string.IsNullOrEmpty(tableName) || !m_Tables.ContainsKey(tableName))
            {
                Log(tableName + " is null");
                return false;
            }

            var table = m_Tables[tableName];

            if (table.Count == 0 || table[0][idKey] == null)
            {
                Log($"Key '{idKey}' doesn't exists in the table {tableName}");
                return false;
            }

            var item = table.FirstOrDefault(i => i[idKey] != null && (int)i[idKey] == id);

            if (item == null)
            {
                Log($"item doesn't exists tableName = {tableName}, idKeyName = {idKey}, id = {id}");
                return false;
            }

            jObj = item as JObject;
            return true;
        }

        public JObject GetDataFromTableById(string tableName, int id, string idKey = DefaultKey)
        {
            if (string.IsNullOrEmpty(tableName) || !m_Tables.ContainsKey(tableName))
            {
                throw new Exception(tableName + " is null");
            }

            var table = m_Tables[tableName];

            if (table.Count == 0 || table[0][idKey] == null)
            {
                throw new Exception($"Key '{idKey}' doesn't exists in the table {tableName}");
            }

            var item = table.FirstOrDefault(i => i[idKey] != null && (int)i[idKey] == id);

            if (item == null)
            {
                throw new Exception("Data is null table: " + tableName + ", Id: " + id);
            }

            return item as JObject;
        }

        public JObject GetDataFromTableByKeyAndValue(string tableName, string key, string value)
        {
            for (int i = 0; i < m_Tables[tableName].Count; i++)
            {
                if (m_Tables[tableName][i][key].ToString() == value.ToString())
                {
                    return m_Tables[tableName][i] as JObject;
                }
            }

            throw new Exception("Data is null table: " + tableName + ", Key: " + key + ", Value: " + value);
        }

        private void ReleaseTextAssets()
        {
            m_DataFiles.ForEach(item =>
            {
                Resources.UnloadAsset(item.textAsset);
            });

            m_DataFiles.Clear();
            m_DataFiles = null;
        }

        private void Log(string content)
        {
            Debug.Log($"#{GetType()}# {content}");
        }

#if UNITY_EDITOR
        public void GenerateDefinitions()
        {
            DataManagerUtils.GenerateDefinition(m_DataFiles);
        }
#endif
    }

#if UNITY_EDITOR
    public static class DataManagerUtils
    {
        const string kClassName = "DMTableName";

        public static void OpenSelectedFile(string path)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "open",
                Arguments = "\"" + path + "\"",
                UseShellExecute = false,
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static void GenerateDefinition(List<DataFileItem> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("/* Auto generated codes */\n");
            stringBuilder.Append($"public class {kClassName}\n");
            stringBuilder.Append("{\n");

            list.ForEach(i =>
            {
                stringBuilder.Append($"\tpublic const string k{i.tableName} = \"{i.tableName}\";\n");
            });

            stringBuilder.Append("}");

            var path = GetFilePath();

            File.WriteAllText(path, stringBuilder.ToString());
            UnityEditor.AssetDatabase.ImportAsset(path);
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log(kClassName + ".cs generated");
        }

        public static string GetFilePath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string path = Path.GetDirectoryName(sourceFilePath);
            string filePath = Path.Combine(path, kClassName + ".cs");
            return filePath;
        }
    }
#endif
}