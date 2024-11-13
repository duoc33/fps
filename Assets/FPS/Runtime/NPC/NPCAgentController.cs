using System.Collections;
using NavMeshExtension;
using UnityEngine;
using UnityEngine.AI;
namespace FPS
{
    public class NPCAgentController : MonoBehaviour
    {
        public NavMeshAgentSO agentSO;
        private NavMeshAgent agent;
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        public void SetWalk()
        {
            agent.speed = agentSO.WalkSpeed - 0.01f;
        }
        public void SetRun()
        {
            agent.speed = agentSO.MaxSpeed;
        }
        public void SetStopping()
        {
            agent.speed = 0;
            agent.velocity = Vector3.zero;
        }
        public void SetDestination(Vector3 target,bool IgnoreStoppingDistance = false)
        {
            if(IgnoreStoppingDistance)
            {
                agent.stoppingDistance = 0;
            }
            else{
                agent.stoppingDistance = agentSO.StoppingDistance;
            }
            agent.SetDestination(target);
        }
        public bool IsStopped()=> agent.isStopped || agent.velocity.magnitude < 0.01f;
        public float GetCurrentNormalizedSpeed() => agentSO.GetNormalizedSpeed(agent.velocity.magnitude);

        public float GetHeight() => agent.height;


        
    }
}

