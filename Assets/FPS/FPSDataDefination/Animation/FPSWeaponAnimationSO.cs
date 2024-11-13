using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class FPSWeaponAnimationSO : SOBase
    {
        public AnimatorOverrideController GetCurrentNPCAnimController() => animatorOverrideController;
        public string ReloadAnimUrl = "";
        public string FireAnimUrl = "";
        private static string templatePath = "FPSAnimationAssets/FPSWeaponTemplate/FPSWeaponAnim";
        private static RuntimeAnimatorController runtimeAnimatorController;
        private static string FireClipInnerName = "Fire";
        private static string ReloadClipInnerName = "Reload";

        private AnimatorOverrideController animatorOverrideController;
        public override async UniTask Download()
        {
            runtimeAnimatorController ??= Resources.Load<RuntimeAnimatorController>(templatePath);
            
            AnimationClip ReloadClip = await DownloadAnimationClipHandler(ReloadAnimUrl);
            AnimationClip FireClip = await DownloadAnimationClipHandler(FireAnimUrl);

            Debug.Log(runtimeAnimatorController);

            animatorOverrideController = new AnimatorOverrideController(runtimeAnimatorController);
            animatorOverrideController.name = "Player Override";
            animatorOverrideController[ReloadClipInnerName] = ReloadClip;
            animatorOverrideController[FireClipInnerName] = FireClip;
        }
        public void Apply(GameObject target)
        {
            Animator animator = target.GetComponentInChildren<Animator>();
            if(animator==null)
            {
                animator = target.AddComponent<Animator>();
            }
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = animatorOverrideController;
            target.AddComponent<FPSWeaponAnimator>();
        }

        public override void OnDestroy()
        {
            Destroy(animatorOverrideController);
        }
    }
}

