using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float life = 1.5f;
    public int damageAmount;
    private void Awake()
    {
        Destroy(gameObject,life);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {            
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Enemy"))
        {            
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
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
