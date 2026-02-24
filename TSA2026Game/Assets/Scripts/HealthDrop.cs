using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    public Rigidbody rb;
    public float rayLength = 1.1f;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            isGrounded = hit.collider.CompareTag("Ground");
        }
        else
        {
            isGrounded = false;
        }
        rb.isKinematic = isGrounded;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerTag"))
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth.health < playerHealth.healthMax)
            {
                playerHealth.health += 1;
            }
            Destroy(gameObject);
        }
    }

    
}
