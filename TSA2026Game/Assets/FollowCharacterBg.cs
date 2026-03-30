using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacterBg : MonoBehaviour
{
    public Transform cam;
    public int bgNum;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // Fix 1: MeshRenderer, not meshRenderer
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, cam.position.y, transform.position.z); // Fix 2: player.position.z, not player.transform.z
        if (SceneLoadManager.checkpointSpawn == bgNum)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}