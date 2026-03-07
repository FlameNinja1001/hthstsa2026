using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightScript : MonoBehaviour
{
    public Transform goal1; //left
    public Transform goal2; //right
    public Transform targetTransform;
    public float speed = 5.0f;
    public float distance = 1.0f;    
    public float initialXScale;

    public Transform playerPos;
    public GameObject damageHitbox;
    public float distanceThresholdPlayer;

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = goal1; 
        initialXScale = transform.localScale.x;            
    }

    // Update is called once per frame
    void Update()
    {      
        if (Vector3.Distance(playerPos.position,transform.position) < distanceThresholdPlayer)
        {
            damageHitbox.SetActive(true);
        }
        else
        {
            damageHitbox.SetActive(false);
        }
        if (targetTransform == goal1)
        {                    
            transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);    
        }
        else
        {
            transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);     
        }  
        if (Vector3.Distance(transform.position,targetTransform.position) < distance)
        {
            if (targetTransform == goal1)
            {
                targetTransform = goal2;    
            }
            else
            {
                targetTransform = goal1;                  
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);
    }
        
}
