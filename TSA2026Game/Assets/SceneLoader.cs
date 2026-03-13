using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public float timer = 0.1f;
    void Awake()
    {
        Destroy(gameObject,timer);
    }
}
