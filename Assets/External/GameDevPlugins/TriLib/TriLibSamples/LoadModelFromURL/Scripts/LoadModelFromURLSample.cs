using System.Collections;
using UnityEngine;
namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents a sample that loads a compressed (Zipped) Model.
    /// </summary>
    public class LoadModelFromURLSample : MonoBehaviour
    {
        /// <summary>
        /// The Model URL.
        /// </summary>
        public string ModelURL = "https://ricardoreis.net/trilib/demos/sample/TriLibSampleModel.zip";

        /// <summary>
        /// Cached Asset Loader Options instance.
        /// </summary>
        private AssetLoaderOptions _assetLoaderOptions;

        /// <summary>
        /// Creates the AssetLoaderOptions instance, configures the Web Request, and downloads the Model.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        private IEnumerator Start()
        {
            if (_assetLoaderOptions == null)
            {                
                _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                AutodeskInteractiveMaterialsHelper.Setup(ref _assetLoaderOptions);
                _assetLoaderOptions.AnimationType = General.AnimationType.Humanoid;
                _assetLoaderOptions.AvatarDefinition = General.AvatarDefinitionType.CreateFromThisModel;
                _assetLoaderOptions.HumanoidAvatarMapper = Resources.Load<Mappers.HumanoidAvatarMapper>("Mappers/Avatar/MixamoAndBipedByNameHumanoidAvatarMapper");
            }
            
            ModelURL = "http://127.0.0.1:8888/UnityRobot.zip";
            var webRequest = AssetDownloader.CreateWebRequest(ModelURL);
            yield return AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);
            
        }

        /// <summary>
        /// Called when any error occurs.
        /// </summary>
        /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            Debug.Log($"Loading Model. Progress: {progress:P}");
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Materials loaded. Model fully loaded.");
            // assetLoaderContext.RootGameObject.TryGetComponent(out Animation component);
            // Destroy(component);
            
        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            
            Debug.Log("Model loaded. Loading materials.");
        }
    }
}
