using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Builders.Grid;
using UnityEngine;
namespace DungeonArchitectExtension
{
    /// <summary>
    /// 有概率生成不出来，就是因为规则互相限制导致。
    /// </summary>
    public class GridDungeonConfigSO : ScriptableObject
    {
        public int Seed; // 0 ~ int.MaxValue 
        /// <summary>
        /// 注意Y值，Y值就是每个格子可能的高度差，所以连接处的楼梯的模型需要考虑这个高度差，至少比它高。
        /// </summary>
        public Vector3 GridCellSize = new Vector3(4, 2, 4);
        public int NumCells = 150;
        /// <summary>
        /// “随机矩形大小范围” , 宽度和高度由下面两个参数之中随机选择。
        /// GridDungeon中有走廊，房间等，下面两个参数限定的是走廊、房间或空缺，一个矩形中的长宽 或 高度 的最小数量。关键词：“矩形中”
        /// </summary>
        public int MinCellSize = 3;
        /// <summary>
        /// GridDungeon中有走廊，房间等，下面两个参数限定的是走廊、房间或空缺，一个矩形中的长宽 或高度 的最大数量。
        /// </summary>
        public int MaxCellSize = 5;

        private int MaxCellSizeLimit = 10;

        /// <summary>
        /// RoomAreaThreshold（房间面积阈值）：
        /// 这个字段设定了一个面积阈值，如果单元的大小超过这个阈值，它将被转换为房间。
        /// 在单元被提升为房间后，所有房间通过走廊连接在一起（无论是直接还是间接连接)
        /// 
        /// 这个值与MinCellSize，MaxCellSize密切相关，假如Builder在选择生成CellSize时，为MaxCellSize * MaxCellSize
        /// 而MaxCellSize * MaxCellSize 小于 RoomAreaThreshold， 那么地图永远生成不出来，
        /// 因为这个GridBuilder的逻辑时，先随机生成房间，再生成走廊。
        /// 
        /// 显然MinCellSize 与 而MaxCellSize 如果一样的话，而房间面积阈值又很小，房间会很密集。
        /// 
        /// 最好的实践，MinCellSize，MaxCellSize之间保持一定跨度，RoomAreaThreshold 适当小于 MaxCellSize * MaxCellSize。
        /// </summary>
        public int RoomAreaThreshold = 20;

        /// <summary>
        /// RoomAspectDelta（房间纵横比）：
        /// 这个字段表示单元的纵横比（宽高比）。保持这个值接近0会生成方形的房间，而接近1则会生成宽长或拉伸的房间，具有较高的宽高比。
        /// 
        /// 它同样与RoomAreaThreshold，MaxCellSize，MinCellSize有关系，RoomAreaThreshold。
        /// </summary>
        public float RoomAspectDelta = 0.4f; // 0 ~ 1

        /// <summary>
        /// 高度变化的概率
        /// </summary>
        public float HeightVariationProbability = 0.3f; // 0 ~ 1


        /// <summary>
        /// 这个字段 SpanningTreeLoopProbability 用于控制地下城生成时循环的数量。具体来说：
        /// 值接近 0：会生成较少的循环，导致地下城的结构更线性，路径较为直接。这种设置通常适合于希望设计出相对简单的地下城，可能让玩家更容易找到出路。
        /// 值接近 1：会生成大量循环，导致地下城的结构显得过于复杂和不原创。这可能使得玩家在探索时感到迷失，且对游戏体验产生负面影响。
        /// 推荐值：建议将这个值设置在较低的范围，比如 0.2，这样可以确保生成的地下城既有一些循环增加探索乐趣，又不会让结构过于复杂或混乱。
        /// </summary>
        public float SpanningTreeLoopProbability = 0.2f; // 0 ~ 1

        /// <summary>
        /// 生成器将添加楼梯以使地牢的不同区域无障碍。但是，我们不希望有太多的楼梯。
        /// 例如，在特定高架区域添加楼梯之前，生成器将检查该区域是否已可从附近的楼梯进入。
        /// 如果是这样，它不会添加它。此 tolerance 参数确定在添加楼梯之前要查找现有路径的距离。
        /// 如果您看到太多楼梯彼此靠近或太少，请使用此参数.
        /// 
        /// 简单来说，区域之间连接需要楼梯，如果值越大，那么楼梯的数量越少，越小，可能单个房间连接的楼梯越多。
        /// </summary>
        public float StairConnectionTollerance = 6; //0 ~ 10


