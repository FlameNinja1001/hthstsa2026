using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportPlayer : MonoBehaviour
{   

    void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("PlayerTag"))
        {            
            SceneLoadManager sceneLoadManager = FindObjectOfType<SceneLoadManager>();    
            StartCoroutine(sceneLoadManager.CheckpointSpawn());
            
        }
    }
}
