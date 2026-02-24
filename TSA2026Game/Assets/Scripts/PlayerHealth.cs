using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public int health;
    public RawImage[] rawImages;
    public bool canBeDamaged;
    public GameObject mesh;
    public float delayDuration;
    public float delayIteration;
    public int healthMax = 4;
    public bool canPlayerMove = true;

    public GameObject deathPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(deathPrefab, new Vector3(transform.position.x,transform.position.y,deathPrefab.transform.position.z), deathPrefab.transform.rotation);
            Destroy(gameObject);
        }
        for (int i = 0; i < rawImages.Length; i++)
        {
            if (i < health)
            {
                rawImages[i].enabled = true;
            }
            else
            {
                rawImages[i].enabled = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Enemy"))
        {            
            if (canBeDamaged)
            {
                health -= 1;
                StartCoroutine(DamagePlayer());
            }
        }
    }

    public IEnumerator DamagePlayer()
    {
        canBeDamaged = false;
        canPlayerMove = false;
        for (int i = 0; i < delayIteration; i++)
        {
            if (i > delayIteration / 3)
            {
                canPlayerMove = true;
            }
            mesh.SetActive(!mesh.activeSelf);
            yield return new WaitForSeconds(delayDuration);
        }
        mesh.SetActive(true);
        canPlayerMove = true;
        canBeDamaged = true;

    }
}
