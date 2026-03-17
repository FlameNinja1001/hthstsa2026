using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBoss : MonoBehaviour
{
    public bool hasWent;
    public GameObject wall1;
    public GameObject wall2;
    private BoxCollider boxCollider;

    public GameObject floor;
    public GameObject birdBossObj;
    
    void Start()
    {
        // Get the BoxCollider component attached to this GameObject
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (birdBossObj == null)
        {
            floor.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag") && !hasWent)
        {
            FollowTarget followTarget = FindObjectOfType<FollowTarget>();
            followTarget.isBoss = true;
            wall1.SetActive(true);
            wall2.SetActive(true);
            boxCollider.size = new Vector3(2.0f, 1.0f, 1.0f);
        }        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            FollowTarget followTarget = FindObjectOfType<FollowTarget>();
            followTarget.isBoss = false;
            wall1.SetActive(false);
            wall2.SetActive(false);
            hasWent = true;
        }        
    }
}
