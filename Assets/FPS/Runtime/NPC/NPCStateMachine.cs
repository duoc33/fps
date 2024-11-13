using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KevinCastejon.FiniteStateMachine;
using RuntimeComponentAdjustTools;
using UnityEngine;
using UnityEngine.AI;
namespace FPS
{
    public enum NPCState
    {
        IdleWalk,
        Found,
        Dead,
    }
    public class NPCStateMachine : AbstractFiniteStateMachine
    {
        public int Health;
        public float IdleWalkTransitionTime = 5f;
        public float WalkOnceDistance = 15f;
        public float FoundOutPlayerDistance = 15f;
        public float ShootInterval = 2f;
        public float CorrectHitRate = 0.3f;
        public float HitOffsetMin = 1f;
        public float HitOffsetMax = 4f; 

        public void BeDamaged(int damage)
        {
            Health-=damage;
        }

        public Vector3 ModifyHitPos(Vector3 pos)
        {
            pos += CorrectHitRate > UnityEngine.Random.value ?  Vector3.zero : UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(HitOffsetMin , HitOffsetMax); // 随机偏移一下位置
            return pos;
        }

        // public WeaponSO weaponSO;
        
        #region Runtime
        public WeaponLogic npcWeapon;
        public NPCAnimatorController nPCAnimatorController;
        public NPCAgentController nPCAgent;

        #endregion
        void Awake()
        {
            Init(NPCState.IdleWalk,
                AbstractState.Create<NPCIdleWalkRunState, NPCState>(NPCState.IdleWalk, this),
                AbstractState.Create<NPCFoundState, NPCState>(NPCState.Found, this),
                AbstractState.Create<NPCDeadState, NPCState>(NPCState.Dead,this)
            );
            
            nPCAgent = GetComponent<NPCAgentController>();
            nPCAnimatorController = GetComponent<NPCAnimatorController>();
            
        }
        

        public bool DetectPlayer(out Vector3 playerPos)
        {
            playerPos = Vector3.zero;
            if(FPSPlayerState.player == null || FPSPlayerState.player.activeInHierarchy == false)
            {
                return false;
            }
            playerPos = FPSPlayerState.player.transform.position;
            float distance = Vector3.Distance(playerPos,transform.position);
            if(distance < FoundOutPlayerDistance)
            {
                return true;
                // Physics.Raycast(new Ray(transform.position + Vector3.up * nPCAgent.GetHeight() / 0.7f, dir ) , FoundOutPlayerDistance , );
            }

            return false;
        }

        public bool IsDead() => Health <= 0;
    }


    public class NPCIdleWalkRunState : AbstractState
    {
        private NPCStateMachine stateMachine=> GetStateMachine<NPCStateMachine>();
        private NPCAgentController agent => GetStateMachine<NPCStateMachine>().nPCAgent;
        private NPCAnimatorController animator => GetStateMachine<NPCStateMachine>().nPCAnimatorController;
        private Coroutine IdleWalkCoroutine;
        public override void OnEnter()
        {
            animator.StopFire();
            IdleWalkCoroutine = stateMachine.StartCoroutine(RandomWalkCoroutine(stateMachine.IdleWalkTransitionTime,stateMachine.WalkOnceDistance));
        }
        public override void OnUpdate()
        {
            float speed = agent.GetCurrentNormalizedSpeed();
            animator.PlayIdleWalkRun(speed);
            if(stateMachine.IsDead())
            {
                TransitionToState(NPCState.Dead);
                return;
            }
            if(stateMachine.DetectPlayer(out Vector3 playerPos))
            {
                TransitionToState(NPCState.Found);
            }
        }
        public override void OnExit()
        {
            stateMachine.StopCoroutine(IdleWalkCoroutine);
            IdleWalkCoroutine = null;
        }
        private float AvoidBugTime = 10f; // 防止卡死
        private IEnumerator RandomWalkCoroutine(float IdleWalkTransitionTime,float WalkDestinationDistance)
        {
            while (true)
            {
                // 进入随机行走状态
                agent.SetWalk();

                Vector3 randomDestination = NavMeshTool.GetRandomPointWithinNavMesh(agent.transform.position,WalkDestinationDistance);

                if (randomDestination != Vector3.zero)
                {
                    agent.SetDestination(randomDestination , true);
                }

                UniTask task1 = UniTask.Delay(TimeSpan.FromSeconds(AvoidBugTime));
                
                UniTask task2 = UniTask.WaitUntil(()=> randomDestination.magnitude - agent.transform.position.magnitude < 0.5f );
                
                IEnumerator enumerator = UniTask.WhenAny(task1,task2).ToCoroutine();
                
                yield return enumerator;

                // 进入待机状态
                agent.SetStopping();

                // 等待Idle时间
                yield return new WaitForSeconds(IdleWalkTransitionTime);
            }
        }

    }
    public class NPCFoundState : AbstractState
    {
        private NPCStateMachine stateMachine=> GetStateMachine<NPCStateMachine>();
        private NPCAgentController agent => GetStateMachine<NPCStateMachine>().nPCAgent;
        private NPCAnimatorController animator => GetStateMachine<NPCStateMachine>().nPCAnimatorController;
        float shootInterval;
        float elapseTime = 0;
        Transform npc;
        public override void OnEnter()
        {
            npc = stateMachine.transform;
            shootInterval = stateMachine.ShootInterval;
            elapseTime = Time.time;
            agent.SetRun();
        }
        public override void OnUpdate()
        {
            float speed = agent.GetCurrentNormalizedSpeed();
            animator.PlayIdleWalkRun(speed);
            if(stateMachine.IsDead())
            {
                TransitionToState(NPCState.Dead);
                return;
            }
            if(stateMachine.DetectPlayer(out Vector3 playerPos))
            {
                agent.SetDestination(playerPos);
                if(Time.time - elapseTime > shootInterval)
                {
                    Vector3 dir = (playerPos - npc.position).normalized;
                    dir.y = 0;
                    npc.rotation = npc.rotation * Quaternion.FromToRotation(npc.forward, dir);
                    animator.PlayFire();
                    playerPos = stateMachine.ModifyHitPos(playerPos);
                    stateMachine.npcWeapon.transform.LookAt(playerPos);
                    stateMachine.npcWeapon.Fire();
                    elapseTime = Time.time;
                }
                // else{
                //     animator.StopFire();
                //     stateMachine.npcWeapon.UnFire();
                // }
            }
            else{
                TransitionToState(NPCState.IdleWalk);
            }
        }
        public override void OnExit()
        {
            animator.StopFire();
            stateMachine.npcWeapon.UnFire();
        }
    }
    public class NPCDeadState : AbstractState
    {
        private NPCStateMachine stateMachine=> GetStateMachine<NPCStateMachine>();
        private NPCAgentController agent => GetStateMachine<NPCStateMachine>().nPCAgent;
        private NPCAnimatorController animator => GetStateMachine<NPCStateMachine>().nPCAnimatorController;

        bool isDeadCountDown = false;
        public override void OnEnter()
        {
            isDeadCountDown = false;
            agent.SetStopping();
            animator.PlayDeath();
            base.OnEnter();
        }
        public override void OnUpdate()
        {
            if(animator.GetCurrentAnimNormalizedTime() > 0.9f)
            {
                if(!isDeadCountDown)
                {
                    UnityEngine.Object.Destroy(stateMachine.gameObject,1f);
                    isDeadCountDown = true;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }

}

