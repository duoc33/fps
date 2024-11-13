using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class LevelTargetSO : SOBase
    {
        public string LevelTargetUrl = "Temp/FPSTarget/Target";
        // public override async UniTask Download()
        // {
        //     await DownloadModelHandler(LevelTargetUrl);
        // }
        public GameObject GetTarget(Vector3 position)
        {
            // GameObject target = Instantiate(GetDownloadedGameObject(LevelTargetUrl),Vector3.zero,Quaternion.identity);
            GameObject target = Instantiate(Resources.Load<GameObject>(LevelTargetUrl),Vector3.zero,Quaternion.identity);
            GameObject levelTarget = BoundsTool.GetBestBoxColliderAndBottomPivot(target);
            levelTarget.GetComponent<BoxCollider>().isTrigger = true;
            levelTarget.AddComponent<Rigidbody>().isKinematic = true;
            levelTarget.AddComponent<SuccessTrigger>();
            levelTarget.transform.position = position;
            return levelTarget;
        }
    }
}

