using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawnerScript : MonoBehaviour
{
    public GameObject turretPrefab;

    public float spawnRate = 1f;

    public bool isRight;
    public bool isLeft;
    public bool isUp;
    public bool isDown;

    float timer;

    public bool isTurret;

    public Transform player;

    public float treshold;


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            if (!isTurret)
            {
                SpawnTurret();
                timer = 0f;
            }   
            else if ((Mathf.Abs(player.transform.position.x - transform.parent.position.x) < treshold) && (Mathf.Abs(player.transform.position.y - transform.parent.position.y) < treshold))
            {
                SpawnTurret();
                timer = 0f;
            }         
        }
    }

    void SpawnTurret()
    {
        GameObject turret = Instantiate(turretPrefab, transform.position, Quaternion.identity);

        TurretScript script = turret.GetComponent<TurretScript>();

        if (script != null)
        {
            script.isRight = isRight;
            script.isLeft = isLeft;
            script.isUp = isUp;
            script.isDown = isDown;
        }
    }
}