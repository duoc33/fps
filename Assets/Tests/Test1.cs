using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FPS;
using ScriptableObjectBase;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public PlayerConfigCenterSO playerConfigCenterSO;
    void Awake()
    {
        SOBase.InitLocal();
    }
    async void Start()
    {
        await playerConfigCenterSO.Download();
        // GameObject player = playerConfigCenterSO.InitPlayer();
        await UniTask.Yield();
    }

    void OnDestroy()
    {
        playerConfigCenterSO.OnDestroy();
        SOBase.Clear();
    }
}
