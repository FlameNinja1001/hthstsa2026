using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float life = 1.5f;
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
    }

    void OnDestroy()
    {
        ProjectileScript.currentShots -= 1;     
    }
}
