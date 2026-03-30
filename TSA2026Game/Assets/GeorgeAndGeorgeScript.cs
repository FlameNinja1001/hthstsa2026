using System.Collections;
using UnityEngine;

public class GeorgeAndGeorgeScript : MonoBehaviour
{
    [Header("Identity")]
    public bool isKing = false;

    [Header("Stage Positions")]
    public Transform rightSide;
    public Transform leftSide;
    public Transform middle;

    [Header("Intro Jump — King George")]
    public float introJumpHeight = 4f;
    public float introJumpDuration = 1.2f;
    public float introJumpDelay = 1f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int shotCount = 3;
    public float shotInterval = 0.35f;
    public float postShootPause = 1.8f;

    [Header("Projectile Movement")]
    public float launchSpeed = 10f;

    [Header("Dashing")]
    public float dashToMiddleDuration = 0.3f;
    public float dashBackDuration = 0.25f;
    public float dashMiddlePause = 0.15f;

    [Header("Side-Swap Jump")]
    public float swapJumpHeight = 3f;
    public float swapJumpDuration = 0.9f;
    public float preSwapPause = 0.5f;

    private Transform _mySide;

    void Start()
    {
        _mySide = isKing ? leftSide : rightSide;
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop()
    {
        if (isKing)
        {
            yield return new WaitForSeconds(introJumpDelay);
            yield return StartCoroutine(ArcJump(transform.position, leftSide.position,
                                                introJumpHeight, introJumpDuration));
        }
        else
        {
            // Wait for King's full intro before the fight loop begins
            yield return new WaitForSeconds(introJumpDelay + introJumpDuration);
        }

        while (true)
        {
            if (isKing)
                yield return StartCoroutine(KingLoop());
            else
                yield return StartCoroutine(WashingtonLoop());
        }
    }

    // ── King's self-contained loop ────────────────────────────────────────────
    // King shoots → idles while Washington dashes → both swap → idles while Washington shoots → King dashes

    IEnumerator KingLoop()
    {
        // King shoots
        yield return StartCoroutine(Shoot(shotCount, shotInterval));
        yield return new WaitForSeconds(postShootPause);

        // Idle for exactly as long as Washington's dash takes
        float washDashTotal = dashToMiddleDuration + dashMiddlePause + dashBackDuration + preSwapPause;
        yield return new WaitForSeconds(washDashTotal);

        // Both swap simultaneously — same duration, no coordination needed
        Transform kingTarget = (_mySide == leftSide) ? rightSide : leftSide;
        yield return StartCoroutine(ArcJump(transform.position, kingTarget.position,
                                            swapJumpHeight, swapJumpDuration));
        _mySide = kingTarget;

        // Idle for exactly as long as Washington's shoot takes
        float washShootTotal = shotCount * shotInterval + postShootPause;
        yield return new WaitForSeconds(washShootTotal);

        // King dashes
        yield return StartCoroutine(DashMiddleAndBack());
        yield return new WaitForSeconds(postShootPause);
    }

    // ── Washington's self-contained loop ──────────────────────────────────────
    // Idles while King shoots → Washington dashes → both swap → Washington shoots → idles while King dashes

    IEnumerator WashingtonLoop()
    {
        // Idle for exactly as long as King's shoot takes
        float kingShootTotal = shotCount * shotInterval + postShootPause;
        yield return new WaitForSeconds(kingShootTotal);

        // Washington dashes
        yield return StartCoroutine(DashMiddleAndBack());
        yield return new WaitForSeconds(preSwapPause);

        // Both swap simultaneously
        Transform washTarget = (_mySide == rightSide) ? leftSide : rightSide;
        yield return StartCoroutine(ArcJump(transform.position, washTarget.position,
                                            swapJumpHeight, swapJumpDuration));
        _mySide = washTarget;

        // Washington shoots
        yield return StartCoroutine(Shoot(shotCount, shotInterval));
        yield return new WaitForSeconds(postShootPause);

        // Idle for exactly as long as King's dash takes
        float kingDashTotal = dashToMiddleDuration + dashMiddlePause + dashBackDuration + postShootPause;
        yield return new WaitForSeconds(kingDashTotal);
    }

    // ── Movement coroutines ───────────────────────────────────────────────────

    IEnumerator DashMiddleAndBack()
    {
        yield return StartCoroutine(Dash(transform.position, middle.position, dashToMiddleDuration));
        yield return new WaitForSeconds(dashMiddlePause);
        yield return StartCoroutine(Dash(transform.position, _mySide.position, dashBackDuration));
    }

    IEnumerator ArcJump(Vector3 from, Vector3 to, float height, float duration)
    {
        float elapsed = 0f;
        Vector3 peak = Vector3.Lerp(from, to, 0.5f) + Vector3.up * height;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 a = Vector3.Slerp(from, peak, t);
            Vector3 b = Vector3.Slerp(peak, to, t);

            transform.position = Vector3.Lerp(a, b, t);
            yield return null;
        }

        transform.position = to;
    }

    IEnumerator Dash(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;
    }

    IEnumerator Shoot(int count, float interval)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{gameObject.name} missing projectilePrefab or firePoint");
            yield break;
        }

        float moveDirection = (_mySide == leftSide) ? 1f : -1f;

        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            bullet.transform.localScale = new Vector3(
                Mathf.Abs(bullet.transform.localScale.x) * moveDirection,
                bullet.transform.localScale.y,
                bullet.transform.localScale.z
            );

            Collider bulletCol = bullet.GetComponent<Collider>();
            if (bulletCol != null)
            {
                foreach (Collider col in FindObjectsOfType<Collider>())
                {
                    if (col.CompareTag("Enemy"))
                        Physics.IgnoreCollision(bulletCol, col, true);
                }
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = new Vector3(moveDirection * launchSpeed, rb.velocity.y, rb.velocity.z);

            yield return new WaitForSeconds(interval);
        }
    }
}