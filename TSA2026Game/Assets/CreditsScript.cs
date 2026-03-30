using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public RectTransform moveObject;
    public Vector2 startPos;
    public Vector2 endPos;
    public float duration = 1f;

    public AudioSource audioSource;
    public float fadeDuration = 1f;

    float timer = 0f;
    float fadeTimer = 0f;

    bool moveDone = false;

    void Update()
    {
        // MOVE FIRST
        if (!moveDone)
        {
            if (timer < duration)
            {
                float t = Mathf.Clamp01(timer / duration);
                moveObject.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                timer += Time.deltaTime;
            }
            else
            {
                moveDone = true;
            }
        }
        // THEN FADE AUDIO
        else
        {
            if (fadeTimer < fadeDuration)
            {
                float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                audioSource.volume = Mathf.Lerp(1f, 0f, t);
                fadeTimer += Time.deltaTime;
            }
        }
    }
}