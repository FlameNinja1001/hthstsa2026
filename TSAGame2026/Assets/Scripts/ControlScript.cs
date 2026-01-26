using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ControlScript : MonoBehaviour
{
    PlayerInputActions input;
    public Vector2 move;  

    public Rigidbody rb;

    public bool jump;
    public bool dash;
    public float moveLeftRightSpeed;
    public float rayLength = 1f;
    public float sideRayLength = 1f;

    public bool isGrounded;

    public float jumpForce = 1f;
    public float gravity = 10f;
    public float normalGravity = 10f;
    public float slideGravity = 5f;
    public bool canJump = true;
    public bool canDash = true;
    public float dashSpeed = 5f;    
    public int moveDirection = 1;
    public bool isDashing;
    public float dashDuration = 1f;

    public bool canDoubleJump = false;
    public bool hasDoubleJumped = false;

    public bool isRaycastHittingWall = false;
    public bool isWallSliding = false;

    public float wallJumpPushHoriz;
    public float wallJumpPushVertical;
    public float wallJumpDuration = 1f;
    public bool isWallJumping = false;

    void Awake()
    {
        input = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void FixedUpdate()
    {        
        if (isRaycastHittingWall && move.x != 0 && !isGrounded && (!jump || rb.linearVelocity.y <= 0))
        {
            isWallSliding = true;
            canDoubleJump = false;
        }        
        else
        {
            isWallSliding = false;
        }
        if (isWallSliding)
        {
            gravity = 0f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,-slideGravity,rb.linearVelocity.z);
        }
        else
        {
            gravity = normalGravity;
        }
        if (canDoubleJump && jump && !hasDoubleJumped)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,jumpForce,rb.linearVelocity.z);
            hasDoubleJumped = true;
        }
        if (move.x > 0)
        {
            moveDirection = 1;
        }
        else if (move.x < 0)
        {
            moveDirection = -1;
        }
        if (!jump)
        {
            canJump = true;
        }
        rb.linearVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        if (isGrounded && jump && canJump)
        {
            canJump = false;            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,jumpForce,rb.linearVelocity.z);
        }
        else if (!jump)
        {
            canDoubleJump = true;
        }
        if (!jump && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z);
            canDoubleJump = true;
        }
    
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

        Debug.DrawRay(transform.position, ((moveDirection > 0) ? Vector3.right : Vector3.left) * sideRayLength, Color.red);

        if (dash && canDash && !isDashing)
        {
            canDash = false;
            rb.linearVelocity = new Vector3(0f,rb.linearVelocity.y,rb.linearVelocity.z);
            StartCoroutine(DashCoroutine());          
        }
        if (!dash)
        {
            canDash = true;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                canDoubleJump = false;
                hasDoubleJumped = false;
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

        RaycastHit hit2;
        if (Physics.Raycast(transform.position, (moveDirection > 0) ? Vector3.right : Vector3.left, out hit2, sideRayLength))
        {
            
            if (hit2.collider.CompareTag("Ground"))
            {
                isRaycastHittingWall = true;
            }
            else
            {
                isRaycastHittingWall = false;
            }
        }
        else
        {
            isRaycastHittingWall = false;
        }       

        jump = input.Player.Jump.IsPressed();
        move = input.Player.Move.ReadValue<Vector2>();
        dash = input.Player.Dash.IsPressed();
        if (!isDashing && !isWallJumping)
        {
            rb.linearVelocity = new Vector3(move.x * moveLeftRightSpeed, rb.linearVelocity.y,rb.linearVelocity.z);
        }
        if (jump && isWallSliding)
        {
            StartCoroutine(WallJumpCoroutine());
        }
    }

    public IEnumerator DashCoroutine()
    {
        float timer = 0;
        isDashing = true;
        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector3(dashSpeed * moveDirection,rb.linearVelocity.y,rb.linearVelocity.z);
            timer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        rb.linearVelocity = new Vector3(0f,rb.linearVelocity.y,rb.linearVelocity.z);
    }

    public IEnumerator WallJumpCoroutine()
    {
        float timer = 0;
        isWallJumping = true;
        int initialMoveDir = moveDirection;
        
        // Apply the jump force ONCE
        rb.linearVelocity = new Vector3(wallJumpPushHoriz * -initialMoveDir, wallJumpPushVertical, rb.linearVelocity.z);
        
        while (timer < wallJumpDuration)
        {
            // Only control horizontal movement during wall jump
            rb.linearVelocity = new Vector3(wallJumpPushHoriz * -initialMoveDir, rb.linearVelocity.y, rb.linearVelocity.z);
            timer += Time.deltaTime;
            yield return null;
        }
        isWallJumping = false;
        hasDoubleJumped = false;
        canDoubleJump = true;
    }

}
