using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngrySunScript : MonoBehaviour
{
    public Transform cameraPos;
    public bool isLeft;
    public float offsetUp;
    public float offsetSide;

    public bool isActive;

    Vector3 rotationCenter;
    float rotationRadius = 2f;
    float angularSpeed = 11f;

    float posX, posY;
    float angle;

    private float journeyTime = 1.5f;
    private float startTime;

    public string stateString = "StandStill";

    public float timer = 0f;
    public float currentDelay;
    public float standStillDelay;
    public float rotatingDelay;

    public GameObject player;
    public Vector3 playerPos;
    public float sweepArc;

    bool descending = false;
    bool hasEntered = false;

    public float hoverHeight = 12f;
    public float hoverSpeed = 5f;
    public float descendSpeed = 12f;

    void Start()
    {
        Vector3 hoverPos = new Vector3(
            player.transform.position.x,
            player.transform.position.y + hoverHeight,
            transform.position.z
        );

        transform.position = hoverPos;
        rotationCenter = transform.position;
        currentDelay = standStillDelay;
    }

    void Update()
    {
        // OFF STATE
        if (!isActive)
        {
            Vector3 hoverPos = new Vector3(
                player.transform.position.x,
                player.transform.position.y + hoverHeight,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                hoverPos,
                hoverSpeed * Time.deltaTime
            );

            descending = false;
            hasEntered = false;
            return;
        }

        // START DESCENT ONCE
        if (!hasEntered)
        {
            descending = true;
            hasEntered = true;
        }

        if (descending)
        {
            Vector3 target = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                descendSpeed * Time.deltaTime
            );

            if (Mathf.Abs(transform.position.y - target.y) < 0.1f)
            {
                transform.position = target;
                descending = false;
            }

            return;
        }

        sweepArc = Mathf.Clamp((cameraPos.position.y - playerPos.y) / 4f, 0.1f, 1f);

        float t = Mathf.InverseLerp(0.1f, 1f, sweepArc);
        sweepArc = Mathf.Lerp(10f, 1f, t);

        timer += Time.deltaTime;

        if (timer >= currentDelay)
        {
            SwitchState();
        }
    }

    void LateUpdate()
    {
        if (!isActive || descending)
        {
            return;
        }

        if (stateString == "StandStill")
        {
            transform.position = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            currentDelay = standStillDelay;
        }

        if (stateString == "Rotating")
        {
            rotationCenter = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

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
            Vector3 sunrise = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            Vector3 sunset = new Vector3(
                cameraPos.position.x - (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            Vector3 center = (sunrise + sunset) * 0.5f;
            center += new Vector3(0, sweepArc, 0);

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
        playerPos = player.transform.position;

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