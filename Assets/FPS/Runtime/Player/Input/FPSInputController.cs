using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace FPS
{
    /// <summary>
    /// 输入相关的控制，注意bool值的逻辑是不一样的。
    /// </summary>
    public class FPSInputController : MonoBehaviour
    {
        public InputActionAsset inputActionsAsset;

        // [JsonIgnore]
        public UnityEvent<bool> OnFireCallback;
        // [JsonIgnore]
        public UnityEvent OnReloadCallback;

        #region Input Values
        public bool isJumping;
        public bool isReloading;
        public bool isFiring;
        public bool isSprinting;
        public Vector2 move;
        public Vector2 look;
        
        /// <summary>
        /// 主要是为了以后可能有其他设备，但是不太可能，影响的是look
        /// </summary>
        public bool IsKeyMouse => playerInput.currentControlScheme == "KeyboardMouse";
        #endregion

        #region 暂时没
        public bool isAiming;
        public bool isCrouching = false; // 下蹲
        #endregion

        private PlayerInput playerInput;
        
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            inputActionsAsset.Enable();
        }

        void OnDestroy()
        {
            inputActionsAsset.Disable();
        }

        #region Input System Callbacks

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            look = value.Get<Vector2>();
        }

        #region Pass Through

        public void OnSprint(InputValue value)
        {
            isSprinting = value.isPressed;
        }
        public void OnAim(InputValue value)
        {
            isAiming = value.isPressed;
        }
        public void OnFire(InputValue value)
        {
            isFiring = value.isPressed;
            OnFireCallback?.Invoke(isFiring);
        }

        public void OnCrouch(InputValue value)
        {
            isCrouching = value.isPressed;
        }

        #endregion

        #region Trigger
        public void OnJump(InputValue value)
        {
            isJumping = value.isPressed;
        }

        public void OnReload(InputValue value)
        {
            isReloading = value.isPressed;
            OnReloadCallback?.Invoke();
        }
        #endregion



        #endregion


        private void OnApplicationFocus(bool hasFocus)
		{
            Cursor.lockState = CursorLockMode.Locked;
		}
    }
}

