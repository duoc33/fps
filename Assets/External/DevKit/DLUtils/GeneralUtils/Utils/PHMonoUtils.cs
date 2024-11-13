using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PHMonoUtils is a utility class that provides methods for managing game objects in the scene.
/// </summary>
public class PHMonoUtils : MonoBehaviour
{

    private static PHMonoUtils instance;
    /// <summary>
    /// Singleton instance of PHMonoUtils. If an instance doesn't exist, it creates one.
    /// </summary>
    public static PHMonoUtils Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(PHMonoUtils)) as PHMonoUtils;
            }
            if (instance == null)
            {
                GameObject obj = new GameObject("AUTOGEN_" + typeof(PHMonoUtils).Name);
                instance = obj.AddComponent<PHMonoUtils>();
            }
            return instance;
        }

    }

    /// <summary>
    /// Finds an initial transform for a game object. The transform is chosen randomly from a list of spawn points.
    /// </summary>
    /// <param name="radius">The radius within which the game object can be spawned.</param>
    /// <param name="spName">The name of the spawn point game object.</param>
    /// <returns>The initial transform for the game object.</returns>
    public Transform FindInitialTransform(float radius, string spName = "SpawnPoint")
    {
        Transform initTransform = transform;

        List<Transform> spawnPoints = new List<Transform>();
        foreach (var item in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (item.name== spName)
            {
                spawnPoints.Add(item.transform);
            }
        }
        Debug.Log(spawnPoints.Count);
        var spawnPoint = PHUtils.GetRandom(spawnPoints);

        Debug.Log("The spawnpoint is " + spawnPoint);
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        if (spawnPoint != null)
        {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        }

        var randomOffset = new Vector3(UnityEngine.Random.Range(-radius, radius), 0, UnityEngine.Random.Range(-radius, radius));


        pos += randomOffset;

        // get terrain height
        if (Terrain.activeTerrain != null)
        {
            var terrainHeight = Terrain.activeTerrain.SampleHeight(pos);
            if (pos.y < terrainHeight)
            {
                // pos = new Vector3(pos.x, terrainHeight + 2f, pos.z);
            }
        }


        initTransform.position = pos;
        initTransform.rotation = rot;




        return initTransform;
    }


    /// <summary>
    /// Called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        instance = null;
    }

}

