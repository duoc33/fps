using Cysharp.Threading.Tasks;
using Foundation;
using FPS;
using ScriptableObjectBase;
using UnityEngine;

public class Test : MonoBehaviour
{
    public FPSConfigSO fPSConfigSO;
    private FPSConfigSO fPSConfigSOTest;
    private void Awake()
    {
        // SOBase.InitLocal();
        ScriptableObject.CreateInstance<ConfigSOTest>();
    }
    private void Start()
    {
        // Serializer.WriteAllText(Serializer.Serialize(fPSConfigSO));
        // DownloadAndInit().Forget();
    }
    private async UniTask DownloadAndInit()
    {
        string json = await DLUtils.DownloadJson(Application.streamingAssetsPath + "/data.json");
        fPSConfigSOTest = Serializer.Deserialize<FPSConfigSO>(json);
        await fPSConfigSOTest.Download();
        fPSConfigSOTest.StartMixComponents();
    }

    private void OnDestroy()
    {
        // fPSConfigSOTest.OnDestroy();
        // SOBase.Clear();
    }
}

public class ConfigSOTest : SOBase
{
    // private void Awake()
    // {
    //     Debug.Log("ConfigSO Awake");
    // }
    // private void OnEnable()
    // {
    //     Debug.Log("ConfigSO OnEnable");
    // }
    // private void OnDisable()
    // {
    //     Debug.Log("ConfigSO OnDisable");
    // }
    // private void OnDestroy() {
    //     Debug.Log("ConfigSO Destroy");
    // }
}


