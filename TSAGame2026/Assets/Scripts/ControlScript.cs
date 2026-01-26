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

    public bool isGrounded;

    public float jumpForce = 1f;
    public float gravity = 10f;
    public bool canJump = true;
    public bool canDash = true;
    public float dashSpeed = 5f;    
    public int moveDirection = 1;
    public bool isDashing;
    public float dashDuration = 1f;
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
        if (!jump && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z);
        }
    
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

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

        jump = input.Player.Jump.IsPressed();
        move = input.Player.Move.ReadValue<Vector2>();
        dash = input.Player.Dash.IsPressed();
        if (!isDashing)
        {
            rb.linearVelocity = new Vector3(move.x * moveLeftRightSpeed, rb.linearVelocity.y,rb.linearVelocity.z);
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

}
