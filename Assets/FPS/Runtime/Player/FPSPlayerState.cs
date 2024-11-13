using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS
{
    public class FPSPlayerState : MonoBehaviour
    {
        public PlayerStateSO playerStateSO;
        public int Health;
        public float Height;
        public float Width;
        public static GameObject player;
        void Awake()
        {
            player = this.gameObject;
        }
        private int health = 0;
        public void DoDamage(int damage)
        {
            health -= damage;
            if(health <=0)
            {
                // Destroy(gameObject);
            }
        }
    }
}

