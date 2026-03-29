using UnityEngine;

public class EvilBullet : MonoBehaviour
{
    public float life = 1.5f;    
    private void Awake()
    {
        Destroy(gameObject,life);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerTag") || collision.gameObject.CompareTag("Ground"))
        {
            Invoke("DelayedMethod", 0.1f);            
        }
    }

    void DelayedMethod()
    {
        Destroy(gameObject);
    }
    
}
