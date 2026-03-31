using UnityEngine;

public class BossProjectilePrefab : MonoBehaviour
{
    public Transform player;
    public float delay = 2f;
    public float speed = 5f;

    [Header("Spawn Animation")]
    public GameObject animatedObject;      // Assign the object to scale up
    public float scaleDuration = 0.5f;     // How long the scale-up takes

    private Vector3 moveDirection;
    private bool started = false;
    private float timer = 0f;

    private Vector3 initialScale;
    private float scaleTimer = 0f;
    private bool scaleComplete = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerTag").GetComponent<Transform>();

        if (animatedObject != null)
        {
            initialScale = animatedObject.transform.localScale;
            animatedObject.transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        // Handle scale animation
        if (!scaleComplete && animatedObject != null)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / scaleDuration);

            // Smooth ease-out feel
            t = 1f - Mathf.Pow(1f - t, 3f);

            animatedObject.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, initialScale, t);

            if (scaleTimer >= scaleDuration)
                scaleComplete = true;
        }

        // Wait for delay then move
        if (!started)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                moveDirection = (player.position - transform.position).normalized;
                started = true;
            }
        }
        else
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }
}