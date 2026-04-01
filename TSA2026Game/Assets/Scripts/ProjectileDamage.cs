using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float life = 1.5f;
    public int damageAmount;
    public bool isGood = true;
    private void Awake()
    {
        Destroy(gameObject,life);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);

        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
        else if (other.GetComponent<EnemyHealth>() != null)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null && isGood)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        ProjectileScript.currentShots -= 1;     
    }
}
