using System;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class ResourceBundle : ScriptableObject
{
    public List<string> TestString = new List<string>();
    public List<ResourceItem> ResourceItems = new List<ResourceItem>();
    public string BundlePath;
    public string BundleLable;

#if UNITY_EDITOR
    public AddressableAssetGroup Group;
    public AddressableAssetSettings Settings;

    public void GetItem()
    {
        Settings = AddressableAssetSettingsDefaultObject.Settings;

        //m_StopWatch = Stopwatch.StartNew();
        //string[] folders = BundlePath.Split("\n");
        //var guids = AssetDatabase.FindAssets(null, folders);

        int count = 0;
        ResourceItems.Clear();

        //foreach (var guid in guids)
        //{
        //    string path = AssetDatabase.GUIDToAssetPath(guid);
        //    if(AssetDatabase.IsValidFolder(path))
        //    {
        //        continue;
        //    }

        //    AddResItems(guid,guid,path);
        //    CreateEntry(addressableSettings, guid, path);
        //    Debug.Log(path);
        //    count++;

        //    TestString.Add(guid.ToString());
        //}

        //EditorUtility.SetDirty(addressableSettings);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    public void AddResItems(string guid,string name,string path)
    {
        AssetReference assetReference = new AssetReference(guid);

        ResourceItem item = new ResourceItem();
        item.Guid = guid;
        item.Name = path;
        item.Path = path;
        item.Reference = assetReference;


        ResourceItems.Add(item);
    }

    public void CreateEntry(AddressableAssetSettings settings,string guid,string path)
    {
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, Group);
        entry.address = path;
        entry.SetLabel(BundleLable, true);
    }
#endif
}
