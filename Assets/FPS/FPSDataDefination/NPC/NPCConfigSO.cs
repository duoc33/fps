using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NavMeshExtension;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.AI;
namespace FPS
{
    /// <summary>
    /// 默认NPC模型强制底部Pivot
    /// </summary>
    public class NPCConfigSO : SOBase
    {
        public string NPCUrl;
        public int GenerateCount;
        public NPCStateSO nPCStateSO;
        public NavMeshAgentSO navMeshAgentSO;
        public NPCAnimationSO nPCAnimationSO;
        public WeaponSO weaponSO;


        private GameObject Npc => GetPooledGameObject(NPCUrl);

        public override async UniTask Download()
        {
            await nPCAnimationSO.Download();
            await weaponSO.Download();
            await DownloadAndPostProcessGameObject(NPCUrl,OnPostProcess);
        }
        private GameObject OnPostProcess(GameObject target)
        {
            Bounds bounds = BoundsTool.CalculateBounds(target);
            GameObject Npc = BoundsTool.GetBestCapsuleColliderAndGetBottomPivot(target.transform, bounds);
            navMeshAgentSO.Apply(Npc,bounds);
            InitComponent(Npc);
            return Npc;
        }
        private void InitComponent(GameObject target)
        {
            target.AddComponent<NPCAgentController>().agentSO = navMeshAgentSO;
            nPCAnimationSO.Apply(target);
            NPCStateMachine npcStateMachine = target.AddComponent<NPCStateMachine>();
            npcStateMachine.npcWeapon = Instantiate(weaponSO.GetWeaponPrefab(), weaponSO.GetWeaponHolder(npcStateMachine.transform)).GetComponent<WeaponLogic>();
            npcStateMachine.npcWeapon.transform.localPosition = Vector3.zero;
            nPCStateSO.Apply(npcStateMachine);
        }

        public GameObject InstantiateNPC(Vector3 position) => Instantiate(Npc,position,Quaternion.identity);
    }
}
