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
    
    public bool hasUsedWallJumpDouble = false;

    public bool isRaycastHittingWall = false;
    public bool isWallSliding = false;

    public float wallJumpPushHoriz;
    public float wallJumpPushVertical;
    public float wallJumpDuration = 1f;
    public bool isWallJumping = false;
    public bool canWallJump = false;
    
    public bool jumpReleased = true;
    public bool animBool;
    public float doubleJumpAnim = 0.5f;

    public bool hasAirDashed = false;
    public float damageSpeed;
    private Collider m_ObjectCollider;
    bool tmPlayerMove;

    void Awake()
    {
        input = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        m_ObjectCollider = GetComponent<CapsuleCollider>();
        
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
        TMScript tMScript = FindObjectOfType<TMScript>(); 
        if (tMScript != null)
        {
            tmPlayerMove = tMScript.canPlayerMove;
        }        
        else
        {
            tmPlayerMove = true;
        }
        if (!BossRoomScript.canPlayerMove)
        {
            rb.velocity = new Vector3(0,0,0);
            return;
        }
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();               
        if (playerHealth.canBeDamaged)
        {
            Collider[] all = FindObjectsOfType<Collider>();

            foreach (Collider col in all)
            {
                if (!col.CompareTag("Ground") && !col.CompareTag("SunTrigger"))
                {
                    Physics.IgnoreCollision(m_ObjectCollider, col, false);
                }
            }           
        }
        else 
        {
            Collider[] all = FindObjectsOfType<Collider>();

            foreach (Collider col in all)
            {
                if (!col.CompareTag("Ground") && !col.CompareTag("SunTrigger"))
                {
                    Physics.IgnoreCollision(m_ObjectCollider, col, true);
                }
            }            
        }
        
        if (playerHealth.canPlayerMove && tmPlayerMove)
        {            
                                
            jump = input.Player.Jump.IsPressed();
            move = input.Player.Move.ReadValue<Vector2>();
            dash = input.Player.Dash.IsPressed();

            if (isGrounded)
            {
                hasAirDashed = false;
            }
        
            if (!jump)
                jumpReleased = true;

            if (isWallSliding && !jump)
                canWallJump = true;

            if (isRaycastHittingWall && move.x != 0 && !isGrounded && (!jump || rb.velocity.y <= 0))
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
                rb.velocity = new Vector3(rb.velocity.x, -slideGravity, rb.velocity.z);
            }
            else
            {
                gravity = normalGravity;
            }
           
            if (canDoubleJump && jump && jumpReleased && !hasDoubleJumped && !isWallJumping)
            {
                StartCoroutine(AnimBoolRoutine());
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                hasDoubleJumped = true;
                hasUsedWallJumpDouble = true;
                jumpReleased = false; 
            }

            if (move.x > 0)
                moveDirection = 1;
            else if (move.x < 0)
                moveDirection = -1;

            if (!jump)
                canJump = true;

            rb.velocity += Vector3.down * gravity * Time.fixedDeltaTime;

            if (isGrounded && jump && canJump && !isWallJumping)
            {
                canJump = false;            
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            }
            else if (!jump && !isWallSliding && !hasUsedWallJumpDouble)
            {
                canDoubleJump = true;
            }

            if (!jump && rb.velocity.y > 0 && !isWallJumping && !isWallSliding)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                if (!hasUsedWallJumpDouble)
                    canDoubleJump = true;
            }

            Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);
            Debug.DrawRay(transform.position, ((moveDirection > 0) ? Vector3.right : Vector3.left) * sideRayLength, Color.red);

            if (dash && canDash && !isDashing && !hasAirDashed)
            {
                canDash = false;
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                StartCoroutine(DashCoroutine());          
            }

            if (!dash)
                canDash = true;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    isGrounded = true;
                    canDoubleJump = false;
                    hasDoubleJumped = false;


                    hasUsedWallJumpDouble = false;
                    jumpReleased = true;
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
                    isRaycastHittingWall = true;
                else
                    isRaycastHittingWall = false;
            }
            else
                isRaycastHittingWall = false;       

            if (!isDashing && !isWallJumping)
                rb.velocity = new Vector3(move.x * moveLeftRightSpeed, rb.velocity.y, rb.velocity.z);

            if (jump && isWallSliding && canWallJump)
                StartCoroutine(WallJumpCoroutine());            
            
        }
        else if (!playerHealth.canPlayerMove)
        {
            gravity = normalGravity;
            rb.velocity = new Vector3(moveDirection * -damageSpeed, -gravity, rb.velocity.z);
        }
        else if (!tmPlayerMove)
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
        
    }

    public IEnumerator DashCoroutine()
    {
        float timer = 0;
        isDashing = true;        
        while (timer < dashDuration)
        {
            rb.velocity = new Vector3(dashSpeed * moveDirection, rb.velocity.y, rb.velocity.z);
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        if (!isGrounded)
        {
            hasAirDashed = true;
        }
    }

    public IEnumerator WallJumpCoroutine()
    {
        float timer = 0;
        isWallJumping = true;
        canJump = false;

        int initialMoveDir = moveDirection;

       
        jumpReleased = false;

        rb.velocity = new Vector3(wallJumpPushHoriz * -initialMoveDir, wallJumpPushVertical, rb.velocity.z);

        
        if (!hasUsedWallJumpDouble)
        {
            canDoubleJump = true;
            hasDoubleJumped = false;
        }

        while (timer < wallJumpDuration)
        {
            rb.velocity = new Vector3(wallJumpPushHoriz * -initialMoveDir, rb.velocity.y, rb.velocity.z);
            timer += Time.deltaTime;
            yield return null;
        }

        isWallJumping = false;    
        canWallJump = false;            
    }
    public IEnumerator AnimBoolRoutine()
    {
        animBool = true;
        yield return new WaitForSeconds(doubleJumpAnim);
        animBool = false;
    }
}
