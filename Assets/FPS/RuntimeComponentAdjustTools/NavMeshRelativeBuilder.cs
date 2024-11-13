using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
namespace RuntimeComponentAdjustTools
{
    /// <summary>
    /// 通过将所有要烘焙的物体，放在一个父物体下，即可自动Runtime烘焙NavMesh。
    /// </summary>
    public class NavMeshRelativeBuilder
    {
        public float AgentHeight;
        public float AgentClimb;
        public float AgentRadius;
        public float AgentSlope;
        NavMeshData m_NavMesh;
        NavMeshDataInstance m_Instance;
        Bounds m_bounds;
        List<NavMeshBuildSource> m_sources;

        public Bounds GetBounds() => m_bounds;

        /// <summary>
        /// terrain.gameobject, terrain  包含地形时，m_bounds就不是真正意义上地图的bounds
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="terrain"></param>
        /// <param name="isUseMesh"></param>
        private void InitNavMesh(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            DestroyNavMesh();
            m_sources ??= new List<NavMeshBuildSource>();
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
            m_bounds = BoundsTool.CalculateBounds(gameObject);
            CollectSources(ref m_sources, gameObject, isUseMesh);
            if (terrain != null)
            {
                var source = new NavMeshBuildSource();
                source.shape = NavMeshBuildSourceShape.Terrain;
                source.sourceObject = terrain.terrainData;
                source.transform = terrain.transform.localToWorldMatrix;
                source.area = 0;
                m_sources.Add(source);
            }
        }

        public void BuildNavMesh(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            InitNavMesh(gameObject, terrain, isUseMesh);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentHeight = AgentHeight;
            defaultBuildSettings.agentClimb = AgentClimb;
            defaultBuildSettings.agentRadius = AgentRadius;
            defaultBuildSettings.agentSlope = AgentSlope;
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_sources, m_bounds);
        }

        public async UniTask BuildNavMeshAsync(GameObject gameObject, Terrain terrain = null, bool isUseMesh = true)
        {
            InitNavMesh(gameObject, terrain, isUseMesh);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            defaultBuildSettings.agentHeight = AgentHeight;
            defaultBuildSettings.agentClimb = AgentClimb;
            defaultBuildSettings.agentRadius = AgentRadius;
            defaultBuildSettings.agentSlope = AgentSlope;
            await NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_sources, m_bounds).ToUniTask();
        }

        public void DestroyNavMesh()
        {
            m_Instance.Remove();
            m_NavMesh = null;
            m_sources?.Clear();
            m_bounds = default;
        }


        /// <summary>
        /// gameObject 包含所有需要烘焙物体的父物体
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="gameObject"></param>
        /// <param name="isUseMesh"></param>
        static void CollectSources(ref List<NavMeshBuildSource> sources, GameObject gameObject, bool isUseMesh = true)
        {
            sources.Clear();

            if (isUseMesh)
            {
                foreach (var meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    if (meshFilter == null || meshFilter.sharedMesh == null) continue;
                    NavMeshBuildSource source = CreateMeshSource(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
                    sources.Add(source);
                }
            }
            else
            {
                var colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    if (collider is MeshCollider)
                    {
                        var meshCollider = collider as MeshCollider;
                        NavMeshBuildSource source = CreateMeshSource(meshCollider.sharedMesh, meshCollider.transform.localToWorldMatrix);
                        sources.Add(source);
                    }
                    else
                    {
                        var source = new NavMeshBuildSource();
                        source.component = collider;
                        source.transform = collider.transform.localToWorldMatrix;
                        if (collider is BoxCollider) source.shape = NavMeshBuildSourceShape.Box;
                        else if (collider is SphereCollider) source.shape = NavMeshBuildSourceShape.Sphere;
                        else if (collider is CapsuleCollider) source.shape = NavMeshBuildSourceShape.Capsule;
                        sources.Add(source);
                    }
                }
            }
        }
        static NavMeshBuildSource CreateMeshSource(Mesh mesh, Matrix4x4 transform)
        {
            var source = new NavMeshBuildSource();
            source.shape = NavMeshBuildSourceShape.Mesh;
            source.sourceObject = mesh;
            source.transform = transform;
            source.area = 0;
            return source;
        }
    }
}
