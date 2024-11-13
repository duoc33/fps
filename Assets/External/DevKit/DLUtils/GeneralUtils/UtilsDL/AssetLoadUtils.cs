using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoadUtils
{
    public static T Load<T>(string key, GameObject holder=null) where T : Object
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (key.Contains("Assets/Resources/"))
        {
            key = key.Replace("Assets/Resources/", "Assets/Resources_moved/");
        }

        return AAUtils.LoadGenericAsset<T>(key);
    }
    public static T Load<T>(object key, GameObject holder=null) where T : Object
    {
        return AAUtils.LoadGenericAsset<T>(key);
    }
}
