using UnityEngine;

public class SpinnerCode : MonoBehaviour
{    
    public Transform pivotPoint; 
    public Vector3 axis = Vector3.up; 
    public float rotationSpeed = 50f; 

    void Update()
    {
        if (pivotPoint != null)
        {            
            transform.RotateAround(pivotPoint.position, axis, rotationSpeed * Time.deltaTime);
        }
    }
}
