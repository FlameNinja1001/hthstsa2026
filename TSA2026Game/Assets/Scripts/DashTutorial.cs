using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    public GameObject dashObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ControlScript controlScript = FindObjectOfType<ControlScript>();
        if (controlScript.isDashing)
        {
            dashObj.SetActive(false);            
        
        }
        else
        {
            dashObj.SetActive(true);            
        }
    }
}
