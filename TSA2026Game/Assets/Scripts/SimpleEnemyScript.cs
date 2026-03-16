using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyScript : MonoBehaviour
{
    public Transform goal1; //left
    public Transform goal2; //right
    public Transform targetTransform;
    public float speed = 5.0f;
    public float distance = 1.0f;
    public Transform model;
    public float initialXScaleMesh;   

    public bool isBird = false; 
    // Start is called before the first frame update
    void Start()
    {
        targetTransform = goal1;
        initialXScaleMesh = model.localScale.x;        
    }

    // Update is called once per frame
    void Update()
    {  
        if (!isBird)
        {
            if (targetTransform == goal1)
            {            
                model.localRotation = Quaternion.Euler(0,-90,0);
                model.localScale = new Vector3(-initialXScaleMesh, model.localScale.y, model.localScale.z);    
            }
            else
            {
                model.localRotation = Quaternion.Euler(0,90,0);
                model.localScale = new Vector3(initialXScaleMesh, model.localScale.y,  model.localScale.z);    
            }  
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
