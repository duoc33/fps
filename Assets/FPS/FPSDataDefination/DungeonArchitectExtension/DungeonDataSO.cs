using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Themeing;
using NavMeshExtension;
using ScriptableObjectBase;
using UnityEngine;
//同一种类型的节点下的不同节点被选择规则，从第一个节点概率算起，如果第一个为1，则绝不会执行剩下的节点。从左往右
//物品的基准是Z轴方向与摄像机反方向,复杂物体使用MeshCollider，简单物体使用BoxCollider，且Pivot在底部中心位置。
namespace DungeonArchitectExtension
{
    //Gameobject.IsStatic = true 不过这段代码要在游戏物体实例化之前运行才能得到优化，否则不会得到优化。
    public class DungeonDataSO : SOBase
    {
        /// <summary>
        /// 配置生成规律相关的
        /// </summary>
        public GridDungeonConfigSO gridDungeonConfigSO;
        /// <summary>
        /// 配置生成的物体相关的 , 第一次下载完成后不可再修改。
        /// </summary>
        public DungeonThemeSO dungeonThemeSO;
        /// <summary>
        /// 网格烘焙调整相关的设置
        /// </summary>
        public NavMeshSO groundNavMeshSO;
        private GameObject DungeonGenerator;
        private GameObject DungeonItemsPool;
        private Dungeon dungeon;
        public Bounds GetMapBounds() => groundNavMeshSO.mapBounds;

        public async UniTask BuildAsync()
        {
            gridDungeonConfigSO.Apply(dungeon.GetComponent<GridDungeonConfig>());
            dungeon.BuildModifyByCC(new List<DungeonThemeData>() {dungeonThemeSO.Data} , false);
            await UniTask.WaitUntil(() => dungeon.IsCompleted);
            dungeon.IsCompleted = false;
            await groundNavMeshSO.ApplyNavMeshAsync(DungeonItemsPool);
        }

        public override async UniTask Download()
        {
            Init();
            await UniTask.Yield();
            await dungeonThemeSO.Download();
        }

        public void DestoryGrid()
        {
            dungeon.DestroyDungeon();
            groundNavMeshSO.Release();
        }

        public override void OnDestroy()
        {
            groundNavMeshSO.Release();
            // dungeon.DestroyDungeon();
            Destroy(DungeonItemsPool);
            Destroy(DungeonGenerator);
        }

        private void Init()
        {
            DungeonGenerator = Instantiate(Resources.Load<GameObject>("DungeonArchitectExtension/DungeonGrid")); 
            DungeonGenerator.name = "DungeonGenerator";
            GameObject temp = new GameObject();
            temp.isStatic = true;
            DungeonItemsPool = Instantiate(temp);
            DungeonItemsPool.name = "DungeonItemsPool";
            DungeonGenerator.GetComponent<PooledDungeonSceneProvider>().itemParent = DungeonItemsPool;
            dungeon = DungeonGenerator.GetComponent<Dungeon>();
            dungeon.name = "DungeonGrid";
            Destroy(temp);
        }
    }
}

