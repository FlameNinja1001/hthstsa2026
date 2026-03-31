using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeperScript : MonoBehaviour
{
    public Transform timeKeeper;
    public Transform player;
    public float threshold;

    public float maxX;
    public float maxY;
    public float minX;
    public float minY;
    public bool isTeleporting;
    public float tpDuration;
    public float idleDuration;

    private float timer;
    public bool hasSpawnedPrefab;
    public GameObject projectilePrefab;
    public GameObject mesh;

    public Animator animator;
    private Vector3 meshScale;
    private float bossScale;

    private Vector3 hiddenScale = new Vector3(0.001f, 0.010f, 0.001f);

    void Start()
    {
        meshScale = mesh.transform.localScale;
        bossScale = timeKeeper.localScale.x;
    }

    void Update()
    {
        // Flip toward player
        if (player.position.x >= timeKeeper.position.x)
        {
            timeKeeper.localScale = new Vector3(-bossScale, timeKeeper.localScale.y, timeKeeper.localScale.z);
        }
        else
        {
            timeKeeper.localScale = new Vector3(bossScale, timeKeeper.localScale.y, timeKeeper.localScale.z);
        }

        // Teleport animation trigger window
        if (((0.5f >= timer) && (timer >= 0f)) || ((idleDuration - 0.5f <= timer) && (timer <= idleDuration)))
        {
            animator.SetBool("IsTeleporting", true);
        }
        else
        {
            animator.SetBool("IsTeleporting", false);
        }

        timer += Time.deltaTime;

        // Shoot
        if (timer >= idleDuration / 4 && !hasSpawnedPrefab)
        {
            Instantiate(projectilePrefab, new Vector3(timeKeeper.transform.position.x,timeKeeper.transform.position.y,projectilePrefab.transform.position.z), projectilePrefab.transform.rotation);
            StartCoroutine(ShootAnim());
            hasSpawnedPrefab = true;
        }

        // Teleport trigger
        if (timer >= idleDuration && !isTeleporting)
        {
            StartCoroutine(Teleport());
        }
        else if (isTeleporting)
        {
            timer = 0f;
            hasSpawnedPrefab = false;
        }
    }

    public IEnumerator Teleport()
    {
        isTeleporting = true;

        // Smooth shrink
        StartCoroutine(LerpScale(mesh.transform.localScale, hiddenScale, 0.2f));

        timeKeeper.gameObject.GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(tpDuration);

        // Random position
        timeKeeper.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), timeKeeper.position.z);

        // Ensure not too close to player
        while (Vector3.Distance(timeKeeper.position, player.position) <= threshold)
        {
            timeKeeper.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), timeKeeper.position.z);
        }

        yield return new WaitForSeconds(0.1f);

        timeKeeper.gameObject.SetActive(true);
        isTeleporting = false;

        // Smooth grow back
        StartCoroutine(LerpScale(mesh.transform.localScale, meshScale, 0.2f));

        mesh.SetActive(true);
        timeKeeper.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public IEnumerator ShootAnim()
    {
        animator.SetBool("IsShooting", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsShooting", false);
    }

    IEnumerator LerpScale(Vector3 start, Vector3 target, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            mesh.transform.localScale = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mesh.transform.localScale = target;
    }
}