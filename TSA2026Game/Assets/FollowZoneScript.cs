using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowZoneScript : MonoBehaviour
{
    public FollowTarget followTarget;
    public bool isX;
    public bool isY;
    // Start is called before the first frame update
    void Start()
    {
        followTarget = FindObjectOfType<FollowTarget>();        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            if (isX)
            {
                followTarget.followX = true;
            }
            if (isY)
            {
                followTarget.followY = true;
            }
        }        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            if (isX)
            {
                followTarget.followX = true;
            }
            if (isY)
            {
                followTarget.followY = true;
            }
        }  
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            if (isX)
            {
                followTarget.followX = false;
            }
            if (isY)
            {
                followTarget.followY = false;
            }
        }  
    }
}
