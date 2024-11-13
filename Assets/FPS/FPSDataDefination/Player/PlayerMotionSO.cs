using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class PlayerMotionSO : SOBase
    {
        /// <summary>
        /// 默认重力, 负的
        /// </summary>
        public float Gravity = -9.81f;
        public float JumpHeight = 1.2f;
        /// <summary>
        /// 向上跳跃的最大速度 ,限制
        /// </summary>
        public float TerminalVelocity = 60f;

        public float SprintSpeed = 6.0f;

        public float MoveSpeed = 4.0f;

        /// <summary>
        /// 每帧速度变化的步长,理解为加速度
        /// </summary>
        public float SpeedChangeRate = 10.0f;

        /// <summary>
        /// 下落判定时间，超过这个时间，则判定为在空中进行下落。
        /// 例如下楼梯，可能不需要判定为Falling，而是自然的往下走。
        /// </summary>
        public float FallJudgeTime = 0.15f;

        /// <summary>
        /// 不能连续落地了就连续起跳，更自然的时间。
        /// </summary>
        public float JumpIntervalTime = 0.1f;

        /// <summary>
        /// 旋转方向的速度
        /// </summary>
        public float RotationSpeed = 10.0f;

        /// <summary>
        /// Unity中世界空间中 Transform 是左手坐标系，所以绕x向下旋转是正，向上为负。
        /// 下面 TopClamp ， BottomClamp 已经做了处理，TopClamp就是向上限制的角度，BottomClamp就是向下限制的角度。
        /// </summary>
        public float TopClamp = 60.0f;
        public float BottomClamp = 60.0f;

        private float _cameraPith = 0.0f;
        private float _cameraYaw = 0.0f;

        private void ApplyCrouch()
        {

        }

        #region Rotation
        /// <summary>
        /// 获取增量的方式
        /// </summary>
        /// <param name="look"></param>
        /// <param name="IsCurrentDeviceMouse"></param>
        /// <returns></returns>
        public (float, float) GetRotationDelta(Vector2 look, bool IsCurrentDeviceMouse = true)
        {
            if (look.sqrMagnitude >= 0.01f)
            {
                //鼠标就不用时间Time.deltaTime来进行处理
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                return (look.y * RotationSpeed * deltaTimeMultiplier, look.x * RotationSpeed * deltaTimeMultiplier);
            }
            else
            {
                return (0.0f, 0.0f);
            }
        }
        /// <summary>
        /// 获取一次性Quaternion结果
        /// </summary>
        /// <param name="look"></param>
        /// <param name="IsCurrentDeviceMouse"></param>
        /// <returns></returns>
        public Quaternion GetRotation(Vector2 look, bool IsCurrentDeviceMouse = true)
        {
            if (look.sqrMagnitude >= 0.01f)
            {
                //鼠标就不用时间Time.deltaTime来进行处理
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cameraPith += look.y * RotationSpeed * deltaTimeMultiplier;
                _cameraYaw += look.x * RotationSpeed * deltaTimeMultiplier;

                ClampAngle();
                _cameraPith = Mathf.Clamp(_cameraPith, -TopClamp, BottomClamp);
            }
            return Quaternion.Euler(_cameraPith, _cameraYaw, 0.0f);
        }
        /// <summary>
        /// 角度一直增加无意义
        /// </summary>
        private void ClampAngle()
        {
            if (_cameraPith < -360) _cameraPith += 360;
            if (_cameraPith > 360) _cameraPith -= 360;
            if (_cameraYaw < -360f) _cameraYaw += 360f;
            if (_cameraYaw > 360f) _cameraYaw -= 360f;
        }

        #endregion

        #region Jump and Gravity
        private float _fallJudgeTime;
        private float _jumpIntervalTime;
        private float _verticalVelocity;

        [NonSerialized]
        public float InAirTime = 0;

        /// <summary>
        /// 模型底部，检测，跳跃检测。
        /// </summary>
        /// <param name="bottomPos"></param>
        /// <param name="radius"></param>
        /// <param name="jump"></param>
        /// <returns></returns>
        public void JumpAndGravity(CharacterController controller, ref bool jump)
        {
            // Vector3 bottomPos = target.TransformPoint(controller.center) - target.up * ( controller.height / 2.0f );
            // float groundCheckRadius = controller.skinWidth;
            if (controller.isGrounded)
            {
                _fallJudgeTime = FallJudgeTime;

                //重制速度，为-2f;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (jump && _jumpIntervalTime <= 0.0f)
                {
                    //开始跳跃
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity); // 向上所以是正的
                }

                // jump timeout
                if (_jumpIntervalTime >= 0.0f)
                {
                    //连续起跳间隔
                    _jumpIntervalTime -= Time.deltaTime;
                }
                InAirTime = 0;
            }
            else
            {
                
                InAirTime += Time.deltaTime;
                _jumpIntervalTime = JumpIntervalTime;

                if (_fallJudgeTime >= 0.0f)
                {
                    //准备下落
                    _fallJudgeTime -= Time.deltaTime;
                }
                else
                {
                    //开始下落
                }

                jump = false;
            }

            if (_verticalVelocity < TerminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime; //Gravity是负的
            }

        }

        #endregion

        #region Move
        private float speed = 0.0f;
        /// <summary>
        /// 输出每帧运动量
        /// </summary>
        /// <param name="sprint"></param>
        /// <param name="inputMove"></param>
        /// <param name="verticalVelocity"></param>
        /// <param name="currentRight"></param>
        /// <param name="currentFoward"></param>
        /// <param name="currentMoveVelocity"></param>
        /// <returns></returns>
        public void ApplyMotion(Transform target, CharacterController controller, ref Vector2 inputMove, ref bool sprint , Action<Vector3> OnMoveHandler = null)
        {
            float targetSpeed = sprint ? SprintSpeed : MoveSpeed;

            if (inputMove == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            //当前速度
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = inputMove.magnitude; //输入值 inputMove.magnitude 乘速度，更拟真
            

            // 加速度 或减 加速度 , 如果当前速度已经为0了，targetSpeed也为0 ，则就是为0，如果
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                //当前速度插值，输入速度
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                speed = Mathf.Round(speed * 1000f) / 1000f;  // 四舍五入到小数点后三位
            }
            else
            {
                speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(inputMove.x, 0.0f, inputMove.y).normalized;

            if (inputMove != Vector2.zero)
            {
                inputDirection = target.right * inputMove.x + target.forward * inputMove.y;
            }
            Vector3 motion = inputDirection.normalized * (speed * Time.deltaTime);
            Vector3 motionJump = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

            controller.Move(motion + motionJump);

            if(motion.sqrMagnitude > 0.01f)
            {
                OnMoveHandler?.Invoke(motion);
            }
        }

        #endregion
    }
}