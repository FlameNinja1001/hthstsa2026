using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOutTrap : MonoBehaviour
{
    public Vector3 initialPos;
    public Vector3 finalPos;
    public float moveDist;
    public float speed;
    public float waitDuration;

    public Vector3 targetPos;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        targetPos = initialPos;
        finalPos = new Vector3(transform.position.x,transform.position.y + moveDist,transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitDuration)
        {
            if (targetPos == initialPos)
            {
                targetPos = finalPos;
                timer = 0f;
            }
            else
            {
                targetPos = initialPos;
                timer = 0f;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
