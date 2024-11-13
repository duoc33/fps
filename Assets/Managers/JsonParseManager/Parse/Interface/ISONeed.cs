using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Managers
{
    public interface NeedDownload
    {
        public void Collect();
    }
    
    /// <summary>
    /// ScriptableObject cant use ITemplate
    /// </summary>
    public interface ITemplate
    {
        [JsonIgnore]
        public string templatePath { get; }

        // public async Task<GameObject> SetTemplate(GlobalContext context, string saveName) 
        // {
        //     return await EditorNRuntimeUtils.GetObjectAsset<GameObject>(templatePath, context.getSaveFolder() + $"{saveName}.prefab");
        // }
        
    }

#if MODEL_MANAGER

    public interface IModelNeed
    {
        public string modelAddress { get; set; }


        public async Task GetModelAction(Action<GameObject> onLoad = null)
        {
            if(!string.IsNullOrEmpty(modelAddress))
            {
                if (modelAddress.StartsWith("http"))
                {
                    Debug.Log($"GetModel: {modelAddress} in Http");
                    // http url
                    Importer.LoadFromUrl(
                        modelUrl: modelAddress, 
                        onMaterialsLoad: loaderContext =>
                        {
                            Debug.Log($"GetModel: on load {modelAddress}");
                            onLoad?.Invoke(loaderContext.RootGameObject);
                        },
                        onError: Debug.LogError);
                }
                else
                {
                    Debug.Log($"GetModel: {modelAddress} in Addressable");
                    // addressable key
                    var modelObject = await EditorNRuntimeUtils.GetObjectAsset<GameObject>(modelAddress, $"{modelAddress}.prefab");
                    onLoad?.Invoke(modelObject);
                }
            }
        }
    }
#endif

}