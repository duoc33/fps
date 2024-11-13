using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS
{
    public class BulletLogic : MonoBehaviour
    {
        public float BulletSpeed;
        public int Damage;
        public float BulletLifeTime;

        bool IsEmit;
        Vector3 dir;
        void Awake()
        {
            dir = Vector3.zero;
            IsEmit = false;
        }
        public void Emit()
        {
            // transform.rotation = Quaternion.LookRotation(FireDire);
            // dir = FireDire;
            IsEmit = true;
        }
        public void Emit(Vector3 dir)
        {
            // transform.rotation = Quaternion.LookRotation(FireDire);
            // dir = FireDire;
            this.dir = dir.normalized;
            IsEmit = true;
        }
        float timeElapse = 0;
        void FixedUpdate()
        {
            if(!IsEmit) return;
            timeElapse += Time.deltaTime;
            // 子弹沿着方向移动
            if(dir!=Vector3.zero)
            {
                transform.position += dir * BulletSpeed * Time.deltaTime;
            }
            else{
                transform.position += transform.forward * BulletSpeed * Time.deltaTime;
            }
            
            if(timeElapse >= BulletLifeTime)
            {
                Destroy(gameObject);
                // Debug.Log("BulletLifeTime Ends");
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject==null) return;

            other.gameObject.TryGetComponent(out FPSPlayerState pSState);
            pSState?.DoDamage(Damage);
            other.gameObject.TryGetComponent(out NPCStateMachine nPCStateMachine);
            nPCStateMachine?.BeDamaged(Damage);

            Debug.Log("Hit People : " + other.gameObject.name);
            Destroy(gameObject);
            // Debug.Log("Bullet Hit");
        }
    }
}

