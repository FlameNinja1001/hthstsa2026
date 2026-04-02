using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancelotBossScript : MonoBehaviour
{
    private Rigidbody rb;

    public string stateString = "Walking";

    public float walkSpeed;
    public Transform player;

    public float ogScaleX;
    public Transform model;

    public float moveDir;
    public float dashSpeed;

    public Animator animator;

    public float customX = 1.447315f;
    public float ogModelX = 1.106341f;

    public bool jumpInitialVelocitySet;
    public float jumpVelocityY;
    public float jumpVelocityX;
    public float jumpVelocityXInitial;
    public float jumpVelocityXFall;

    public float initialGravity;
    public float fallGravity;
    public float gravity;

    public float playerDistThreshold;
    public GameObject collider;

    public float walkDuration = 2f;
    public float dashDuration = 1f;
    private float stateTimer;

    public float groundRayDistance = 1.5f;
    public bool isGrounded;

    // ⭐ SHINE
    public GameObject shineObject;
    public float shineSeconds;      // when to start shine before transition
    public float shineAnimSeconds;  // how long shine stays on

    private float shineTimer;
    private bool hasShined;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ogScaleX = transform.localScale.x;

        stateTimer = walkDuration;

        if (shineObject != null)
            shineObject.SetActive(false);
    }

    void FixedUpdate()
    {
        CheckGround();

        // ================= WALKING =================
        if (stateString == "Walking")
        {
            gravity = initialGravity;
            jumpVelocityX = jumpVelocityXInitial;
            stateTimer -= Time.fixedDeltaTime;

            // ⭐ START SHINE BEFORE TRANSITION
            if (stateTimer <= shineSeconds && !hasShined)
            {
                hasShined = true;
                shineTimer = 0f;

                if (shineObject != null)
                    shineObject.SetActive(true);
            }

            // ⭐ HANDLE SHINE TIMER
            if (hasShined)
            {
                shineTimer += Time.fixedDeltaTime;

                if (shineTimer >= shineAnimSeconds)
                {
                    if (shineObject != null)
                        shineObject.SetActive(false);
                }
            }

            if (Mathf.Abs(transform.position.x - player.position.x) < playerDistThreshold)
            {
                collider.SetActive(true);
                animator.SetBool("isSlashing", true);
            }
            else
            {
                animator.SetBool("isSlashing", false);
                collider.SetActive(false);
            }

            jumpInitialVelocitySet = false;
            animator.SetBool("isDashing", false);
            animator.SetBool("isUppercutting", false);

            if (player.position.x > transform.position.x)
            {
                rb.velocity = new Vector3(walkSpeed, 0f, 0f);
                transform.localScale = new Vector3(-ogScaleX, transform.localScale.y, transform.localScale.z);
                model.localRotation = Quaternion.Euler(0, 0, 0);
                model.localScale = new Vector3(ogModelX, model.localScale.y, model.localScale.z);
                moveDir = 1;
            }
            else
            {
                rb.velocity = new Vector3(-walkSpeed, 0f, 0f);
                transform.localScale = new Vector3(ogScaleX, transform.localScale.y, transform.localScale.z);
                model.localRotation = Quaternion.Euler(0, 0, 0);
                model.localScale = new Vector3(ogModelX, model.localScale.y, model.localScale.z);
                moveDir = -1;
            }

            // ⭐ TRANSITION (AFTER SHINE WINDOW)
            if (stateTimer <= 0f)
            {
                // reset shine
                hasShined = false;
                if (shineObject != null)
                    shineObject.SetActive(false);

                if (Random.value < 0.5f)
                {
                    stateString = "Dashing";
                    stateTimer = dashDuration;
                }
                else
                {
                    stateString = "Jumping";
                }
            }
        }

        // ================= DASHING =================
        if (stateString == "Dashing")
        {
            gravity = initialGravity;
            jumpVelocityX = jumpVelocityXInitial;
            stateTimer -= Time.fixedDeltaTime;

            animator.SetBool("isSlashing", false);
            collider.SetActive(true);
            jumpInitialVelocitySet = false;

            animator.SetBool("isUppercutting", false);
            animator.SetBool("isDashing", true);

            rb.velocity = new Vector3(dashSpeed * moveDir, 0f, 0f);
            model.localRotation = Quaternion.Euler(0, 45, 0);
            model.localScale = new Vector3(customX, model.localScale.y, model.localScale.z);

            if (stateTimer <= 0f)
            {
                stateString = "Walking";
                stateTimer = walkDuration;
            }
        }

        // ================= JUMPING =================
        if (stateString == "Jumping")
        {
            animator.SetBool("isSlashing", false);
            collider.SetActive(false);

            if (!jumpInitialVelocitySet)
            {
                jumpInitialVelocitySet = true;
                rb.velocity = new Vector3(jumpVelocityX * moveDir, jumpVelocityY, 0f);
            }

            animator.SetBool("isUppercutting", true);
            animator.SetBool("isDashing", false);

            rb.velocity -= new Vector3(0f, gravity, 0f);

            model.localScale = new Vector3(ogModelX, model.localScale.y, model.localScale.z);

            if (rb.velocity.y < 0f)
            {
                gravity = fallGravity;
                jumpVelocityX = jumpVelocityXFall;
            }
            else
            {
                gravity = initialGravity;
                jumpVelocityX = jumpVelocityXInitial;
            }

            if (isGrounded && rb.velocity.y <= 0f)
            {
                stateString = "Walking";
                stateTimer = walkDuration;
            }
        }
    }

    // ================= GROUND CHECK =================
    void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundRayDistance))
        {
            if (hit.collider.CompareTag("Ground"))
                isGrounded = true;
            else
                isGrounded = false;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(transform.position, Vector3.down * groundRayDistance, isGrounded ? Color.green : Color.red);
    }
}