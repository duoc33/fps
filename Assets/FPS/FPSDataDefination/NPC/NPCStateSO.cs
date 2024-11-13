using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS
{
    /// <summary>
    /// 状态，准确度相关的。
    /// </summary>
    public class NPCStateSO : ScriptableObject
    {
        public int Health = 40;
        public float IdleWalkTransitionTime = 5f;
        public float WalkOnceDistance = 15f;
        public float FoundOutPlayerDistance = 5f;
        public float ShootInterval = 2f;
        public float CorrectHitRate = 0.3f;
        public float HitOffsetMin = 1f;
        public float HitOffsetMax = 4f; 

        public void Apply(NPCStateMachine target)
        {
            target.Health = Health;
            target.IdleWalkTransitionTime = IdleWalkTransitionTime;
            target.WalkOnceDistance = WalkOnceDistance;
            target.FoundOutPlayerDistance = FoundOutPlayerDistance;
            target.ShootInterval = ShootInterval;
            target.CorrectHitRate = CorrectHitRate;;
            target.HitOffsetMin = HitOffsetMin;
            target.HitOffsetMax = HitOffsetMax;
        }
    }
}
