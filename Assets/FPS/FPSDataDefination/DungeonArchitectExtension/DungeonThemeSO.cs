using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonArchitect.Themeing;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
namespace DungeonArchitectExtension
{
    /// <summary>
    /// 注意！顺序非常重要 , 排在前边的概率会先执行,如果前边概率100%，则后面的对象执行生成
    /// </summary>
    public class DungeonThemeSO : SOBase
    {
        public List<ThemeItemSO> GroundItems = new List<ThemeItemSO>();
        public List<ThemeItemSO> WallItems = new List<ThemeItemSO>();
        public List<ThemeItemSO> DoorItems = new List<ThemeItemSO>();
        public List<ThemeItemSO> FenceItems = new List<ThemeItemSO>();
        public List<ThemeItemSO> StairItems = new List<ThemeItemSO>();
        public List<ThemeItemSO> OtherDecoItems = new List<ThemeItemSO>(); // 用来装饰上面的其他物品

        public override async UniTask Download()
        {
            _data = new DungeonThemeData();
            
            foreach (var item in GroundItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Ground;
                _data.Props.Add(item.CreateThemeItem());
            }
            foreach (var item in WallItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Wall;
                _data.Props.Add(item.CreateThemeItem());
            }
            foreach (var item in DoorItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Door;
                _data.Props.Add(item.CreateThemeItem());
            }
            foreach (var item in FenceItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Fence;
                _data.Props.Add(item.CreateThemeItem());
            }
            foreach (var item in StairItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Stair;
                _data.Props.Add(item.CreateThemeItem());
            }
            foreach(var item in OtherDecoItems)
            {
                await item.Download();
                item.ParentSocket = GridDungeonBuilderMakerNodeType.Custom;
                _data.Props.Add(item.CreateThemeItem());
            }
        }
        private DungeonThemeData _data;
        [JsonIgnore]
        public DungeonThemeData Data => _data;
    }
}

