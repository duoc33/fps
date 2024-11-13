using System.Collections.Generic;
using Addler.Runtime.Core;
using UnityEngine;
//using AddressablesMaster;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

//"com.inc8877.addressables-master": "https://github.com/inc8877/AddressablesMaster.git",
public class AAUtils
{
    // public static GameObject aaHolder;
    public static Dictionary<string, Object> caches = new Dictionary<string, Object>();
    public static GameObject getHolder()
    {
        var obj = PHUtils.TryFindGameObject("aaAAAHolder");
        Object.DontDestroyOnLoad(obj);
        return obj;
    }
    public static T LoadGenericAsset<T>(object key, GameObject holder = null) where T : Object
    {
        // holder = getHolder();
        if (caches.TryGetValue(key.ToString(), out Object o))
        {
            return o as T;
        }
        var obj = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        caches[key.ToString()] = obj;
        // ManageAddressables.AddAutoReleaseAssetTrigger(key, holder);
        return obj;
    }

    //public static Scene LoadScene(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, GameObject holder = null)
    //{
    //    if (holder == null) holder = getHolder();

    //    var sceneInstance = ManageAddressables.LoadSceneSync(key, loadMode, activateOnLoad);
    //    ManageAddressables.AddAutoReleaseInstanceTrigger(key, holder);
    //    return sceneInstance.Scene;
    //}

    public static void Clear()
    {
        // var aaHolder = getHolder();
        // Object.Destroy(aaHolder);
        foreach (KeyValuePair<string, Object> cache in caches)
        {
            Debug.Log("Release " + cache.Key + " from AAUtils");
            Addressables.Release(cache.Value);
        }

        caches.Clear();
    }
}
