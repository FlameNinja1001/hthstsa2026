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

    // BOSS MODE
    public bool isBoss;

    public float bossX1;
    public float bossX2;
    public float bossMoveSpeed = 4f;

    bool bossAtPos1 = true;
    public string loadStringCopy;

    void Start()
    {
        GameOverScript.loadString = loadStringCopy;
        lastTargetX = target.position.x;
    }

    void Update()
    {
        if (isBoss)
        {
            BossCamera();
            return;
        }

        NormalCamera();
    }

    void NormalCamera()
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

    void BossCamera()
    {
        float targetX = bossAtPos1 ? bossX1 : bossX2;

        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, bossMoveSpeed * Time.deltaTime);
    }

    public void SwitchBossPos()
    {
        bossAtPos1 = !bossAtPos1;
    }
}