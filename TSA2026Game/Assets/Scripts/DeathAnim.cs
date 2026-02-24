using UnityEngine;

public class DeathAnim : MonoBehaviour
{
    public Rigidbody rb;
    public float upwardVelocity = 8f;
    public float extraGravity = 20f;   // Increase for faster fall
    public float destroyAfter = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, upwardVelocity, 0);
    }

    void FixedUpdate()
    {
        // Apply extra downward force
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }
}