using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehavior : MonoBehaviour
{
    public Animator animator;
    public Transform hidePos;
    public Transform playerTransform;
    public float distanceThreshold;
    public Transform target;
    public bool canSet = true;
    public float setTargetDelay = 0.5f;
    public float returnSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = hidePos.position;
        target = hidePos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, hidePos.position) < 0.1)
        {
            canSet = true;
            animator.SetBool("IsChasing",false);
        }
        else
        {
            animator.SetBool("IsChasing",true);
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime); 
        if (target == hidePos)
        {
            currentSpeed = returnSpeed;
            if ((Vector3.Distance(transform.position, playerTransform.position) < distanceThreshold) && canSet)
            {
                StartCoroutine(SetTarget(playerTransform));    
            }
        }
        else if (target == playerTransform)
        {
            currentSpeed = chaseSpeed;
            if (Vector3.Distance(transform.position, playerTransform.position) > distanceThreshold * 3)
            {
                StartCoroutine(SetTarget(hidePos));    
            }            
        }
    }

    public IEnumerator SetTarget(Transform t)
    {
        target = t;
        canSet = false;
        yield return new WaitForSeconds(setTargetDelay);        
    }    

    
}
