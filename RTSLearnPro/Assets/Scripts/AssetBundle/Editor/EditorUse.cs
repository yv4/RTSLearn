using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorUse
{
    public static string[] FindAssets(string[] folders)
    {
        var guids = AssetDatabase.FindAssets(null, folders);
        return guids;
    }
  
}
