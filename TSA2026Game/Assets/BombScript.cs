using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public Transform goal1; //left
    public Transform goal2; //right
    public Transform targetTransform;
    public float speed = 5.0f;
    public float distance = 1.0f;
    public Transform model;

    public GameObject mesh1;
    public GameObject mesh2;
    public float initialXScaleMesh;   
    public float initialYScaleMesh;  
    public float initialZScaleMesh;  
    

    public GameObject player;
    public float boomThreshold;
    public bool isExploding;
    private float explosionTimer;
    public float flashInterval;
    public bool correctMeshActive;
    public float count = 0;
    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerTag");
        targetTransform = goal1;
        initialXScaleMesh = model.localScale.x;        
        initialYScaleMesh = model.localScale.y;        
        initialZScaleMesh = model.localScale.z;        
    }

    // Update is called once per frame
    void Update()
    {  
        if (count >= 3)
        {
            Instantiate(explosionPrefab, new Vector3(transform.position.x,transform.position.y + 1.5f, transform.position.z), explosionPrefab.transform.rotation);
            Destroy(gameObject);
        }
        if (Vector3.Distance(player.transform.position, transform.position) < boomThreshold)
        {
            isExploding = true;
        }
        else
        {
            isExploding = false;
            explosionTimer = 0f;
            correctMeshActive = true;
            count = 0;
        }
       
        if (targetTransform == goal1)
        {            
            model.localRotation = Quaternion.Euler(0,-90,0);
            model.localScale = new Vector3(-initialXScaleMesh, initialYScaleMesh, initialZScaleMesh);    
        }
        else
        {
            model.localRotation = Quaternion.Euler(0,90,0);
            model.localScale = new Vector3(initialXScaleMesh, initialYScaleMesh,  initialZScaleMesh);    
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

        if (isExploding)
        {
            explosionTimer += Time.deltaTime;
            if (explosionTimer >= flashInterval)
            {
                correctMeshActive = !correctMeshActive;
                explosionTimer = 0f;
                count++;
            }
        }

        mesh2.transform.localRotation = model.localRotation;
        mesh2.transform.localScale = model.localScale;

        if (correctMeshActive)
        {
            mesh1.transform.localScale = model.localScale;
            mesh2.transform.localScale = model.localScale * 0.01f;
        }
        else
        {
            mesh2.transform.localScale = model.localScale;
            mesh1.transform.localScale = model.localScale * 0.01f;
        }
    }
        
}
