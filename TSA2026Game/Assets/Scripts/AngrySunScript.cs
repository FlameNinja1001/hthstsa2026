using UnityEngine;

public class AngrySunScript : MonoBehaviour
{
    public Transform cameraPos;
    public bool isLeft;
    public float offsetUp;
    public float offsetSide;

    public bool isActive;
    private bool wasActive = false;

    // Rotation
    Vector3 rotationCenter;
    float rotationRadius = 2f;
    float angularSpeed = 11f;
    float posX, posY;
    float angle;

    // Sweep
    private float journeyTime = 2.5f;
    private float startTime;

    // State
    public string stateString = "StandStill";
    public float timer = 0f;
    public float currentDelay;
    public float standStillDelay;
    public float rotatingDelay;

    // Player / sweep
    public GameObject player;
    public Vector3 playerPos;
    public float sweepArc;

    // Fade
    public SpriteRenderer sunSprite;
    public float fadeSpeed = 2f;
    private float fadeTimer = 0f;

    private enum FadeState { Idle, FadingIn, FadingOut }
    private FadeState fadeState = FadeState.Idle;

    // Collider
    private SphereCollider sphereCollider;

    // Attack loop
    private bool attacking = false;

    // ✅ Store position for fade-out
    private Vector3 fadeOutStartPos;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null) sphereCollider.enabled = false;

        SetAlpha(0f);
    }

    void Update()
    {
        bool justActivated   = isActive  && !wasActive;
        bool justDeactivated = !isActive && wasActive;
        wasActive = isActive;

        if (justActivated)
        {
            // Spawn directly in visible position
            transform.position = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            SetAlpha(0f);

            fadeState = FadeState.FadingIn;
            fadeTimer = 0f;

            attacking = false;

            stateString = "StandStill";
            timer = 0f;
            angle = 0f;
            currentDelay = standStillDelay;
            startTime = Time.time;
            playerPos = player.transform.position;
        }

        if (justDeactivated)
        {
            fadeState = FadeState.FadingOut;
            fadeTimer = 0f;

            // ✅ Lock current position
            fadeOutStartPos = transform.position;

            attacking = false;

            if (sphereCollider != null)
                sphereCollider.enabled = false;
        }

        // ───── FADE IN ─────
        if (fadeState == FadeState.FadingIn)
        {
            fadeTimer += Time.deltaTime;

            float duration = 1f / fadeSpeed;
            float t = Mathf.Clamp01(fadeTimer / duration);

            float a = t * t * (3f - 2f * t); // smoothstep
            SetAlpha(a);

            // Follow camera while fading in
            transform.position = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            if (a >= 1f)
            {
                fadeState = FadeState.Idle;
                attacking = true;
                fadeTimer = 0f;

                if (sphereCollider != null)
                    sphereCollider.enabled = true;
            }
            return;
        }

        // ───── FADE OUT ─────
        if (fadeState == FadeState.FadingOut)
        {
            fadeTimer += Time.deltaTime;

            float duration = 1f / fadeSpeed;
            float t = Mathf.Clamp01(fadeTimer / duration);

            float a = 1f - (t * t * (3f - 2f * t));
            SetAlpha(a);

            // ✅ Stay exactly where fade started (no snapping)
            transform.position = fadeOutStartPos;

            if (a <= 0f)
            {
                fadeState = FadeState.Idle;
                fadeTimer = 0f;

                stateString = "StandStill";
                timer = 0f;
                angle = 0f;
                currentDelay = standStillDelay;
            }
            return;
        }

        // Not attacking
        if (!attacking)
            return;

        // ── Attack loop ──

        sweepArc = Mathf.Clamp((cameraPos.position.y - playerPos.y) / 2f, 0.1f, 1f);
        float tArc = Mathf.InverseLerp(0.1f, 1f, sweepArc);
        sweepArc = Mathf.Lerp(10f, 1f, tArc);

        timer += Time.deltaTime;

        if (stateString != "Sweep" && timer >= currentDelay)
            SwitchState();
    }

    void LateUpdate()
    {
        if (!attacking) return;

        if (stateString == "StandStill")
        {
            transform.position = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );
            currentDelay = standStillDelay;
        }
        else if (stateString == "Rotating")
        {
            rotationCenter = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            posX = rotationCenter.x + Mathf.Cos(angle) * rotationRadius;
            posY = rotationCenter.y - Mathf.Sin(angle) * rotationRadius;
            transform.position = new Vector3(posX, posY, transform.position.z);

            angle += Time.deltaTime * angularSpeed;
            if (angle >= 360f) angle = 0f;

            currentDelay = rotatingDelay;
        }
        else if (stateString == "Sweep")
        {
            Vector3 sunrise = new Vector3(
                cameraPos.position.x + (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );
            Vector3 sunset = new Vector3(
                cameraPos.position.x - (isLeft ? -offsetSide : offsetSide),
                cameraPos.position.y + offsetUp,
                transform.position.z
            );

            Vector3 center = (sunrise + sunset) * 0.5f + new Vector3(0, sweepArc, 0);

            Vector3 riseRelCenter = sunrise - center;
            Vector3 setRelCenter  = sunset  - center;

            float fracComplete = Mathf.Clamp01((Time.time - startTime) / journeyTime);
            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete) + center;

            currentDelay = journeyTime;

            if (fracComplete >= 1f)
                SwitchState();
        }
    }

    void SwitchState()
    {
        playerPos = player.transform.position;

        if (stateString == "StandStill")
            stateString = "Rotating";
        else if (stateString == "Rotating")
            stateString = "Sweep";
        else
        {
            isLeft = !isLeft;
            stateString = "StandStill";
        }

        startTime = Time.time;
        timer = 0f;
    }

    // ── Helpers ──

    void SetAlpha(float a)
    {
        if (sunSprite == null) return;
        Color c = sunSprite.color;
        sunSprite.color = new Color(c.r, c.g, c.b, a);
    }

    float GetAlpha()
    {
        return sunSprite != null ? sunSprite.color.a : 0f;
    }
}