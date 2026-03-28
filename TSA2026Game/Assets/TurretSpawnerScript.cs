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

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnTurret();
            timer = 0f;
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