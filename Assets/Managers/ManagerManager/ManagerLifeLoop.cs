using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Managers
{
    public class ManagerLifeLoop: MonoBehaviour
    {
        [Header("Debug")] 
        public bool showDebugGUI;
        // public ManagerDebuger ManagerDebuger;
        
        [Range(0, 3)] public float latencyTime = 1f;
        public GameObjectRelationShip gameObjectRelationShip = new();

        // [Header("Runtime")]
        // public List<ExecutorBase> runtimeExecutors;
        
        [Header("Database Settings")] 
        public bool DatabaseInEditor = true;
        public string devAccountId = "uiduiduid";

        [Header("EventSystem")] 
        private EventSystem eventSystem;
        
        private void Awake()
        {
            // runtimeExecutors = FindObjectsOfType<ExecutorBase>().Where(t => t.enabled).ToList();
            EventSystemCreate();
            GameFlowCreate(); // todo: 临时
            // CameraManager.Init();
        }

        private void GameFlowCreate()
        {
            // var gameFlow = FindObjectOfType<GameFlowManager>();
            // if (gameFlow == null)
            // {
            //     gameFlow = gameObject.AddComponent<GameFlowManager>();
            // }
        }

        private void EventSystemCreate()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                var es = gameObject.AddComponent<EventSystem>();
                es.gameObject.AddComponent<StandaloneInputModule>();
            }
        }

        private async void Start()
        {
            UIManager.Init();
            // SystemPrepare();
        }

        private void Update()
        {
            GameUpdate();
        }
        





        #region Game State

        private bool mapLoadDone = false;
        private void GameUpdate()
        {
            if (!mapLoadDone)
            {
                mapLoadDone = true;
            }
        }
        

        #endregion





        #region EventSystem
        public async void DisableInputForDuration(float duration)
        {
            if(eventSystem == null) eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.enabled = false;
                await UniTask.Delay(TimeSpan.FromSeconds(duration));
                eventSystem.enabled = true;
            }
        }
        #endregion
    }

    public class GameObjectRelationShip
    {
    }
}