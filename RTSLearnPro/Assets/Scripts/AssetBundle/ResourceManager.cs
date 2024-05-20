using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : PrefabSingleton<ResourceManager>
{
    [SerializeField]
    private List<ResourceBundle> m_ResourceBundle;

    protected override void Awake()
    {
        base.Awake();
    }

#if UNITY_EDITOR
    public void GatherAll()
    {
        foreach(var bundle in m_ResourceBundle)
        {
            if(!string.IsNullOrEmpty(bundle.BundlePath))
            {
                bundle.GetItem();
            }
        }
    }
#endif

    private List<ResourceBundle> GetResourceBundleByLabel(string label)
    {
        if(string.IsNullOrEmpty(label))
        {
            throw new Exception("Label can't be empty");
        }

        var settings = m_ResourceBundle.FindAll(item => item.BundleLable == label);

        if(settings.Count==0)
        {
            throw new Exception(label + "doesn't exists");
        }

        return settings;
    }

    public T LoadAddressableAssetByNameAndLabel<T>(string name,string label) where T : UnityEngine.Object
    {
        var settings = GetResourceBundleByLabel(label);
        ResourceItem item = null;

        foreach(var setting in settings)
        {
 
            foreach(var data in setting.ResourceItems)
            {
                if(data.Name == name)
                {
                    item = data;
                    break;
                }
            }
        }

        if(item==null)
        {
            throw new Exception("None asset name "+name);
        }

        if(item.Reference.Asset!=null)
        {
            var obj = (T)item.Reference.Asset;
            return obj;
        }

        var handler = item.Reference.LoadAssetAsync<T>();
        handler.Completed += (handler) =>
        {
            if (handler.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("AssetReference Fail");
            };
        };

        handler.WaitForCompletion();
        return (T)item.Reference.Asset;
    }

    public void ReleaseAssetsByLabel(string label)
    {
        var settings = GetResourceBundleByLabel(label);
        int assetReleaseCount = 0;

        foreach(var bundle in settings)
        {
           foreach(var item in bundle.ResourceItems)
            {
                if(item.Reference.Asset!=null)
                {
                    item.Reference.ReleaseAsset();
                    assetReleaseCount++;
                }
            }

            Debug.Log("Asset Release By Label:"+label+"  Count:"+ assetReleaseCount);
        }
    }
}
