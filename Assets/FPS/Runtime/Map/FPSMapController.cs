using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonArchitectExtension;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 初始化地图
    /// </summary>
    public class FPSMapController : MonoBehaviour
    {
        public DungeonDataSO dungeonDataSO;
        public async UniTask BuildAsync() => await dungeonDataSO.BuildAsync();
        public void Destroy() => dungeonDataSO.DestoryGrid();

        void OnDestroy() => dungeonDataSO.OnDestroy();
    }
}

