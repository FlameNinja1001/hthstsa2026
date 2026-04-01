using System.Collections;
using UnityEngine;

public class GeorgeAndGeorgeScript : MonoBehaviour
{
    [Header("Identity")]
    public bool isKing = false;

    [Header("Animation")]
    public Animator animator;

    [Header("Shooting Visual")]
    public GameObject gunObj;

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

        // Ensure gun starts hidden
        if (gunObj != null)
            gunObj.SetActive(false);

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

    IEnumerator KingLoop()
    {
        yield return StartCoroutine(Shoot(shotCount, shotInterval));
        yield return new WaitForSeconds(postShootPause);

        float washDashTotal = dashToMiddleDuration + dashMiddlePause + dashBackDuration + preSwapPause;
        yield return new WaitForSeconds(washDashTotal);

        Transform kingTarget = (_mySide == leftSide) ? rightSide : leftSide;
        yield return StartCoroutine(ArcJump(transform.position, kingTarget.position,
                                            swapJumpHeight, swapJumpDuration));
        _mySide = kingTarget;

        float washShootTotal = shotCount * shotInterval + postShootPause;
        yield return new WaitForSeconds(washShootTotal);

        yield return StartCoroutine(DashMiddleAndBack());
        yield return new WaitForSeconds(postShootPause);
    }

    IEnumerator WashingtonLoop()
    {
        float kingShootTotal = shotCount * shotInterval + postShootPause;
        yield return new WaitForSeconds(kingShootTotal);

        yield return StartCoroutine(DashMiddleAndBack());
        yield return new WaitForSeconds(preSwapPause);

        Transform washTarget = (_mySide == rightSide) ? leftSide : rightSide;
        yield return StartCoroutine(ArcJump(transform.position, washTarget.position,
                                            swapJumpHeight, swapJumpDuration));
        _mySide = washTarget;

        yield return StartCoroutine(Shoot(shotCount, shotInterval));
        yield return new WaitForSeconds(postShootPause);

        float kingDashTotal = dashToMiddleDuration + dashMiddlePause + dashBackDuration + postShootPause;
        yield return new WaitForSeconds(kingDashTotal);
    }

    // ── Movement ─────────────────────────────────────────

    IEnumerator DashMiddleAndBack()
    {
        SetDashing(true);

        yield return StartCoroutine(Dash(transform.position, middle.position, dashToMiddleDuration));
        yield return new WaitForSeconds(dashMiddlePause);
        yield return StartCoroutine(Dash(transform.position, _mySide.position, dashBackDuration));

        SetDashing(false);
    }

    IEnumerator ArcJump(Vector3 from, Vector3 to, float height, float duration)
    {
        SetJumping(true);

        float elapsed = 0f;
        Vector3 peak = Vector3.Lerp(from, to, 0.5f) + Vector3.up * height;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 a = Vector3.Slerp(from, peak, t);
            Vector3 b = Vector3.Slerp(peak, to, t);

            transform.position = Vector3.Lerp(a, b, t);

            FaceDirection(to - from);

            yield return null;
        }

        transform.position = to;

        SetJumping(false);
    }

    IEnumerator Dash(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        FaceDirection(to - from);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;
    }

    // ── Shooting ─────────────────────────────────────────

    IEnumerator Shoot(int count, float interval)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{gameObject.name} missing projectilePrefab or firePoint");
            yield break;
        }

        SetShooting(true);

        float moveDirection = (_mySide == leftSide) ? 1f : -1f;

        FaceDirection(new Vector3(moveDirection, 0, 0));

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

        SetShooting(false);
    }

    // ── Animation Helpers ─────────────────────────────────

    void SetShooting(bool value)
    {
        if (animator != null)
            animator.SetBool("IsShooting", value);

        if (gunObj != null)
            gunObj.SetActive(value);
    }

    void SetJumping(bool value)
    {
        if (animator != null)
            animator.SetBool("IsJumping", value);
    }

    void SetDashing(bool value)
    {
        if (animator != null)
            animator.SetBool("IsDashing", value);
    }

    // ── Facing Direction ─────────────────────────────────

    void FaceDirection(Vector3 direction)
    {
        if (direction.x == 0) return;

        float dir = Mathf.Sign(direction.x);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }
}