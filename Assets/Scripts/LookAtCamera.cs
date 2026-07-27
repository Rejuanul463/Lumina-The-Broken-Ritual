using UnityEngine;

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
