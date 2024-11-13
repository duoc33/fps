using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonArchitect.Themeing;
using Newtonsoft.Json;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonArchitectExtension
{
    public class ThemeItemSO : SOBase
    {
        public string Url;
        /// <summary>
        /// 该物品所属的类型，父插槽, 默认有如下类型，在ChildSockets中不能出现如下类型，但自定义任何字符串。
        /// Door,
        /// Wall,
        /// Fence,
        /// Stair,
        /// Stair2X,
        /// WallHalfSeparator,
        /// WallSeparator,
        /// WallHalf,
        /// </summary>
        [JsonIgnore]
        [NonSerialized]
        public GridDungeonBuilderMakerNodeType ParentSocket;
        [Header("当GridDungeonBuilderMakerNodeType 为Custom时 , 需要填写ParentSocketName")]
        public string ParentSocketName; // 父插槽的名称
        public bool IsNeedMeshCollider; //复杂物体，需要 , false 则使用 boxcollider
        [Header("Transform Offset , 主要用于如果有错误，进行调整")]
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        [NonSerialized]
        [JsonIgnore]
        public bool AffectsNavigation = true; // 决定是否烘焙
        public bool IsObstacle = false; // 是否障碍物，用于导航，false 则不生成障碍物，true 则生成障碍物，用于导航。

        [Range(0,1)]
        public float AttachmentProbability = 1.0f; // 0 ~ 1 生成概率 , 同makernode下奏效
        public bool ConsumeOnAttach = true; // 是否能消耗当前位置，禁止其他物品生成。true表示禁止，false表示可以在当前位置上继续生成。

        /// <summary>
        /// 这是自定义的，不能够等于Ground,Door,Wall,Fence,Stair,等等，只能是是非空的ParentSocketName元素
        /// </summary>
        public List<string> ChildSockets = new List<string>();
        private GameObject PooledTarget;

        public GameObjectDungeonThemeItem CreateThemeItem()
        {
            GameObjectDungeonThemeItem item = new GameObjectDungeonThemeItem();
            item.NodeId = System.Guid.NewGuid().ToString();
            item.Offset = GetMatrixOffset(Position, Rotation, Scale);
            item.affectsNavigation = AffectsNavigation;
            item.AttachToSocket = ParentSocket.Equals(GridDungeonBuilderMakerNodeType.Custom) ? ParentSocketName : ParentSocket.ToString();
            item.Affinity = AttachmentProbability;
            item.ConsumeOnAttach = ConsumeOnAttach;
            item.Template = PooledTarget;
            item.ChildSockets = new List<PropChildSocketData>();
            
            ChildSockets?.ForEach(s=>{
                item.ChildSockets.Add(new PropChildSocketData(){SocketType = s , Offset = Matrix4x4.identity});
            });

            item.useSpatialConstraint = false;
            item.spatialConstraint = null;
            item.UseSelectionRule = false;
            item.SelectorRuleClassName = "";
            item.UseTransformRule = false;
            item.TransformRuleClassName = "";
            return item;
        }

        private GameObject PostProcessGameObject(GameObject target)
        {
            target.isStatic =true;
            GameObject newParent =  null;
            Bounds bounds = BoundsTool.CalculateBounds(target);
            if(IsNeedMeshCollider)
            {
                BoundsTool.SetMeshColliderConvex(target);
                newParent = BoundsTool.GetBestBottomPivot(target,bounds);
            }
            else{
                newParent = BoundsTool.GetBestBoxColliderAndBottomPivot(target , bounds);
            }
            if(IsObstacle)
            {
                NavMeshObstacle obstacle = newParent.AddComponent<NavMeshObstacle>();
                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.carving = false;
                obstacle.center = Vector3.up * (bounds.size.y / 2);
                obstacle.size = bounds.size;
            }
            newParent.isStatic = true;
            PooledTarget = newParent;
            return newParent;
        }

        public override async UniTask Download()
        {
            await DownloadAndPostProcessGameObject(Url,PostProcessGameObject);
        }

        private Matrix4x4 GetMatrixOffset(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            int precision = 4;
            RoundVector(ref position, precision);
            RoundVector(ref rotation, precision);
            RoundVector(ref scale, precision);
            return Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
        }
        
        private static void RoundVector(ref Vector3 vector, int precision)
        {
            vector.x = Round(vector.x, precision);
            vector.y = Round(vector.y, precision);
            vector.z = Round(vector.z, precision);
        }
        
        private static float Round(float f, int precision)
        {
            var multiplier = Mathf.Pow(10, precision);
            return Mathf.Round(f * multiplier) / multiplier;
        }
    }

    
}

