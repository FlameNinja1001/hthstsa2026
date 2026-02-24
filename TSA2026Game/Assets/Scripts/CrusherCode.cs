using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherCode : MonoBehaviour
{
    public GameObject player;
    public float timeDown = 2f;
    public float timeUp = 10f;
    public Transform posUp;
    public Transform posDown;
    public bool canCrush = true;
    public float distanceThreshold = 5f;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("PlayerTag");
    }

    // Update is called once per frame
    void Update()
    {
        distance = Mathf.Abs(transform.position.x - player.transform.position.x);
        if (distance < distanceThreshold && canCrush)
        {
            canCrush = false;
            StartCoroutine(CrushCoroutine());
        }
    }

    public IEnumerator CrushCoroutine()
    {
        float timer = 0;

        while (timer < timeDown)
        {
            timer += Time.deltaTime;
            float t = timer / timeDown; 
            transform.position = Vector3.Lerp(posUp.position, posDown.position, t);
            yield return null;
        }
        timer = 0;

        while (timer < timeUp)
        {
            timer += Time.deltaTime;
            float t = timer / timeUp; 
            transform.position = Vector3.Lerp(posDown.position, posUp.position, t);
            yield return null;
        }
        canCrush = true;
        yield return null;
    }
}
