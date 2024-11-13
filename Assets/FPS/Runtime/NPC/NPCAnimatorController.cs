using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class NPCAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private int idleWalkRunHash = Animator.StringToHash("t_speed");
        private int fireHash = Animator.StringToHash("t_fire");
        // private string FireStateName = "Fire";
        private int deathHash = Animator.StringToHash("t_death");
        private int baseLayer;
        private int fireLayer;
        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            baseLayer = animator.GetLayerIndex("Base Layer");
            fireLayer = animator.GetLayerIndex("Fire Layer");
        }
        /// <summary>
        /// Normalized 的 speed ， 0~1之间 , 0.5f 代表 walk 
        /// </summary>
        /// <param name="speed"></param>
        public void PlayIdleWalkRun(float speed)
        {
            animator.SetFloat(idleWalkRunHash,speed);
        }
        public void PlayFire()
        {
            animator.SetBool(fireHash,true);
            // animator.Play(FireStateName,fireLayer,0);
        }
        public void StopFire()
        {
            animator.SetBool(fireHash,false);
        }
        public void PlayDeath()
        {
            animator.SetBool(fireHash,false);
            animator.SetFloat(idleWalkRunHash, 0);
            animator.SetLayerWeight(fireLayer,0);
            animator.SetBool(deathHash,true);
        }
        public float GetCurrentAnimNormalizedTime() => animator.GetCurrentAnimatorStateInfo(baseLayer).normalizedTime;
    }
}

