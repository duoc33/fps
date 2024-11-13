using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TriLibCore;
using TriLibCore.Mappers;
using UnityEngine;

namespace TriLibExtensions
{
    public class ModelLoader
    {
        private float Timeout = 10f;

        private MaterialMapper[] _materialMapper;
        private ByNameHumanoidAvatarMapper _humanoidAvatarMapper;
        public ModelLoader(float timeout = 10f, TriLibCore.General.StringComparisonMode hunmanoidStringComparisonMode = TriLibCore.General.StringComparisonMode.LeftEndsWithRight)
        {
            Timeout = timeout;
            _materialMapper ??= new MaterialMapper[] { UnityEngine.Object.Instantiate(Resources.Load<MaterialMapper>("ModelLoaderSettings/UniversalRPMaterialMapper")) };
            _humanoidAvatarMapper ??= UnityEngine.Object.Instantiate(Resources.Load<ByNameHumanoidAvatarMapper>("ModelLoaderSettings/MixamoAndBipedByNameHumanoidAvatarMapper"));
        }
        public async UniTask<ModelLoaderResult> LoadHumanoidModelAsync(string url, Avatar avatar = null)
        {
            ModelLoaderResult res = new ModelLoaderResult();
            AssetLoaderOptions _assetLoaderOptions = CreateModelLoaderOptions();
            _assetLoaderOptions.EnforceTPose = true;
            _assetLoaderOptions.AnimationType = TriLibCore.General.AnimationType.Humanoid;
            if (avatar == null)
            {
                _assetLoaderOptions.AvatarDefinition = TriLibCore.General.AvatarDefinitionType.CreateFromThisModel;
            }
            else
            {
                _assetLoaderOptions.AvatarDefinition = TriLibCore.General.AvatarDefinitionType.CopyFromOtherAvatar;
                _assetLoaderOptions.Avatar = avatar;
            }
            await LoadModelCoroutine(url, res,_assetLoaderOptions).ToUniTask();
            UniTask DelayTask = UniTask.Delay(TimeSpan.FromSeconds(Timeout));
            UniTask CompeletedTask = UniTask.WaitUntil(() => res.Model != null);
            int index = await UniTask.WhenAny(DelayTask, CompeletedTask);
            if (index == 1)
            {
                res.Model.TryGetComponent(out Animation animation);
                res.Model.TryGetComponent(out Animator animator);
                res.Clips = GetAllAnimationClips(animation);
                res.Avatar = animator?.avatar;
            }
            else
            {
                Debug.LogError("Time out ~~~~~~~~~~~~, 已经超过时间了，下载失败");
            }
            return res;
        }
        public async UniTask<ModelLoaderResult> LoadGenericModelAsync(string url, bool isNeedAnimation = true)
        {
            ModelLoaderResult res = new ModelLoaderResult();
            AssetLoaderOptions _assetLoaderOptions = CreateModelLoaderOptions();
            if (isNeedAnimation)
            {
                _assetLoaderOptions.AnimationType = TriLibCore.General.AnimationType.Generic;
                _assetLoaderOptions.AvatarDefinition = TriLibCore.General.AvatarDefinitionType.CopyFromOtherAvatar;
                _assetLoaderOptions.Avatar = null;
            }
            else
            {
                _assetLoaderOptions.AnimationType = TriLibCore.General.AnimationType.None;
            }
            await LoadModelCoroutine(url, res,_assetLoaderOptions).ToUniTask();
            UniTask DelayTask = UniTask.Delay(TimeSpan.FromSeconds(Timeout));
            UniTask CompeletedTask = UniTask.WaitUntil(() => res.Model != null);
            int index = await UniTask.WhenAny(DelayTask, CompeletedTask);
            await UniTask.Yield(); //以防万一，等一下组件刷新。
            if (index == 1)
            {
                res.Model.TryGetComponent(out Animation animation);
                res.Model.TryGetComponent(out Animator animator);
                res.Clips = GetAllAnimationClips(animation);
                res.Avatar = animator?.avatar;
            }
            else
            {
                Debug.LogError("Time out ~~~~~~~~~~~~, 已经超过时间了，下载失败");
            }
            return res;
        }
        public async UniTask<ModelLoaderResult> LoadAsyncAnimationOnly(string url)
        {
            ModelLoaderResult res = new ModelLoaderResult();
            AssetLoaderOptions _assetLoaderOptions = CreateDefault();
            // if(IsNeedHumanoidAvatar==true)
            // {
            //     _assetLoaderOptions.AnimationType = TriLibCore.General.AnimationType.Humanoid;
            //     _assetLoaderOptions.AvatarDefinition = TriLibCore.General.AvatarDefinitionType.CreateFromThisModel;
            // }
            _assetLoaderOptions.ImportMaterials = false;
            _assetLoaderOptions.ImportTextures = false;
            _assetLoaderOptions.ImportMeshes = false;
            await LoadAnimationOnlyCoroutine(url, res,_assetLoaderOptions).ToUniTask();
            UniTask DelayTask = UniTask.Delay(TimeSpan.FromSeconds(Timeout));
            UniTask CompeletedTask = UniTask.WaitUntil(() => res.Clips != null);
            int index = await UniTask.WhenAny(DelayTask, CompeletedTask);
            if (index == 1)
            {
            }
            else
            {
                Debug.LogError("Time out ~~~~~~~~~~~~, 已经超过时间了，下载失败");
            }
            return res;
        }
        
