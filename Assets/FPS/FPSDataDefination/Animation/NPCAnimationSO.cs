using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class NPCAnimationSO : SOBase
    {
        public string IdleAnimUrl = "";
        public string WalkAnimUrl = "";
        public string RunAnimUrl = "";
        public string FireAnimUrl = "";
        public string DeathAnimUrl = "";
        

        private static string templatePath = "FPSAnimationAssets/NPCTemplate/FPSNPCAnim"; 
        private static RuntimeAnimatorController runtimeAnimatorController;
        private static string IdleClipInnerName = "Idle";
        private static string WalkClipInnerName = "Walk";
        private static string RunClipInnerName = "Run";
        private static string FireClipInnerName = "Fire";
        private static string DeathClipInnerName = "Death";


        private AnimatorOverrideController animatorOverrideController;
        /// <summary>
        /// 显然需要 提前Avatar确定制作好
        /// </summary>
        /// <returns></returns>
        public override async UniTask Download()
        {
            runtimeAnimatorController ??= Resources.Load<RuntimeAnimatorController>(templatePath);
            
            AnimationClip IdleClip = await DownloadAnimationClipHandler(IdleAnimUrl);
            AnimationClip WalkClip = await DownloadAnimationClipHandler(WalkAnimUrl);
            AnimationClip RunClip = await DownloadAnimationClipHandler(RunAnimUrl);
            AnimationClip FireClip = await DownloadAnimationClipHandler(FireAnimUrl);
            AnimationClip DeathClip = await DownloadAnimationClipHandler(DeathAnimUrl);

            animatorOverrideController = new AnimatorOverrideController(runtimeAnimatorController);
            animatorOverrideController.name = "NPC Override";
            animatorOverrideController[IdleClipInnerName] = IdleClip;
            animatorOverrideController[WalkClipInnerName] = WalkClip;
            animatorOverrideController[RunClipInnerName] = RunClip;
            animatorOverrideController[FireClipInnerName] = FireClip;
            animatorOverrideController[DeathClipInnerName] = DeathClip;
        }
        public void Apply(GameObject target)
        {
            Animator animator = target.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            target.AddComponent<NPCAnimatorController>();
        }

        public override void OnDestroy()
        {
            Destroy(animatorOverrideController);
        }
    }
}

