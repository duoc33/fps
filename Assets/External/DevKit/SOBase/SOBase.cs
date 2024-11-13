using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace ScriptableObjectBase
{
    public abstract class SOBase : ScriptableObject
    {
        #region Base
        public static Func<string, UniTask<GameObject>> DownloadModelHandler;
        public static Func<string, UniTask<AudioClip>> DownloadAudioHandler;
        public static Func<string, UniTask<Sprite>> DownloadSpriteHandler;
        public static Func<string, UniTask<Texture2D>> DownloadTextureHandler;
        public static Func<string, UniTask<AnimationClip>> DownloadAnimationClipHandler;
        public static Func<string, UnityEngine.Object> GetResHandler;
        

        protected static Sprite GetDownloadedSrpite(string url) => GetResHandler?.Invoke(url) as Sprite;
        protected static Texture2D GetDownloadedTexture2D(string url) => GetResHandler?.Invoke(url) as Texture2D;
        protected static GameObject GetDownloadedGameObject(string url) => GetResHandler?.Invoke(url) as GameObject;
        protected static AudioClip GetDownloadedAudioClip(string url) => GetResHandler?.Invoke(url) as AudioClip;
        protected static AnimationClip GetDownloadedAnimationClip(string url) => GetResHandler?.Invoke(url) as AnimationClip;
        
        public static void InitLocal()
        {
            DownloadModelHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<GameObject>(str);
                await request;
                return request.asset as GameObject;
            };
            DownloadSpriteHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Sprite>(str);
                await request;
                return request.asset as Sprite;
            };
            DownloadAudioHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<AudioClip>(str);
                await request;
                return request.asset as AudioClip;
            };
            DownloadTextureHandler = async (str) =>
            {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Texture2D>(str);
                await request;
                return request.asset as Texture2D;
            };
            DownloadAnimationClipHandler = async (str) => {
                if(string.IsNullOrEmpty(str)) return null;
                ResourceRequest request = Resources.LoadAsync<Texture2D>(str);
                await request;
                return request.asset as AnimationClip;
            };
            GetResHandler = (str) => Resources.Load<UnityEngine.Object>(str);
        }
        public static void InitServer()
        {
            DownloadModelHandler = DLUtils.LoadAddressableObject;
            DownloadSpriteHandler = DLUtils.DownloadSpriteData;
            DownloadAudioHandler = DLUtils.DownloadAudioData;
            DownloadTextureHandler = DLUtils.DownloadTextureData;
            DownloadAnimationClipHandler = DLUtils.LoadAnimationClip;
            GetResHandler = DLUtils.GetResources;
        }
        
        public static void Clear() 
        {
            DestroyImmediate(PostProcessGameObjectHolder);
            PostProcessGameObjectsPool?.Clear();
            DLUtils.ClearCache();
        } 
        
        #endregion

        #region Post Process Resources 
        
        private static GameObject PostProcessGameObjectHolder;
        private static Dictionary<string,GameObject> PostProcessGameObjectsPool;
        protected static GameObject GetPooledGameObject(string url)
        {
            if(PostProcessGameObjectsPool.ContainsKey(url))
            {
                return PostProcessGameObjectsPool[url];
            }
            return null;
        }
        protected static async UniTask DownloadAndPostProcessGameObject(string url, Func<GameObject,GameObject> PostProcessHandler = null ,Func<GameObject , UniTask<GameObject>> PostProcessAsyncHandler = null , bool IsInstantiate = false)
        {
            GameObject target = await DownloadModelHandler.Invoke(url);
            if(target == null) 
            {
                Debug.LogError("Failed to download model: " + url);
                return;
            }
            PostProcessGameObjectsPool ??= new Dictionary<string, GameObject>();
            if(PostProcessGameObjectsPool.ContainsKey(url))
            {
                Debug.LogWarning("PostProcessGameObjectsPool already exists for url: " + url);
                return;
            }
            PostProcessGameObjectHolder ??= new GameObject("PostProcessGameObjectHolder");
            PostProcessGameObjectHolder.SetActive(false);
            GameObject poolItem = IsInstantiate ? target : Instantiate(target,Vector3.zero,Quaternion.identity) ;
            poolItem.name = target.name;
            GameObject newParent = null;
            if(PostProcessAsyncHandler!=null)
            {
                newParent = await PostProcessAsyncHandler.Invoke(poolItem);
            }
            if(PostProcessHandler!=null)
            {
                newParent = PostProcessHandler.Invoke(newParent==null ? poolItem : newParent);
            }
            newParent.transform.SetParent(PostProcessGameObjectHolder.transform);
            PostProcessGameObjectsPool[url] = newParent;
        }

        #endregion
        
        public virtual async UniTask Download() => await UniTask.Yield();
        public virtual void StartMixComponents(){}
        public virtual void OnDestroy() {
            
            // Debug.Log("OnDestroy SOBase");    
        }

    }
}