        private IEnumerator LoadModelCoroutine(string url, ModelLoaderResult result, AssetLoaderOptions _assetLoaderOptions)
        {
            void OnError(IContextualizedError obj)
            {
                Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
            }
            void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
            {
            }
            void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
            {
                result.Model = assetLoaderContext.RootGameObject;
            }
            void OnLoad(AssetLoaderContext assetLoaderContext)
            {
                
            }
            var webRequest = AssetDownloader.CreateWebRequest(url);
            yield return AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);
        }
        private IEnumerator LoadAnimationOnlyCoroutine(string url, ModelLoaderResult result, AssetLoaderOptions _assetLoaderOptions)
        {
            void OnError(IContextualizedError obj)
            {
                Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
            }
            void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
            {
            }
            void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
            {
            }
            void OnLoad(AssetLoaderContext assetLoaderContext)
            {
                AddAnimation(assetLoaderContext,result);
            }
            var webRequest = AssetDownloader.CreateWebRequest(url);
            yield return AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);
        }

        private List<AnimationClip> GetAllAnimationClips(Animation animation)
        {
            List<AnimationClip> list = new List<AnimationClip>();
            foreach (AnimationState item in animation)
            {
                list.Add(item.clip);
            }
            return list;
        }
        private void AddAnimation(AssetLoaderContext loadedAnimationContext,ModelLoaderResult result)
        {
            var rootGameObjectAnimation = loadedAnimationContext.RootGameObject.GetComponent<Animation>();
            if (rootGameObjectAnimation != null)
            {
                result.Clips = GetAllAnimationClips(rootGameObjectAnimation);
            }
            UnityEngine.Object.DestroyImmediate(loadedAnimationContext.RootGameObject);
        }
        private AssetLoaderOptions CreateDefault()
        {
            AssetLoaderOptions _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            return _assetLoaderOptions;
        }
        private AssetLoaderOptions CreateModelLoaderOptions()
        {
            AssetLoaderOptions _assetLoaderOptions = CreateDefault();
            _assetLoaderOptions.Static = false;
            _assetLoaderOptions.TextureCompressionQuality = TriLibCore.General.TextureCompressionQuality.NoCompression;
            _assetLoaderOptions.LoadTexturesAsSRGB = false;
            _assetLoaderOptions.ReadEnabled = true;
            
            //下面是官方推荐设置
            _assetLoaderOptions.GetCompatibleTextureFormat = false;
            _assetLoaderOptions.EnforceAlphaChannelTextures = false;
            _assetLoaderOptions.UseUnityNativeTextureLoader = true;
            _assetLoaderOptions.UseUnityNativeNormalCalculator = true;
            _assetLoaderOptions.CompactHeap = false;

            _assetLoaderOptions.AutomaticallyPlayLegacyAnimations = false;
            _assetLoaderOptions.AddAssetUnloader = true;
            _assetLoaderOptions.PivotPosition = TriLibCore.General.PivotPosition.Center;
            _assetLoaderOptions.MaterialMappers = _materialMapper;
            _assetLoaderOptions.HumanoidAvatarMapper = _humanoidAvatarMapper;
            return _assetLoaderOptions;
        }
        public void Dispose()
        {
            if (_materialMapper != null && _materialMapper.Length > 0)
            {
                foreach (var item in _materialMapper)
                {
                    UnityEngine.Object.DestroyImmediate(item);
                }
                _materialMapper = null;
            }

            if (_humanoidAvatarMapper != null)
            {
                UnityEngine.Object.DestroyImmediate(_humanoidAvatarMapper);
            }

        }
    }
    public class ModelLoaderResult
    {
        public GameObject Model;
        public List<AnimationClip> Clips;
        public Avatar Avatar;
    }


    public class BuildAvatar
    {
        // private static void SetupHumanoidAvatar(AssetLoaderContext assetLoaderContext, Animator animator)
        // {
        //     var valid = false;
        //     var mapping = assetLoaderContext.Options.HumanoidAvatarMapper.Map(assetLoaderContext);
        //     if (mapping.Count > 0)
        //     {
        //         var parent = assetLoaderContext.RootGameObject.transform.parent;
        //         var rootGameObjectPosition = assetLoaderContext.RootGameObject.transform.position;
        //         assetLoaderContext.RootGameObject.transform.SetParent(null, false);
        //         assetLoaderContext.Options.HumanoidAvatarMapper.PostSetup(assetLoaderContext, mapping);
        //         Transform hipsTransform = null;
        //         var humanBones = new HumanBone[mapping.Count];
        //         var boneIndex = 0;
        //         foreach (var kvp in mapping)
        //         {
        //             if (kvp.Key.HumanBone == HumanBodyBones.Hips)
        //             {
        //                 hipsTransform = kvp.Value;
        //             }
        //             humanBones[boneIndex++] = CreateHumanBone(kvp.Key, kvp.Value.name);
        //         }
        //         if (hipsTransform != null)
        //         {
        //             var skeletonBones = new Dictionary<Transform, SkeletonBone>();
        //             var bounds = assetLoaderContext.RootGameObject.CalculateBounds();
        //             var toBottom = bounds.min.y;
        //             if (toBottom < 0f)
        //             {
        //                 var hipsTransformPosition = hipsTransform.position;
        //                 hipsTransformPosition.y -= toBottom;
        //                 hipsTransform.position = hipsTransformPosition;
        //             }
        //             var toCenter = Vector3.zero - bounds.center;
        //             toCenter.y = 0f;
        //             if (toCenter.sqrMagnitude > 0.01f)
        //             {
        //                 var hipsTransformPosition = hipsTransform.position;
        //                 hipsTransformPosition += toCenter;
        //                 hipsTransform.position = hipsTransformPosition;
        //             }
        //             foreach (var kvp in assetLoaderContext.GameObjects)
        //             {
        //                 if (!skeletonBones.ContainsKey(kvp.Value.transform))
        //                 {
        //                     skeletonBones.Add(kvp.Value.transform, CreateSkeletonBone(kvp.Value.transform));
        //                 }
        //             }
        //             var triLibHumanDescription = assetLoaderContext.Options.HumanDescription ?? new General.HumanDescription();
        //             var humanDescription = new HumanDescription
        //             {
        //                 armStretch = triLibHumanDescription.armStretch,
        //                 feetSpacing = triLibHumanDescription.feetSpacing,
        //                 hasTranslationDoF = triLibHumanDescription.hasTranslationDof,
        //                 legStretch = triLibHumanDescription.legStretch,
        //                 lowerArmTwist = triLibHumanDescription.lowerArmTwist,
        //                 lowerLegTwist = triLibHumanDescription.lowerLegTwist,
        //                 upperArmTwist = triLibHumanDescription.upperArmTwist,
        //                 upperLegTwist = triLibHumanDescription.upperLegTwist,
        //                 skeleton = skeletonBones.Values.ToArray(),
        //                 human = humanBones
        //             };
        //             var avatar = AvatarBuilder.BuildHumanAvatar(assetLoaderContext.RootGameObject, humanDescription);
        //             avatar.name = $"{assetLoaderContext.RootGameObject.name}Avatar";
        //             animator.avatar = avatar;
        //         }
        //         assetLoaderContext.RootGameObject.transform.SetParent(parent, false);
        //         assetLoaderContext.RootGameObject.transform.position = rootGameObjectPosition;
        //         valid = animator.avatar.isValid || !assetLoaderContext.Options.ShowLoadingWarnings;
        //     }
        //     if (!valid)
        //     {
        //         Debug.LogWarning($"Could not create an Avatar for the model \"{(assetLoaderContext.Filename == null ? "Unknown" : FileUtils.GetShortFilename(assetLoaderContext.Filename))}\"");
        //     }
        // }
    }

}

