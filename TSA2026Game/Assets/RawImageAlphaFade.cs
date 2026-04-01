using UnityEngine;
using UnityEngine.UI;

public class RawImageAlphaFade : MonoBehaviour
{
    public RawImage targetImage;

    [Header("Fade Settings")]
    public float fadeSpeed = 1f;

    [Header("Delay Settings")]
    public float delayAtEnds = 1f; // Time to wait at 0 and 1

    float alpha = 0f;
    bool fadingIn = true;

    float delayTimer = 0f;
    bool isDelaying = false;

    void Update()
    {
        if (targetImage == null) return;

        // Handle delay
        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayAtEnds)
            {
                delayTimer = 0f;
                isDelaying = false;
                fadingIn = !fadingIn; // switch direction AFTER delay
            }
            return;
        }

        // Fade logic
        if (fadingIn)
        {
            alpha += fadeSpeed * Time.deltaTime;
            if (alpha >= 1f)
            {
                alpha = 1f;
                isDelaying = true;
            }
        }
        else
        {
            alpha -= fadeSpeed * Time.deltaTime;
            if (alpha <= 0f)
            {
                alpha = 0f;
                isDelaying = true;
            }
        }

        // Apply alpha
        Color c = targetImage.color;
        c.a = alpha;
        targetImage.color = c;
    }
}