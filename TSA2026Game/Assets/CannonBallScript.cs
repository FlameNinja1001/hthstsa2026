using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{   
    public float xSpeed; 
    public float ySpeed; 

    public float a;
    public float b;
    public float c;

    public float xTimer;
    public float yTimer;

    public float yOffset;
    public float xOffset;

    public Vector3 initialPos;
    public bool isRight;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        xTimer += Time.deltaTime;
        yTimer += Time.deltaTime * ySpeed;

        xOffset = xTimer * xSpeed;
        yOffset = a*(yTimer*yTimer)+b*yTimer+c;
        transform.position = initialPos + (isRight ? new Vector3(xOffset,yOffset,0f) : new Vector3(-xOffset,yOffset,0f));
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }    

    
    
        
}
