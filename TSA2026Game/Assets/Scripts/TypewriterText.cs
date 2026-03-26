using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterText : MonoBehaviour
{
    [TextArea] public string inputText;
    public float startDelay = 0.5f;
    public float typingSpeed = 0.05f;

    private TMP_Text textDisplay;
    private Coroutine typingCoroutine;

    public bool isTyping { get; private set; }   // 🔹 ADD THIS

    void Start()
    {
        textDisplay = GetComponent<TMP_Text>();
        StartTyping();
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(inputText));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;   // 🔹 START typing

        textDisplay.text = "";
        yield return new WaitForSeconds(startDelay);

        foreach (char c in fullText)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;  // 🔹 DONE typing
    }
}