using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RuntimeComponentAdjustTools
{
    public class BoundsTool
    {
        public static void SetMeshColliderConvex(GameObject target)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (meshFilter.GetComponent<Collider>() == null)
                {
                    MeshCollider meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = false;
                }
            }
        }
        public static GameObject GetBestBoxColliderAndBottomPivot(GameObject target, Bounds bounds = default)
        {
            if (bounds == default)
            {
                bounds = CalculateBounds(target);
            }
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Vector3 bottomPos = center - Vector3.up * size.y / 2;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = bottomPos;

            BoxCollider boxCollider = pivot.AddComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.center = Vector3.up * size.y / 2;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }
        public static GameObject GetBestBottomPivot(GameObject target, Bounds bounds = default)
        {
            if (bounds == default)
            {
                bounds = CalculateBounds(target);
            }
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            Vector3 bottomPos = center - Vector3.up * size.y / 2;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = bottomPos;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }
        public static GameObject GetBestBoxColliderAndCenterPivot(GameObject target)
        {
            Bounds bounds = CalculateBounds(target);
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;
            GameObject pivot = new GameObject(target.name);
            pivot.transform.position = center;

            BoxCollider boxCollider = pivot.AddComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.center = Vector3.zero;
            target.transform.SetParent(pivot.transform);
            return pivot;
        }

        /// <summary>
        /// 等一帧再执行其他相关Bounds操作 ， bounds会在下一帧更新。避免Scale的影响。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static BoxCollider SetBestCollider(GameObject target)
        {
            Bounds bounds = CalculateBounds(target);
            Vector3 size = bounds.size;
            Vector3 center = bounds.center;
            BoxCollider collider = target.AddComponent<BoxCollider>();
            collider.size = size;
            collider.center = center - target.transform.position;
            return collider;
        }
        public static GameObject GetBestCapsuleColliderAndGetBottomPivot(Transform current , Bounds bounds = default, bool isTpos = true)
        {
            if(bounds.Equals(default))
            {
                bounds = CalculateBounds(current.gameObject);
            }
            Vector3 center = bounds.center;
            Vector3 size = bounds.size;

            float height = size.y ;
            float radius = isTpos ? bounds.size.x / 4.0f : bounds.size.x / 2.0f;

            GameObject pivot = GetBestBottomPivot(current.gameObject,bounds);

            var capsuleCollider = pivot.AddComponent<CapsuleCollider>();
            capsuleCollider.center = center - pivot.transform.position;
            capsuleCollider.height = height;
            capsuleCollider.radius = radius;

            return pivot;
        }
        //如果模型的Scale有问题，不为Vector3.one，那么就会出问题, 
        public static CharacterController InitBestBoundsForCharacterController(Transform current, bool isTpos = true, float heightOffsetScale = 0.05f, float centerOffsetScale = 0.1f)
        {
            Bounds modelBounds = CalculateBounds(current.gameObject);
            float height = modelBounds.size.y * (1 - heightOffsetScale);
            float radius = isTpos ? modelBounds.size.x / 4.0f : modelBounds.size.x / 2.0f;

            Vector3 centerOffset = modelBounds.center - current.position;
            centerOffset.y = centerOffset.y * centerOffsetScale + centerOffset.y;

            current.TryGetComponent(out CapsuleCollider capsuleCollider);
            capsuleCollider ??= current.gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = centerOffset;
            capsuleCollider.height = height;
            capsuleCollider.radius = radius;


            current.gameObject.TryGetComponent(out CharacterController characterController);
            characterController ??= current.gameObject.AddComponent<CharacterController>();

            characterController.center = centerOffset; // 设置 CharacterController 的中心
            characterController.height = height; // 设置 CharacterController 的半径和高度
            characterController.radius = radius; // 设置 CharacterController 的半径和高度 (T pos)

            characterController.slopeLimit = 45; // 默认 45
            characterController.stepOffset = height * 0.2f; // 步伐高度,趴楼梯用到，默认升高的1/5;
            characterController.skinWidth = radius * 0.15f; // 两个碰撞体碰撞时，相互穿透的距离皮肤宽度，默认radius * 0.1f;

            // current.position = oldPos;

            return characterController;
        }
        public static GameObject InitCharacterControllerAndReturnBottomPivot(Transform current, bool isTpos = true, float heightOffsetScale = 0.05f, float centerOffsetScale = 0.1f)
        {
            Bounds modelBounds = CalculateBounds(current.gameObject);
            GameObject pivot = GetBestBottomPivot(current.gameObject, modelBounds);

            float height = modelBounds.size.y * (1 - heightOffsetScale);
            float radius = isTpos ? modelBounds.size.x / 4.0f : modelBounds.size.x / 2.0f;

            Vector3 centerOffset = modelBounds.center - pivot.transform.position;
            centerOffset.y = centerOffset.y * centerOffsetScale + centerOffset.y;


            pivot.TryGetComponent(out CapsuleCollider capsuleCollider);
            capsuleCollider ??= pivot.AddComponent<CapsuleCollider>();
            capsuleCollider.center = centerOffset;
            capsuleCollider.height = height;
            capsuleCollider.radius = radius;


            pivot.TryGetComponent(out CharacterController characterController);
            characterController ??= pivot.AddComponent<CharacterController>();

            characterController.center = centerOffset; // 设置 CharacterController 的中心
            characterController.height = height; // 设置 CharacterController 的半径和高度
            characterController.radius = radius; // 设置 CharacterController 的半径和高度 (T pos)

            characterController.slopeLimit = 45; // 默认 45
            characterController.stepOffset = height * 0.2f; // 步伐高度,趴楼梯用到，默认升高的1/5;
            characterController.skinWidth = radius * 0.15f; // 两个碰撞体碰撞时，相互穿透的距离皮肤宽度，默认radius * 0.1f;


            return pivot;
        }

        public static void FitToBounds(Camera camera, GameObject gameObject, float distance)
        {
            Bounds bounds = CalculateBounds(gameObject);
            FitToBounds(camera, bounds, distance);
        }
        /// <summary>
        /// 根据相机的属性和物体的大小，计算出相机与物体之间的合适距离，以便于将整个物体都显示在摄像机的视野内。
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="bounds"></param>
        /// <param name="distance"></param>
        public static void FitToBounds(Camera camera, Bounds bounds, float distance)
        {
            float magnitude = bounds.extents.magnitude;
            // 将 magnitude 除以视场的正切值（来计算在某个距离下相机需要移动的量），然后再乘以一个距离因子 distance，最终得到 num。
            float num = magnitude / (2f * Mathf.Tan(0.5f * camera.fieldOfView * (Mathf.PI / 180f))) * distance;
            if (!float.IsNaN(num))
            {
                camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z + num);
                camera.transform.LookAt(bounds.center);
            }
        }
        public static void FitToBounds(Camera camera, Bounds bounds, Quaternion rotation, float distance)
        {
            float magnitude = bounds.extents.magnitude;//从中心出发的距离边界最大值 ， 把想象为摄像机的视野一半的垂直长度
                                                       // camera.fieldOfView 是Vertical角度，它的Tan值，被extents.magnitude除，得到摄像机的视场距离模型边界一个距离的度量。
            float num = magnitude / (2f * Mathf.Tan(0.5f * camera.fieldOfView * (Mathf.PI / 180f))) * distance;
            if (!float.IsNaN(num))
            {
                camera.transform.position = bounds.center - rotation * Vector3.forward * num;
                camera.transform.LookAt(bounds.center);
            }
        }

        public static void SetModelInUnit(Transform target, Bounds bounds, float scalePerUnit = 1)
        {
            if (scalePerUnit == 0 || bounds.Equals(default) || target.gameObject.isStatic) return;
            Vector3 currentSize = bounds.size; // 当前尺寸
                                               // 计算缩放因子
            float maxCurrentSize = Mathf.Max(currentSize.x, currentSize.y, currentSize.z);
            float scaleFactor = scalePerUnit * 1.0f / maxCurrentSize;
            if (scaleFactor == Mathf.Infinity)
            {
                scaleFactor = 1;
            }
            // 应用缩放
            target.localScale = Vector3.one * scaleFactor;
        }


        public static Bounds CalculateBounds(GameObject gameObject, bool localSpace = false)
        {
            Vector3 position = gameObject.transform.position;
            Quaternion rotation = gameObject.transform.rotation;
            Vector3 localScale = gameObject.transform.localScale;

            if (localSpace)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }

            Bounds result = default;
            Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
            if (componentsInChildren.Length != 0)
            {
                Bounds bounds = componentsInChildren[0].bounds;
                result.center = bounds.center;
                result.extents = bounds.extents;
                for (int i = 1; i < componentsInChildren.Length; i++)
                {
                    Renderer renderer = componentsInChildren[i];
                    Bounds bounds2 = renderer.bounds;
                    result.Encapsulate(bounds2);
                }
                // 如果边界的大小非常小，说明可能没有有效的物体在该边界内，或该边界的计算不够精确。
                if (result.size.magnitude < 0.001f)
                {
                    result = CalculatePreciseBounds(gameObject);
                }
            }
            else
            {
                result = CalculatePreciseBounds(gameObject);
            }

            if (localSpace)
            {
                gameObject.transform.position = position;
                gameObject.transform.rotation = rotation;
                gameObject.transform.localScale = localScale;
            }

            return result;
        }


        /// <summary>
        /// 重新计算模型Bounds，
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds CalculatePreciseBounds(GameObject gameObject)
        {
            Bounds result = default;
            bool flag = false;
            MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
            if (componentsInChildren.Length != 0)
            {
                result = GetMeshBounds(componentsInChildren[0].gameObject, componentsInChildren[0].sharedMesh);
                flag = true;
                for (int i = 1; i < componentsInChildren.Length; i++)
                {
                    result.Encapsulate(GetMeshBounds(componentsInChildren[i].gameObject, componentsInChildren[i].sharedMesh));
                }
            }

            SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (componentsInChildren2.Length != 0)
            {
                Mesh mesh = new Mesh();
                //如果MeshFilter为0个，这里就需要找出SkinnedMeshRenderer第一个，进行初始化result
                if (!flag)
                {
                    componentsInChildren2[0].BakeMesh(mesh);
                    result = GetMeshBounds(componentsInChildren2[0].gameObject, mesh);
                }

                for (int j = 1; j < componentsInChildren2.Length; j++)
                {
                    //生成一个新的临时网格 ,计算出来的 SkinnedMeshRenderer的 Mesh 不会受到动画的影响（TPOS）。
                    componentsInChildren2[j].BakeMesh(mesh);
                    result = GetMeshBounds(componentsInChildren2[j].gameObject, mesh);
                }

                Object.Destroy(mesh);
            }

            return result;
        }

        /// <summary>
        /// mesh 就是 gameObject上的组件，或重新烘焙出来的 
        /// 该方法根据Mesh顶点，当前物体对象，计算了该mesh的世界空间bounds
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private static Bounds GetMeshBounds(GameObject gameObject, Mesh mesh)
        {
            Bounds result = default;
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length != 0)
            {
                result = new Bounds(gameObject.transform.TransformPoint(vertices[0]), Vector3.zero);
                for (int i = 1; i < vertices.Length; i++)
                {
                    result.Encapsulate(gameObject.transform.TransformPoint(vertices[i]));
                }
            }
            return result;
        }

        #region 旧版代码

        // public static Bounds GetRealBounds(Transform current)
        // {
        //     Bounds bounds = default;
        //     GetRealBoundsRecursively(current, ref bounds);
        //     return bounds;
        // }
        // public static Bounds GetRealLocalBounds(Transform current)
        // {
        //     Bounds bounds = default;
        //     int count = 0;
        //     GetRealLocalBoundsRecursively(current, ref bounds, ref count);
        //     return bounds;
        // }
        // public static bool GetBounds(Transform target, out Bounds bounds)
        // {
        //     Renderer renderer = target.GetComponent<Renderer>();
        //     if (renderer != null)
        //     {
        //         bounds = renderer.bounds;
        //         return true;
        //     }
        //     Collider collider = target.GetComponent<Collider>();
        //     if (collider != null)
        //     {
        //         bounds = collider.bounds;
        //         return true;
        //     }
        //     bounds = default;
        //     return false;
        // }
        // public static bool GetLocalBounds(Transform current, out Bounds bounds)
        // {
        //     Renderer renderer = current.GetComponent<Renderer>();
        //     if (renderer != null)
        //     {
        //         bounds = renderer.localBounds;
        //         return true;
        //     }
        //     bounds = default;
        //     return false;
        // }

        // private static void GetRealBoundsRecursively(Transform current, ref Bounds resbounds)
        // {
        //     // 检查当前物体的 Bounds
        //     if (GetBounds(current, out Bounds bounds))
        //     {
        //         resbounds.Encapsulate(bounds);
        //     }

        //     // 递归遍历子物体
        //     foreach (Transform child in current)
        //     {
        //         GetRealBoundsRecursively(child, ref resbounds);
        //     }
        // }

        // private static void GetRealLocalBoundsRecursively(Transform current, ref Bounds resbounds, ref int count)
        // {
        //     // 检查当前物体的 Bounds
        //     if (GetLocalBounds(current, out Bounds bounds))
        //     {
        //         count++;
        //         resbounds.Encapsulate(bounds);
        //     }

        //     // 递归遍历子物体
        //     foreach (Transform child in current)
        //     {
        //         GetRealLocalBoundsRecursively(child, ref resbounds, ref count);
        //     }
        // }


        #endregion

    }

}
