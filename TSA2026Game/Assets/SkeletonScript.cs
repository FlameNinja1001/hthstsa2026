using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{
    Rigidbody rb;
    public Transform player;
    public float moveDirection = 1;
    public float walkSpeed;
    public bool isGrounded;
    public float rayCastDistance;
    public float timer;
    public float inBetweenJumpDuration;
    public string currentState;
    public float jumpVelocity;
    public float leapSpeed;
    public float jumpTimer;
    public float actualJumpDuration;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = "Walking";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState == "Jumping")
        {            
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= actualJumpDuration)
            {
                currentState = "Walking";
            }
        }
        else
        {
            jumpTimer = 0;
        }        
        if (timer >= inBetweenJumpDuration && currentState == "Walking")
        {
            currentState = "Jumping";
        }
        if (isGrounded)
        {
            if (player.position.x >= transform.position.x)
            {
                moveDirection = 1;
            }
            else
            {
                moveDirection = -1;
            }
        }
        
        if (currentState == "Walking" && isGrounded)
        {
            timer += Time.deltaTime;
            rb.velocity = new Vector3(walkSpeed * moveDirection,rb.velocity.y,rb.velocity.z);
        }
        else if (currentState == "Walking" && !isGrounded)
        {
            timer = 0f;
            jumpTimer = 0f;
            rb.velocity = new Vector3(leapSpeed * moveDirection,rb.velocity.y,rb.velocity.z);
            rb.velocity += new Vector3(0f,-1f,0f);
        }
        else
        {
            timer = 0f;
            rb.velocity = new Vector3(leapSpeed * moveDirection, jumpVelocity,rb.velocity.z);
        }
        
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayCastDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
}
