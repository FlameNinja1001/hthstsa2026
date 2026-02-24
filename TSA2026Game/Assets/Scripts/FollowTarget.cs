using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public bool followX;
    public bool followY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3((followX ?  target.position.x : transform.position.x),(followY ?  target.position.y : transform.position.y),transform.position.z);
    }        
}
