using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace FPS
{
    public class FPSRuntimeUICanvas : MonoBehaviour
    {
        public Sprite AimPointIcon;
        private TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {
            text = transform.Find("Distance").GetComponentInChildren<TextMeshProUGUI>();
            if(AimPointIcon != null)
            {
                transform.Find("AimPoint").GetComponentInChildren<Image>().sprite = AimPointIcon;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if(FPSPlayerState.player == null || SuccessTrigger.LevelTarget == null || text == null) 
            return;
            
            int value = (int)Vector3.Distance(SuccessTrigger.LevelTarget.transform.position, FPSPlayerState.player.transform.position);
            text.text = "Current Distance to Target: " + value.ToString() ;
        }
    }
}

