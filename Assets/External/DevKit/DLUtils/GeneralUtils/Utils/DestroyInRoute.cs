using System;
using UnityEngine;

public class DestroyInRoute:MonoBehaviour
{
    private void Awake()
    {
        // Debug.Log("aaaaaThis is in + " + gameObject.name);
    }

    private void OnDisable()
    {
        // Debug.Log("aaaaaThis disable is in + " + gameObject.name);
    }
}