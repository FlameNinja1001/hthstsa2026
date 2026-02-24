using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public GameObject heartObj;
    public int chance = 90;
    public GameObject mesh;
    public float hitDelay = 0.05f;
    public GameObject explosionPrefab;
    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(HitCoroutine());
        if (health <= 0)
        {
            Kill();
        }
    }
    public void Kill()
    {
        int randomNumber = UnityEngine.Random.Range(0, 100);

        if (randomNumber >= chance)
        {
            Instantiate(heartObj, transform.position, heartObj.transform.rotation);
        }
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
        Destroy(gameObject);
    }

    public IEnumerator HitCoroutine()
    {
        mesh.SetActive(false);
        yield return new WaitForSeconds(hitDelay);
        mesh.SetActive(true);
    }

}
