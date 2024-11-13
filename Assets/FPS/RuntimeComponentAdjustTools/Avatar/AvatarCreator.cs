using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RuntimeComponentAdjustTools
{
    public class AvatarCreator
    {
        /// <summary>
        /// 通过调整骨骼的位姿（bind pose）来修正骨骼的位置和旋转，通常用于在 Unity 中处理 SkinnedMeshRenderer 的绑定骨骼数据。这种方法常用于角色模型的骨骼修复或调整，确保骨骼的姿势和旋转正确无误。
        /// 用于处理 SkinnedMeshRenderer 组件的绑定姿势（bind pose）。绑定姿势是指在模型的骨骼动画系统中，网格在未进行动画变换时的默认姿势。
        /// 绑定姿势（Bind Pose）：描述骨骼在模型初始状态下的位置和方向的矩阵。
        /// 最终位置计算：通过将局部顶点位置乘以绑定姿势矩阵，再加上当前骨骼的变换，得到顶点在世界空间中的位置。
        /// 绑定姿势（Bind Pose）：bindposes 数组中的每个矩阵描述了一个骨骼在模型的初始状态下的变换。这些矩阵实际上定义了骨骼的局部空间如何与模型的全局空间对齐。
        /// 提取和应用每个骨骼的绑定姿势，同时保留其原始的层次结构。通过这种方式，可以在需要时重构骨骼的初始位置和方向，比如在进行一些特定的动画预处理或调试时。
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SampleBindPose(GameObject gameObject)
        {
            SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
            {
                Matrix4x4 localToWorldMatrix = skinnedMeshRenderer.transform.localToWorldMatrix;
                Dictionary<Transform, Transform> dictionary = new Dictionary<Transform, Transform>(skinnedMeshRenderer.bones.Length);
                //解除父级关系，但记录下来：以便于对它们的位置和旋转进行直接操作。
                //对每个骨骼进行遍历，保存其原始父级到字典中，并将骨骼的父级设置为 null，
                for (int j = 0; j < skinnedMeshRenderer.bones.Length; j++)
                {
                    Transform transform = skinnedMeshRenderer.bones[j];
                    dictionary[transform] = transform.parent;
                    transform.SetParent(null, worldPositionStays: true);
                }

                for (int k = 0; k < skinnedMeshRenderer.bones.Length; k++)
                {
                    Transform transform2 = skinnedMeshRenderer.bones[k];
                    // 使用 localToWorldMatrix 和 bindposes 的逆矩阵计算每个骨骼在世界空间中的位置和旋转。
                    // 这表示从绑定姿势转换到当前世界空间的位置。
                    Matrix4x4 matrix4x = localToWorldMatrix * skinnedMeshRenderer.sharedMesh.bindposes[k].inverse;
                    Vector3 lhs = new Vector3(matrix4x.m00, matrix4x.m10, matrix4x.m20);
                    Vector3 vector = new Vector3(matrix4x.m01, matrix4x.m11, matrix4x.m21);
                    Vector3 vector2 = new Vector3(matrix4x.m02, matrix4x.m12, matrix4x.m22);
                    Vector3 vector3 = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
                    transform2.position = vector3 * (Mathf.Abs(transform2.lossyScale.z) / vector2.magnitude); // 考虑模型 z 缩放
                                                                                                              // 考虑旋转
                    transform2.rotation = (Vector3.Dot(Vector3.Cross(lhs, vector), vector2) >= 0f) ? Quaternion.LookRotation(vector2, vector) : Quaternion.LookRotation(-vector2, -vector);
                }

                for (int l = 0; l < skinnedMeshRenderer.bones.Length; l++)
                {
                    Transform transform3 = skinnedMeshRenderer.bones[l];
                    transform3.SetParent(dictionary[transform3], worldPositionStays: true);
                }
            }
        }
    }
}

