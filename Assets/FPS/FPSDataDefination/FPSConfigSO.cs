using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonArchitectExtension;
using Newtonsoft.Json;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class FPSConfigSO : SOBase
    {
        public DungeonDataSO dungeonDataSO;
        public List<NPCConfigSO> nPCConfigSOs;
        public PlayerConfigCenterSO playerConfigCenterSO;
        [NonSerialized]
        [JsonIgnore]
        public LevelTargetSO levelTargetSO;
        [NonSerialized]
        [JsonIgnore]
        public FPSRuntimeUISO fPSRuntimeUISO;
        public override async UniTask Download()
        {
            await dungeonDataSO.Download();
            await dungeonDataSO.BuildAsync(); // Download 之后立马生成，不然，Player等Agent的初始化会出问题。
            foreach (NPCConfigSO npcConfigSO in nPCConfigSOs)
            {
                await npcConfigSO.Download();
            }
            await playerConfigCenterSO.Download();
            // await levelTargetSO.Download();
            // await fPSRuntimeUISO.Download();
            levelTargetSO??= CreateInstance<LevelTargetSO>();
            fPSRuntimeUISO??= CreateInstance<FPSRuntimeUISO>();
        }
        public override void StartMixComponents()
        {
            fPSRuntimeUISO.InitRuntimeUI();
            SpawnNPC();
            SpawnPlayer();
            SpawnTarget();
        }
        public override void OnDestroy()
        {
            dungeonDataSO.OnDestroy();
            foreach (NPCConfigSO npcConfigSO in nPCConfigSOs)
            {
                npcConfigSO.OnDestroy();
            }
            playerConfigCenterSO.OnDestroy();
            levelTargetSO.OnDestroy();
            fPSRuntimeUISO.OnDestroy();
            foreach (GameObject npc in npcs)
            {
                Destroy(npc);
            }
        }

        private List<GameObject> npcs = new List<GameObject>();
        private void SpawnNPC()
        {
            foreach (var npc in nPCConfigSOs)
            {
                for (int i = 0; i < npc.GenerateCount; i++)
                {
                    if (NavMeshTool.GetRandomPositionInBounds(dungeonDataSO.GetMapBounds(), out Vector3 pos))
                    {
                        npcs.Add(npc.InstantiateNPC(pos));
                    }
                }
            }
        }
        private void SpawnPlayer()
        {
            if (NavMeshTool.GetRandomPositionInBounds(dungeonDataSO.GetMapBounds(), out Vector3 pos))
            {
                GameObject player = playerConfigCenterSO.InitPlayer(pos + Vector3.up * 5);
                player.GetComponent<FPSMotionController>().FailedPanel = fPSRuntimeUISO.GetFailedPanel();
            }
        }
        private void SpawnTarget()
        {
            if (NavMeshTool.GetRandomPositionInBounds(dungeonDataSO.GetMapBounds(), out Vector3 pos))
            {
                levelTargetSO.GetTarget(pos).GetComponent<SuccessTrigger>().successPanelUI = fPSRuntimeUISO.GetSuccessPanel();
            }
        }
    }
}
