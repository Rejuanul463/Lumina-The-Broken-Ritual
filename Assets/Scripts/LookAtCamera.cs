using UnityEngine;

/// <summary>
/// Billboard helper: keeps this object (e.g. a world-space health bar or icon)
/// turned toward the main camera. Finds the camera by the "MainCamera" tag.
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    public GameObject cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(cam.transform);
    }
}
