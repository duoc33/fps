
using UnityEngine;
using UnityEngine.AI;
namespace RuntimeComponentAdjustTools
{
    public class NavMeshTool
    {
        public static Vector3 GetRandomPointWithinNavMesh(Vector3 src ,float SphereRadiusRange)
        {
            // 随机生成一个方向，距离为10单位
            Vector3 randomDirection = Random.insideUnitSphere * SphereRadiusRange;
            randomDirection += src;

            NavMeshHit hit;
            // 使用NavMesh.SamplePosition确保随机点在导航网格内
            if (NavMesh.SamplePosition(randomDirection, out hit, SphereRadiusRange + 2.0f , NavMesh.AllAreas))
            {
                return hit.position;
            }

            return Vector3.zero; // 返回零向量表示未找到有效位置
        }
        public static bool FindClosestEdgePos(Vector3 src, out Vector3 edge)
        {
            if (NavMesh.FindClosestEdge(src, out var hit, NavMesh.AllAreas))
            {
                edge = hit.position;
                return true;
            }
            edge = Vector3.zero;
            return false;
        }
        public static Vector3[] GetNavMeshPath(Vector3 src, Vector3 dest)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(src, dest, NavMesh.AllAreas, path))
            {
                return path.corners;
            }
            return null;
        }
        public static bool GetRandomPositionInBounds(Bounds bounds, out Vector3 pos)
        {
            Vector3 randomPoint = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
            int maxIterations = 10;
            return GetNearPosition(bounds, randomPoint, 1, out pos, ref maxIterations);
        }
        public static bool GetConcreteNearPosition(Bounds bounds, Vector3 cpos, out Vector3 pos)
        {
            float x = Mathf.Clamp(cpos.x, bounds.min.x, bounds.max.x);
            float z = Mathf.Clamp(cpos.z, bounds.min.z, bounds.max.z);
            float y = Mathf.Clamp(cpos.y, bounds.min.y, bounds.max.y);
            cpos = new Vector3(x, y, z);
            int maxIterations = 10;
            return GetNearPosition(bounds, cpos, 1, out pos, ref maxIterations);
        }


        static bool GetNearPosition(Bounds bounds, Vector3 cpos, float dy, out Vector3 pos, ref int maxIterations)
        {
            maxIterations--;
            if (maxIterations < 0)
            {
                pos = Vector3.zero;
                return false;
            }
            NavMeshHit hit;
            if (NavMesh.SamplePosition(cpos, out hit, bounds.size.y + dy, NavMesh.AllAreas))
            {
                pos = hit.position;
                return true;
            }
            else
            {
                return GetNearPosition(bounds, cpos, dy + 1.0f, out pos, ref maxIterations);
            }
        }
    }
}