        /// <summary>
        /// 连接房间的走廊额外增加的宽度
        /// </summary>
        public int CorridorWidth = 2; // 0 ~ 10

        /// <summary>
        /// 一个房间生成门的紧密程度，如果为0，则可能生成紧挨着的门，如果值越大的话，生成的门如果靠得太近会被剔除。
        /// </summary>
        public float DoorProximitySteps = 3; // 0 ~ 10

        /// <summary>
        /// 正态分布均值。
        /// 正态分布：正态分布意味着生成的随机值更可能接近均值（NormalMean），而远离均值的值出现的频率较低。因此，当你设定均值为 0 时，生成的值通常会围绕 0 这个值分布。
        /// 直接的理解就是：当 NormalMean 的值增大时，生成的房间数量会增加，而走廊数量会减少。这是因为均值越高，生成的随机数越可能偏向于更高的值，从而导致更多的房间被创建。
        /// </summary>
        public float NormalMean = 0; // -1 ~ 1 ，注意当为-1时，地图生成失败，因为这个地图生成是由房间为着手点连接的，也就是-1的话，只生成走廊是没办法形成地图的。


        /// <summary>
        /// 正态分布标准差
        /// 标准差的作用：
        /// 较小的标准差：生成的随机值将更集中在均值附近，结果会更加一致，导致房间和走廊的数量差异较小。
        /// 较大的标准差：生成的随机值会更分散，导致房间和走廊数量的变化更大，可能会生成更多的房间或走廊，增加布局的多样性。
        /// 别设为0，否则会导致生成失败或卡死，原因还是刚才说的，该地图生成的原理是房间为着手点连接的，如果0的话，它们没有数量差异，大概率会卡死失败。
        /// </summary>
        public float NormalStd = 0.3f;//0.25 ~ 1


        /// <summary>
        /// Wall会生成在边缘，而不是像地板那样在中心。
        /// </summary>
        private GridDungeonWallType WallLayoutType = GridDungeonWallType.WallsAsEdges;

        /// <summary>
        /// 与GridCellsize的Y值相关，也与美术模型的Bounds楼梯高度有关，1表示模型遵从，允许一层高度。2表示允许两层高度。
        /// 但是对应要有两层高度的美术模型。在该Builder主题中的Stairs2X部分。
        /// </summary>
        private int MaxAllowedStairHeight = 1;

        /// <summary>
        /// 里面的数字，全是测的效果得的结论。。。尽可能保证生成成功
        /// </summary>
        /// <param name="gridDungeonConfig"></param>
        public void Apply(GridDungeonConfig gridDungeonConfig)
        {
            gridDungeonConfig.Seed = (uint)Seed;
            gridDungeonConfig.NumCells = NumCells;
            gridDungeonConfig.GridCellSize = GridCellSize;
            gridDungeonConfig.MinCellSize = Mathf.Clamp(MinCellSize, 1 , MaxCellSizeLimit);
            gridDungeonConfig.MaxCellSize = Mathf.Clamp(MaxCellSize , MinCellSize, MaxCellSizeLimit);
            gridDungeonConfig.RoomAreaThreshold = Mathf.Clamp(RoomAreaThreshold, 4 , MaxCellSize * MaxCellSize);
            gridDungeonConfig.RoomAspectDelta = Mathf.Clamp(RoomAspectDelta,0,1f);
            gridDungeonConfig.HeightVariationProbability = Mathf.Clamp(HeightVariationProbability,0,1f);
            gridDungeonConfig.SpanningTreeLoopProbability = Mathf.Clamp(SpanningTreeLoopProbability,0,1f);
            gridDungeonConfig.StairConnectionTollerance = Mathf.Clamp(StairConnectionTollerance,0,10);
            gridDungeonConfig.CorridorWidth = Mathf.Clamp(CorridorWidth,0,10);
            gridDungeonConfig.DoorProximitySteps = Mathf.Clamp(DoorProximitySteps,0,10f);
            gridDungeonConfig.NormalMean = Mathf.Clamp(NormalMean,-0.8f,1f);
            gridDungeonConfig.NormalStd = Mathf.Clamp(NormalStd,0.25f,1f);
            gridDungeonConfig.WallLayoutType = WallLayoutType;
            gridDungeonConfig.MaxAllowedStairHeight = MaxAllowedStairHeight;
        }
    }

}
