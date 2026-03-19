using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotWall : MonoBehaviour
{
    public GameObject collectable;
    bool hasSet = false;
    public bool isLast = false;
    public MonoBehaviour script;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLast)
        {
            return;
        }
        TMScript tMScript = FindObjectOfType<TMScript>(); 
        if (collectable == null)
        {
            if (!hasSet)
            {
                tMScript.StartDialogueSequence();
                hasSet = true;
                Destroy(gameObject);
                script.enabled = true;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {   
        TMScript tMScript = FindObjectOfType<TMScript>();      
        if (isLast)
        {
            if (collision.gameObject.CompareTag("PlayerTag"))
            {
                if (!hasSet)
                {
                    tMScript.StartDialogueSequence();
                    hasSet = true;
                    Destroy(gameObject);
                }
            }
        }        
    }
}
