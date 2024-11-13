using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS
{
    public class FPSWeaponAnimator : MonoBehaviour
    {
        private int baselayer;
        private int ReloadHash = Animator.StringToHash("t_reload");
        private int FireHash = Animator.StringToHash("t_fire");
        private string ReloadStateName = "Reload";
        private string FireStateName = "Fire";
        private Animator animator;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            baselayer = animator.GetLayerIndex("Base Layer");
        }
        public void Reload()
        {
            animator.SetTrigger(ReloadHash);
        }
        public void Fire()
        {
            if(IsReloading()) return;
            animator.SetTrigger(FireHash);
            animator.Play(FireStateName,baselayer,0);
        }
        public bool IsReloading() => animator.GetCurrentAnimatorStateInfo(baselayer).IsName(ReloadStateName);
        public bool IsFiring() => animator.GetCurrentAnimatorStateInfo(baselayer).IsName(FireStateName);
    }
}

