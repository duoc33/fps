using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeshIntergrate : MonoBehaviour
{
    public void Intergrate(MeshFilter[] meshFilters,string newGameObjectName,string newMeshName)
    {
        if(meshFilters == null || meshFilters.Length == 0) return;
        CombineInstance[] combines = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Transform original = meshFilters[i].transform;
            // Matrix4x4.TRS(,);
        }
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combines);
        newMesh.name = newMeshName;
        GameObject obj = new GameObject(newGameObjectName);
        obj.AddComponent<MeshFilter>().sharedMesh = newMesh;
        obj.AddComponent<MeshRenderer>().sharedMaterial = meshFilters[0].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
