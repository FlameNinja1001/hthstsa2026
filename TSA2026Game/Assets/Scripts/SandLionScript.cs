using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandLionScript : MonoBehaviour
{
    public bool isLeft;
    public float xSpeed;    
    public GameObject mesh;

    public float bounceHeight = 3f;   // Max height
    public float bounceSpeed = 2f;    // How fast it bounces

    float startY;
    float timer;
    public float initialScale;
    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        initialScale = mesh.transform.localScale.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        timer += Time.deltaTime * bounceSpeed;

        float yOffset = Mathf.Abs(Mathf.Sin(timer)) * bounceHeight;
      
        if (!isLeft)
        {
            mesh.transform.localScale = new Vector3(-initialScale,mesh.transform.localScale.y,mesh.transform.localScale.z);
        }    
        else
        {
            mesh.transform.localScale = new Vector3(initialScale,mesh.transform.localScale.y,mesh.transform.localScale.z);
        }    
        transform.position = new Vector3(
            transform.position.x + (isLeft ? -xSpeed : xSpeed) * Time.deltaTime,
            startY + yOffset,
            transform.position.z
        );
    }

    
    
        
}
