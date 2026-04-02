using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeorgeManager : MonoBehaviour
{
    public MonoBehaviour script1;
    public MonoBehaviour script2;

    bool hasEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasEnabled)
        {
            script1.enabled = true;
            script2.enabled = true;
            hasEnabled = true;
        }
        else if (script1 == null && script2 == null)
        {
            Destroy(gameObject);
        }
    }
}
