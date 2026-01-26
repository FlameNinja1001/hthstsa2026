using UnityEngine;
using UnityEngine.InputSystem;

public class ControlScript : MonoBehaviour
{
    PlayerInputActions input;
    public Vector2 move;  

    public Rigidbody rb;

    public bool jump;
    public float moveLeftRightSpeed;
    public float rayLength = 1f;

    public bool isGrounded;

    public float jumpForce = 1f;
    public float gravity = 10f;
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
        rb.linearVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
        if (isGrounded && jump)
        {            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,jumpForce,rb.linearVelocity.z);
        }
        if (!jump && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z);
        }
    
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

        
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
        rb.linearVelocity = new Vector3(move.x * moveLeftRightSpeed, rb.linearVelocity.y,rb.linearVelocity.z);
    }

}
