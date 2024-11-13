using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers
{
    
    public static class GameObjectExtensions
    {
        public static void AddAdjustCollider(this GameObject gameObject)
        {
            // gameObject.GetComponent<Mesh>()
            
        }
        
        /// <summary>
        /// 基于多个Renderer的联合Bounds创建适应的Collider。
        /// </summary>
        /// <param name="gameObject">要生成Collider的GameObject</param>
        public static void AddColliderBasedOnMultipleRenderers(this GameObject gameObject)
        {
            // 获取当前对象及其子对象上的所有Renderer
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"GameObject {gameObject.name} 没有找到任何 Renderer 组件，无法生成 Collider。");
                return;
            }

            // 计算所有Renderer的联合Bounds
            Bounds combinedBounds = renderers[0].bounds;
            foreach (var renderer in renderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            // 移除现有Collider，防止重复
            RemoveExistingColliders(gameObject);

            // 自动选择Collider类型
            var extents = combinedBounds.extents;

            if (Mathf.Approximately(extents.x, extents.y) && Mathf.Approximately(extents.y, extents.z))
            {
                // 如果联合Bounds接近球体，则添加SphereCollider
                SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.center = gameObject.transform.InverseTransformPoint(combinedBounds.center);
                sphereCollider.radius = extents.magnitude;
            }
            else if (extents.y > extents.x && extents.y > extents.z)
            {
                // 如果高度大于宽度和深度，添加CapsuleCollider
                CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.center = gameObject.transform.InverseTransformPoint(combinedBounds.center);
                capsuleCollider.height = combinedBounds.size.y;
                capsuleCollider.radius = Mathf.Min(extents.x, extents.z);
                capsuleCollider.direction = 1; // Y轴方向
            }
            else
            {
                // 默认添加BoxCollider
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.center = gameObject.transform.InverseTransformPoint(combinedBounds.center);
                boxCollider.size = combinedBounds.size;
            }
        }
        
        /// <summary>
        /// 为GameObject添加Collider，其大小基于Renderer的Bounds。
        /// </summary>
        /// <param name="gameObject">需要添加Collider的对象</param>
        public static void AddColliderBasedOnRenderer(this GameObject gameObject)
        {
            // 检查是否有Renderer组件
            var renderer = gameObject.FindComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"GameObject {gameObject.name} 没有 Renderer 组件，无法生成 Collider。");
                return;
            }

            // 获取Renderer的Bounds
            Bounds bounds = renderer.bounds;

            // 移除已有的Collider，避免重复
            RemoveExistingColliders(gameObject);

            // 自动选择合适的Collider类型
            var extents = bounds.extents;

            if (Mathf.Approximately(extents.x, extents.y) && Mathf.Approximately(extents.y, extents.z))
            {
                // 如果Bounds接近球形，添加SphereCollider
                SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.center = gameObject.transform.InverseTransformPoint(bounds.center);
                sphereCollider.radius = extents.magnitude;
            }
            else if (extents.y > extents.x && extents.y > extents.z)
            {
                // 如果高度大于宽度和深度，添加CapsuleCollider
                CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.center = gameObject.transform.InverseTransformPoint(bounds.center);
                capsuleCollider.height = bounds.size.y;
                capsuleCollider.radius = Mathf.Min(extents.x, extents.z);
                capsuleCollider.direction = 1; // Y轴方向
            }
            else
            {
                // 默认添加BoxCollider
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.center = gameObject.transform.InverseTransformPoint(bounds.center);
                boxCollider.size = bounds.size;
            }
        }

        /// <summary>
        /// 删除对象上已存在的Collider，避免重复添加。
        /// </summary>
        /// <param name="obj">要删除Collider的对象</param>
        private static void RemoveExistingColliders(GameObject obj)
        {
            foreach (var collider in obj.GetComponents<Collider>())
            {
                Object.Destroy(collider);
            }
        }
        
        public static T TryAddComponent<T>(this GameObject gameObject) where T: Component
        {
            var type = typeof(T);
            var comp = gameObject.GetComponent(type);
            if (comp == null) comp = gameObject.AddComponent(type);
            return comp as T;
        }
        
        
        public static T FindComponent<T>(this GameObject c) where T : Component
        {
            T Ttt = c.GetComponent<T>();
            if (Ttt != null) return Ttt;

            Ttt = c.GetComponentInChildren<T>(true);
            if (Ttt != null) return Ttt;

            Ttt = c.GetComponentInParent<T>();
            if (Ttt != null) return Ttt;
            
            return default;
        }



        public static Component FindComponent(this GameObject c, Type t)
        {
            var Ttt = c.GetComponent(t);
            if (Ttt != null) return Ttt;

            Ttt = c.GetComponentInChildren(t, true);
            if (Ttt != null) return Ttt;

            Ttt = c.GetComponentInParent(t);
            if (Ttt != null) return Ttt;
            
            return default;
        }

    }
}