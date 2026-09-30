using System;
using UnityEngine;

/// <summary>
/// Shows objective markers (e.g. a "go here" arrow) one at a time, in order.
/// Only the current marker in "targets" is active; UpdateTarget() hides it and shows the next one.
/// Used by OpenCage to advance the objective after the cage has been opened.
/// </summary>
public class LookAtTarget : MonoBehaviour
{
    // Ordered list of marker objects
    public GameObject[] targets;
    // Index of the marker currently shown
    private int ind;

    private void Start()
    {
        // Show only the first marker at the start
        ind = 0;
        targets[ind].SetActive(true);

    }

    // Hides the current marker and shows the next one (does nothing extra after the last marker).
    public void UpdateTarget()
    {
        targets[ind].SetActive(false);
        ind++;
        if(ind < targets.Length)
            targets[ind].SetActive(true);
    }
}
