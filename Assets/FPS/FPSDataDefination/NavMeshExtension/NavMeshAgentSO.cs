using RuntimeComponentAdjustTools;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshExtension
{
    /// <summary>
    /// Agent Step Height 够了 ，navMeshSurface.GetBuildSettings()全局掌控
    /// </summary>
    public class NavMeshAgentSO : ScriptableObject
    {
        /// <summary>
        /// 最大速度，游戏中的一单元/s ,
        /// </summary>
        public float MaxSpeed = 3.5f; // 2 ~ 8 , 人类的速度范围

        public float WalkSpeed = 1f; // 普通速度 , 一定是小于MaxSpeed的

        /// <summary>
        /// 每秒转动多少度
        /// </summary>
        public float AngularSpeed = 120;

        /// <summary>
        /// 加速度，游戏中的一单元/s^2 , Speed = 3.5, Acceleration = 8 , 也就是 Speed /Acceleration =   0.4375 秒到达最大速度 , 至于距离什么的物理公式算一下吧。
        /// </summary>
        public float Acceleration = 8; // 人类的加速度范围，不太了解。

        /// <summary>
        /// 停止距离
        /// </summary>
        public float StoppingDistance = 5;



        // /// <summary>
        // /// 代表导航代理（如AI角色）的半径，影响其在路径规划中的碰撞范围。可以用Bounds自适应该大小
        // /// </summary>
        // private float Radius;
        // /// <summary>
        // /// 模型高度匹配较好，同理使用bounds参数，设置高度范围
        // /// </summary>
        // private float Height;

        /// <summary>
        /// 需要基于底部pivot
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="bounds"></param>
        /// <param name="isTpos"></param>
        public void Apply(GameObject Target,Bounds bounds = default,bool isTpos = true)
        {
            if(bounds.Equals(default))
            {
                bounds = BoundsTool.CalculateBounds(Target.gameObject);
            }
            Target.TryGetComponent(out NavMeshAgent agent);
            agent ??= Target.AddComponent<NavMeshAgent>();
            agent.speed = MaxSpeed;
            agent.angularSpeed = AngularSpeed;
            agent.acceleration = Acceleration;
            agent.stoppingDistance = StoppingDistance;

            agent.autoBraking = true;
            agent.autoTraverseOffMeshLink = true;
            agent.autoRepath = true;
            agent.areaMask = NavMesh.AllAreas;
            agent.radius = isTpos ? bounds.size.x / 4 : bounds.size.x / 2;
            agent.height = bounds.size.y;

            if(MaxSpeed  < WalkSpeed){
                WalkSpeed = MaxSpeed /  2.0f;
            }
        }

        /// <summary>
        /// 归一化0~1的速度，主要用于动画状态控制
        /// </summary>
        /// <param name="currnetSpeed"></param>
        /// <returns></returns>
        public float GetNormalizedSpeed(float currnetSpeed) => MapThree(currnetSpeed,WalkSpeed,MaxSpeed,0f,0.5f,1f);
    
        float MapThree(float va, float va1, float va2,float start,float mid ,float end)
        {
            va = Mathf.Abs(va);
            float mappedValue = 0f;
            if (va < va1)
            {
                mappedValue = Mathf.Lerp(start, mid, va / va1);
            }
            else if (va >= va1 && va <= va2)
            {
                mappedValue = Mathf.Lerp(mid, end, (va - va1) / (va2 - va));
            }
            else if (va > va2)
            {
                mappedValue = 1f; 
            }
            return mappedValue;
        }
    
    
    }
}

