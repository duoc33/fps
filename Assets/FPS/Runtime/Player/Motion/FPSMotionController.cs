using System.Collections;
using System.Collections.Generic;
using RuntimeComponentAdjustTools;
using UnityEditor;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 与其他FPSController不同的是，通过bounds自动配置CharacterController。
    /// 通常 T pos 人物为标准。不在0，0，0有问题
    /// </summary>
    public class FPSMotionController : MonoBehaviour
    {
        public GameObject FailedPanel;
        public PlayerMotionSO playerMotionSO;
        FPSInputController fpsInput;
        CharacterController characterController;
        public CameraDithering cameraDithering;

        public Transform CameraTransform;

        void Awake()
        {
            fpsInput = gameObject.GetComponent<FPSInputController>();
            characterController = gameObject.GetComponent<CharacterController>();
        }
        private float OutOfMapTime = 10f;
        void Update()
        {
            if(playerMotionSO.InAirTime > OutOfMapTime)
            {
                FailedPanel?.SetActive(true);
            }
            ApplayMoveAndjump();
            ApplyCouch();
        }
        void LateUpdate()
        {
            ApplayRotation();
        }

        private void Dithering(Vector3 motion)
        {
            // cameraDithering.Shake(50);
        }

        #region Move Jump
        private void ApplayMoveAndjump()
        {
            playerMotionSO.JumpAndGravity(characterController, ref fpsInput.isJumping);
            playerMotionSO.ApplyMotion(transform, characterController, ref fpsInput.move, ref fpsInput.isSprinting, Dithering);
        }
        #endregion



        #region Rotation 相关的 摄像机的上下使用存量 , 角色的左右旋转使用增量。 如果后续需要头部模型上下旋转再说。这么做是可以防止一些旋转的问题的。
        
        private void ApplyCouch()
        {
            // if (fpsInput.isCrouching && (!(characterController.height < characterHegiht)))
            // {
            //     characterController.height = characterController.height / 2.0f; 
            // }
            // else{
            //     characterController.height = characterHegiht;
            // }
            // fpsInput.isCrouching = false;
        }
        private void ApplayRotation()
        {
            var item = playerMotionSO.GetRotationDelta(fpsInput.look);
            ApplyCameraPitch(CameraTransform, item.Item1);
            ApplyCharacterYaw(transform, item.Item2);
        }
        private float _pitch;
        private void ApplyCameraPitch(Transform target, float pitch)
        {
            _pitch += pitch;
            _pitch = Mathf.Clamp(_pitch, -playerMotionSO.TopClamp, playerMotionSO.BottomClamp);
            target.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
        private void ApplyCharacterYaw(Transform target, float yaw)
        {
            target.Rotate(Vector3.up * yaw);
        }
        #endregion


    }
}

