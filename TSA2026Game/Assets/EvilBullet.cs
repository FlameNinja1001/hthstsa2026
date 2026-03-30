using UnityEngine;

public class EvilBullet : MonoBehaviour
{    
    void Start()
    {
        
    }
    public float life = 1.5f;    
    private void Awake()
    {
        Destroy(gameObject,life);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name + 
                " | Tag: " + collision.gameObject.tag);

        PhysicMaterial mat = collision.collider.sharedMaterial;

        if (mat != null)
        {
            Debug.Log("Hit material: " + mat.name);
        }
        if (collision.gameObject.CompareTag("PlayerTag") || collision.gameObject.CompareTag("Ground") || (collision.gameObject.CompareTag("Untagged") && mat.name == "Ground") )
        {
            Invoke("DelayedMethod", 0.1f);            
        }
    }    

    void DelayedMethod()
    {
        Destroy(gameObject);
    }
    
    
}
