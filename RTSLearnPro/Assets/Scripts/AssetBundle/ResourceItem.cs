using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class ResourceItem
{
    public string Guid;
    public string Name;
    public string Path;
    public AssetReference Reference;
}
