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

    // ⭐ DIRECTIONAL SHINE
    public GameObject leftShine;
    public GameObject rightShine;

    public float shineSeconds;
    public float shineAnimSeconds;

    private float shineTimer;
    private bool hasShined;

    void Start()
    {
        ogScaleX = mesh.localScale.x;
        rb = GetComponent<Rigidbody>();
        currentState = "Walking";
        boxCollider = GetComponent<BoxCollider>();

        if (leftShine != null) leftShine.SetActive(false);
        if (rightShine != null) rightShine.SetActive(false);
    }

    void FixedUpdate()
    {
        // Activation check
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
            // Collider swap
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

            animator.SetBool("IsGrounded", isGrounded);

            // Jump duration handling
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

            // ⭐ SHINE + JUMP LOGIC
            if (currentState == "Walking")
            {
                timer += Time.deltaTime;

                // Start shine BEFORE jump
                if (timer >= inBetweenJumpDuration - shineSeconds && !hasShined)
                {
                    shineTimer = 0f;
                    hasShined = true;
                }

                // Handle shine
                if (hasShined)
                {
                    shineTimer += Time.deltaTime;

                    // Direction-based shine switching
                    if (moveDirection == 1)
                    {
                        if (rightShine != null) rightShine.SetActive(true);
                        if (leftShine != null) leftShine.SetActive(false);
                    }
                    else
                    {
                        if (leftShine != null) leftShine.SetActive(true);
                        if (rightShine != null) rightShine.SetActive(false);
                    }

                    // End shine
                    if (shineTimer >= shineAnimSeconds)
                    {
                        if (leftShine != null) leftShine.SetActive(false);
                        if (rightShine != null) rightShine.SetActive(false);
                    }
                }

                // Trigger jump
                if (timer >= inBetweenJumpDuration)
                {
                    currentState = "Jumping";
                    timer = 0f;
                    hasShined = false;
                }
            }

            // Direction + rotation
            if (isGrounded)
            {
                mesh.localRotation = Quaternion.Euler(0, 0, 0);

                if (player.position.x >= transform.position.x)
                {
                    moveDirection = 1;
                    mesh.localScale = new Vector3(ogScaleX, mesh.localScale.y, mesh.localScale.z);
                }
                else
                {
                    moveDirection = -1;
                    mesh.localScale = new Vector3(-ogScaleX, mesh.localScale.y, mesh.localScale.z);
                }
            }
            else
            {
                mesh.localRotation = Quaternion.Euler(0, 45 * -moveDirection, 0);
            }

            // Movement
            if (currentState == "Walking" && isGrounded)
            {
                rb.velocity = new Vector3(walkSpeed * moveDirection, rb.velocity.y, rb.velocity.z);
            }
            else if (currentState == "Walking" && !isGrounded)
            {
                jumpTimer = 0f;
                rb.velocity = new Vector3(leapSpeed * moveDirection, rb.velocity.y, rb.velocity.z);
                rb.velocity += new Vector3(0f, -1f, 0f);
            }
            else // Jumping
            {
                rb.velocity = new Vector3(leapSpeed * moveDirection, jumpVelocity, rb.velocity.z);
            }

            // Ground check
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
            // FULL RESET
            timer = 0f;
            jumpTimer = 0f;

            currentState = "Walking";

            rb.velocity = Vector3.zero;

            isGrounded = false;

            mesh.localRotation = Quaternion.Euler(0, 0, 0);

            moveDirection = 1;
            mesh.localScale = new Vector3(ogScaleX, mesh.localScale.y, mesh.localScale.z);

            animator.SetBool("IsGrounded", false);

            // ⭐ RESET SHINES
            if (leftShine != null) leftShine.SetActive(false);
            if (rightShine != null) rightShine.SetActive(false);

            hasShined = false;
            shineTimer = 0f;
        }
    }
}