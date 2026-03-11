using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    public bool followX = true;
    public bool followY = true;

    public float deadZoneX = 2f;
    public float deadZoneY = 1.5f;

    public float lookAhead = 2f;
    public float smoothSpeed = 5f;

    float lastTargetX;

    void Start()
    {
        lastTargetX = target.position.x;
    }

    void Update()
    {
        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position;

        float moveDir = Mathf.Sign(target.position.x - lastTargetX);

        float targetX = camPos.x;
        float targetY = camPos.y;

        if (followX)
        {
            float leftEdge = camPos.x - deadZoneX;
            float rightEdge = camPos.x + deadZoneX;

            float desiredX = targetPos.x + (lookAhead * moveDir);

            if (desiredX < leftEdge)
                targetX = desiredX + deadZoneX;

            if (desiredX > rightEdge)
                targetX = desiredX - deadZoneX;
        }

        if (followY)
        {
            float bottomEdge = camPos.y - deadZoneY;
            float topEdge = camPos.y + deadZoneY;

            if (targetPos.y < bottomEdge)
                targetY = targetPos.y + deadZoneY;

            if (targetPos.y > topEdge)
                targetY = targetPos.y - deadZoneY;
        }

        Vector3 targetCameraPos = new Vector3(targetX, targetY, camPos.z);

        transform.position = Vector3.Lerp(transform.position, targetCameraPos, smoothSpeed * Time.deltaTime);

        lastTargetX = target.position.x;
    }
}