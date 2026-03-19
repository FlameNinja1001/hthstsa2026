using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkScript : MonoBehaviour
{
    public GameObject flashObj;
    private float timer;
    public float blinkDuration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkDuration)
        {
            flashObj.SetActive(!flashObj.activeSelf);
            timer = 0f;
        }
    }
}
