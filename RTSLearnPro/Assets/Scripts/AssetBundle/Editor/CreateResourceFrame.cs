using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;


public class CreateResourceFrame
{
    private static string m_ResourceFilePath = "Assets/NewResourceFrame/";
    private static string m_FileManagerFileName = "ResourceManager";
    private static string m_FileBundleFileName = "ResourceBundle";
    private static string m_FileExtension = ".asset";
    private static Stopwatch m_StopWatch;

    [MenuItem("Tools/CreateResourceFrame")]
    public static void Create()
    {
        if (File.Exists(m_ResourceFilePath + m_FileBundleFileName + m_FileExtension))
            UnityEngine.Debug.Log("ResourceBundle文件已存在");
        else
        {
            ResourceBundle bundle = ScriptableObject.CreateInstance<ResourceBundle>();
            AssetDatabase.CreateAsset(bundle, m_ResourceFilePath + m_FileBundleFileName + m_FileExtension);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/InitBundle")]
    public static void InitBundle()
    {
        string bundlePath = m_ResourceFilePath + m_FileBundleFileName + m_FileExtension;
        ResourceBundle bundle = AssetDatabase.LoadAssetAtPath<ResourceBundle>(bundlePath);
        bundle.GetItem();
        GetBundleItems(bundle);
    }

    public static void GetBundleItems(ResourceBundle bundle)
    {

        m_StopWatch = Stopwatch.StartNew();
        string[] folders = bundle.BundlePath.Split("\n");
        var guids = AssetDatabase.FindAssets(null, folders);

        int count = 0;
        bundle.ResourceItems.Clear();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(path))
            {
                continue;
            }

            bundle.AddResItems(guid, guid, path);
            bundle.CreateEntry(bundle.Settings, guid, path);
            count++;

            bundle.TestString.Add(guid.ToString());
        }

        EditorUtility.SetDirty(bundle.Settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
