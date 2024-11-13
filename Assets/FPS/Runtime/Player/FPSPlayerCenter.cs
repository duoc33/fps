using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    public class FPSPlayerCenter : MonoBehaviour
    {
        public Transform AimTransform;
        public WeaponLogic weaponLogic;
        private FPSInputController inputController;
        void Start()
        {
            inputController = GetComponent<FPSInputController>();
            inputController.OnFireCallback.AddListener(OnPlayerFire);
            inputController.OnReloadCallback.AddListener(OnPlayerReload);
        }

        private void OnPlayerFire(bool firing)
        {
            if(firing)
            {
                weaponLogic.PlayerFire(AimTransform.forward);
            }
        }
        private void OnPlayerReload()
        {
            weaponLogic.Reload();
        }
        void OnDestroy()
        {
            inputController.OnFireCallback.RemoveListener(OnPlayerFire);
            inputController.OnReloadCallback.RemoveListener(OnPlayerReload);
        }

    }
}