using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public bool isRight;
    public bool isLeft;
    public bool isUp;
    public bool isDown;

    public float speed = 5f;

    public GameObject mesh;

    public Vector3 rightRot;
    public Vector3 leftRot;
    public Vector3 upRot;
    public Vector3 downRot;

    void Start()
    {
        // Set rotation based on direction
        if (isRight)
            mesh.transform.localEulerAngles = rightRot;

        if (isLeft)
            mesh.transform.localEulerAngles = leftRot;

        if (isUp)
            mesh.transform.localEulerAngles = upRot;

        if (isDown)
            mesh.transform.localEulerAngles = downRot;
    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if (isRight) moveDir = Vector3.right;
        if (isLeft) moveDir = Vector3.left;
        if (isUp) moveDir = Vector3.up;
        if (isDown) moveDir = Vector3.down;

        transform.position += moveDir * speed * Time.deltaTime;
    }
}