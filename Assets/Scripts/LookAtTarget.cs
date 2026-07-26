using System;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public GameObject[] targets;
    private int ind;

    private void Start()
    {
        ind = 0;
        targets[ind].SetActive(true);
        
    }

    public void UpdateTarget()
    {
        targets[ind].SetActive(false);
        ind++;
        if(ind < targets.Length)
            targets[ind].SetActive(true);
    }
}
