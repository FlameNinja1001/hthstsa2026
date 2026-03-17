using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{
    public Vector3 sizeBefore;
    public Vector3 centerBefore;

    public Vector3 sizeAfter;
    public Vector3 centerAfter;
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

    public Transform mesh;
    public Animator animator;

    public float ogScaleX;
    public float distPlayer;
    public bool isActive;
    public bool hasBeenActive;
    BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        ogScaleX = mesh.localScale.x;
        rb = GetComponent<Rigidbody>();
        currentState = "Walking";
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        if (Vector3.Distance(player.position, transform.position) < distPlayer)
        {
            isActive = true;            
        }
        else if (Vector3.Distance(player.position, transform.position) > distPlayer * 1.5)
        {
            isActive = false;            
        }
        if (isActive)
        {
            if (!isGrounded)
            {
                boxCollider.size = sizeAfter;
                boxCollider.center = centerAfter;
            }
            else
            {
                boxCollider.size = sizeBefore;
                boxCollider.center = centerBefore;
            }
            animator.SetBool("IsGrounded",isGrounded);
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
                mesh.localRotation = Quaternion.Euler(0,0,0);
                if (player.position.x >= transform.position.x)
                {
                    moveDirection = 1;
                    mesh.localScale = new Vector3(ogScaleX, mesh.localScale.y,mesh.localScale.z);
                }
                else
                {
                    moveDirection = -1;
                    mesh.localScale = new Vector3(-ogScaleX, mesh.localScale.y,mesh.localScale.z);
                }
            }
            else
            {
                mesh.localRotation = Quaternion.Euler(0,45 * -moveDirection,0);
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
        else
        {
            // Reset timers
            timer = 0f;
            jumpTimer = 0f;

            // Reset state
            currentState = "Walking";

            // Reset movement
            rb.velocity = Vector3.zero;

            // Reset grounding
            isGrounded = false;

            // Reset mesh rotation
            mesh.localRotation = Quaternion.Euler(0, 0, 0);

            // Face default direction
            moveDirection = 1;
            mesh.localScale = new Vector3(ogScaleX, mesh.localScale.y, mesh.localScale.z);

            // Reset animator
            animator.SetBool("IsGrounded", false);
        }    
    }
}
