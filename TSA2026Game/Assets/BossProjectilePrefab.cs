using UnityEngine;

public class BossProjectilePrefab : MonoBehaviour
{
    public Transform player;       // Assign in inspector
    public float delay = 2f;       // Time before movement starts
    public float speed = 5f;       // Movement speed

    private Vector3 moveDirection;
    private bool started = false;
    private float timer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerTag").GetComponent<Transform>();
    }
    void Update()
    {
        // Wait for delay
        if (!started)
        {
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                // Capture direction ONCE
                moveDirection = (player.position - transform.position).normalized;
                started = true;
            }
        }
        else
        {
            // Move forever in that direction
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }
}