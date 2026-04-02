using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistScript : MonoBehaviour
{
    public Transform goal1; //left
    public Transform goal2; //right
    public Transform targetTransform;
    public float speed = 5.0f;
    public float distance = 1.0f;
    public Transform model;
    public float initialXScaleMesh;   

    public bool isBird = false; 

    public float shootTimer;
    public Animator animator;
    public bool isShooting;
    private float timer;
    public bool hasStarted;
    public float shootDuration;
    public int moveDirection;
    public GameObject bulletObj;
    public float launchSpeed;
    public float vertOffset;
    private float horizDist = 30f;
    // Start is called before the first frame update
    void Start()
    {
        targetTransform = goal1;
        moveDirection = -1;
        initialXScaleMesh = model.localScale.x;        
    }

    // Update is called once per frame
    void Update()
    {          
        timer += Time.deltaTime;
        if (timer >= shootTimer)
        {
            isShooting = true;
            if (!hasStarted)
            {
                StartCoroutine(Shoot());
                hasStarted = true;
            }

        }
        else
        {
            isShooting = false;
        }
        animator.SetBool("IsShooting",isShooting);
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
                moveDirection = 1;    
            }
            else
            {
                targetTransform = goal1;  
                moveDirection = -1;                
            }
        }
        if (!isShooting)
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);
    }

    public IEnumerator Shoot()
    {
        GameObject player = GameObject.FindWithTag("PlayerTag");
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < horizDist)
        {
             GameObject bullet = Instantiate(bulletObj, new Vector3(transform.position.x,transform.position.y + vertOffset,transform.position.z), bulletObj.transform.rotation);
            bullet.transform.localScale = new Vector3(bullet.transform.localScale.x * moveDirection, bullet.transform.localScale.y, bullet.transform.localScale.z);


            Collider m_ObjectCollider = bullet.GetComponent<Collider>();        

            Collider[] all = FindObjectsOfType<Collider>();
            foreach (Collider col in all)
            {
                if (col.gameObject.CompareTag("Enemy"))
                    Physics.IgnoreCollision(m_ObjectCollider, col, true);
            } 

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(moveDirection * launchSpeed, rb.velocity.y, rb.velocity.z);             

        }       
        yield return new WaitForSeconds(shootDuration);
        isShooting = false;
        hasStarted = false;
        timer = 0f;

    }
        
}
