using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    // 控制相机抖动
    public class CameraDithering : MonoBehaviour
    {
        private Vector3 currentRotation; // 当前旋转角度
        private Vector3 targetRotation; // 目标旋转角度
        private float snappiness; // 旋转平滑度
        private float returnAmount; // 回归平滑度
        void Update()
        {
            // 让目标旋转角度逐渐回归到零向量
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);

            // 使用插值方法让当前旋转角度逐渐接近目标旋转角度
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);

            // 将当前旋转角度应用到相机的局部旋转
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
        /// <summary>
        /// 抖动相机
        /// </summary>
        /// <param name="i"></param>
        /// <param name="Snappiness">每帧旋转的跨度 </param>
        /// <param name="ReturnAmount">每帧回归0旋转的跨度 </param>
        public void Shake(int i, float Snappiness = 5f, float ReturnAmount = 8f)
        {
            snappiness = Snappiness;
            returnAmount = ReturnAmount;
            // 设置目标旋转角度为一个在 (-i, i) 范围内的随机向量
            targetRotation = new Vector3(Random.Range(-i, i), Random.Range(-i, i), Random.Range(-i, i));
        }
    }
}
