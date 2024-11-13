using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FPS
{
    public class SuccessTrigger : MonoBehaviour
    {
        public static GameObject LevelTarget;
        public GameObject successPanelUI;
        void Awake()
        {
            LevelTarget = this.gameObject;
        }
        void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent(out FPSPlayerState component);
            if(component!=null && successPanelUI !=null)
            {
                successPanelUI.gameObject.SetActive(true);
            }
        }
        void OnDestroy()
        {
            Destroy(successPanelUI);
        }
    }
}

