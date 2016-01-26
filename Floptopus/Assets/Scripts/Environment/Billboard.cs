using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour 
{
    public Camera camera;

    void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
    }
 
    void Update()
    {
        
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
    }
}
