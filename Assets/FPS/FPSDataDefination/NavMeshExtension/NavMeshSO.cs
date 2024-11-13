using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuntimeComponentAdjustTools;
using UnityEngine;
namespace NavMeshExtension
{
    public class NavMeshSO : ScriptableObject
    {
        /// <summary>
        /// 定义网格和地形边缘的不可烘焙的距离
        /// </summary>
        public float Radius = 0.3f;
        /// <summary>
        /// 定义可以被烘焙最大高度
        /// </summary>
        public float Height = 2;
        /// <summary>
        /// 定义可以烘焙的最大坡度
        /// </summary>
        public float MaxSlope = 55;

        /// <summary>
        /// 最大能能够跳跃的垂直高度
        /// </summary>
        public float MaxClimb = 0.85f;

        [JsonIgnore]
        public Bounds mapBounds => relativeBuilder.GetBounds();
        private NavMeshRelativeBuilder relativeBuilder;
        
        public async UniTask ApplyNavMeshAsync(GameObject mapParent)
        {
            relativeBuilder ??= new NavMeshRelativeBuilder();
            relativeBuilder.AgentClimb = MaxClimb;
            relativeBuilder.AgentHeight =Height;
            relativeBuilder.AgentRadius =Radius;
            relativeBuilder.AgentSlope = MaxSlope;
            await relativeBuilder.BuildNavMeshAsync(mapParent);
        }

        public void Release()
        {
            relativeBuilder?.DestroyNavMesh();
            relativeBuilder = null;
        }
    }
}
