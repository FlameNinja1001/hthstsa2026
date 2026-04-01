using System.Collections;
using UnityEngine;

public class BirdBossManager : MonoBehaviour
{
    [Header("Boss")]
    public GameObject bossObject;

    [Header("Dead Object")]
    public GameObject deadObject;
    public float deadObjectMaxTime = 5f;
    public float flickerSpeed = 0.1f;

    [Header("Wire")]
    public Animator wireAnimator;
    public GameObject[] wireColliders;

    // ── internal state ──────────────────────────────────────────
    private bool _bossDeathHandled = false;
    private bool _wireSnapped = false;
    private bool _deadTimerRunning = false;
    private float _deadTimer = 0f;

    private Vector3 _originalScale;

    // ────────────────────────────────────────────────────────────
    void Start()
    {
        if (deadObject != null)
            _originalScale = deadObject.transform.localScale;
    }

    void Update()
    {
        // 1. Watch for boss destruction
        if (!_bossDeathHandled && bossObject == null)
        {
            _bossDeathHandled = true;
            OnBossDied();
        }

        // 2. Watch for wire snap → disable wire colliders
        if (!_wireSnapped && wireAnimator != null &&
            wireAnimator.GetBool("hasSnapped"))
        {
            _wireSnapped = true;
            DisableWireColliders();
        }

        // 3. Dead-object lifetime timer
        if (_deadTimerRunning && deadObject != null && deadObject.activeSelf)
        {
            _deadTimer += Time.deltaTime;

            if (_deadTimer >= deadObjectMaxTime)
            {
                _deadTimerRunning = false;
                StartCoroutine(FlickerThenDisappear());
            }
        }
    }

    // ── Trigger: BirdTag enters this collider ────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BirdTag") && wireAnimator != null)
        {
            wireAnimator.SetBool("hasSnapped", true);
        }
    }

    // ── Called once when boss is destroyed ──────────────────────
    void OnBossDied()
    {
        if (deadObject != null)
        {
            deadObject.SetActive(true);
            deadObject.transform.localScale = _originalScale;

            _deadTimer = 0f;
            _deadTimerRunning = true;
        }
    }

    // ── Disable every wire collider GameObject ───────────────────
    void DisableWireColliders()
    {
        if (wireColliders == null) return;
        foreach (GameObject col in wireColliders)
        {
            if (col != null)
                col.SetActive(false);
        }
    }

    // ── Flicker coroutine (scale instead of active) ──────────────
    IEnumerator FlickerThenDisappear()
    {
        float flickerDuration = 1.5f;
        float elapsed = 0f;

        bool visible = true;

        while (elapsed < flickerDuration)
        {
            visible = !visible;

            if (visible)
                deadObject.transform.localScale = _originalScale;
            else
                deadObject.transform.localScale = Vector3.zero;

            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        // Final state: hidden
        deadObject.transform.localScale = Vector3.zero;
    }
}