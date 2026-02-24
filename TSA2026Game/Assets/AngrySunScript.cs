using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngrySunScript : MonoBehaviour
{
    public Transform cameraPos;
    public bool isLeft;
    public float offsetUp;
    public float offsetSide;
    Vector3 rotationCenter;
    float rotationRadius = 2f, angularSpeed = 11f;

    float posX, posY;
    
    float angle;
    private float journeyTime = 1.5f;
    private float startTime;

    // The time at which the animation started.    

    public string stateString = "StandStill";

    public float timer = 0f;
    public float currentDelay;
    public float standStillDelay;
    public float rotatingDelay;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(cameraPos.position.x + (isLeft ? -offsetSide : offsetSide), cameraPos.position.y + offsetUp, transform.position.z);
        rotationCenter = transform.position;
        currentDelay = standStillDelay;
    }

    // Update is called once per frame

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentDelay)
        {
            SwitchState();
        }
    }
    void LateUpdate()
    {  
        if (stateString == "StandStill")
        {
            transform.position = new Vector3(cameraPos.position.x + (isLeft ? -offsetSide : offsetSide), cameraPos.position.y + offsetUp, transform.position.z);
            currentDelay = standStillDelay;
        }       
        
        if (stateString == "Rotating")
        {
            rotationCenter = new Vector3(cameraPos.position.x + (isLeft ? -offsetSide : offsetSide), cameraPos.position.y + offsetUp, transform.position.z);   
            posX = rotationCenter.x + Mathf.Cos(angle) * rotationRadius;
            posY = rotationCenter.y - Mathf.Sin(angle) * rotationRadius;
            transform.position = new Vector3(posX, posY, transform.position.z);
            angle += Time.deltaTime * angularSpeed;
            if (angle >= 360f)
            {
                angle = 0f;
            }
            currentDelay = rotatingDelay;
        }        
        if (stateString == "Sweep")
        {            
            Vector3 sunrise = new Vector3(cameraPos.position.x + (isLeft ? -offsetSide : offsetSide), cameraPos.position.y + offsetUp, transform.position.z);
            Vector3 sunset = new Vector3(cameraPos.position.x - (isLeft ? -offsetSide : offsetSide), cameraPos.position.y + offsetUp, transform.position.z);
            Vector3 center = (sunrise + sunset) * 0.5F;
            
            center += new Vector3(0, 1, 0);
            
            Vector3 riseRelCenter = sunrise - center;
            Vector3 setRelCenter = sunset - center;

            float fracComplete = (Time.time - startTime) / journeyTime;

            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            transform.position += center;            
            currentDelay = journeyTime;
        }           

    }
    

    void SwitchState()
    {
        if (stateString == "StandStill")
        {        
            stateString = "Rotating";
        }
        else if (stateString == "Rotating")
        {
            stateString = "Sweep";
        }
        else
        {
            isLeft = !isLeft;
            stateString = "StandStill";
            
        }
        startTime = Time.time;
        timer = 0f;
    }
    
}
