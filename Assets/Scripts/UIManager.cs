using System;
using UnityEngine;

/// <summary>
/// Simple singleton that gives other scripts access to shared UI (e.g. OpenCage opens the
/// companion screen through UIManager.instance.companionUI) and remembers which companion
/// was most recently freed.
/// </summary>
public class UIManager : MonoBehaviour
{
    // Global access point: UIManager.instance
    public static UIManager instance;
    // The companion most recently assigned (see AssignHero)
    public TeamNPC currentCompanion;

    // Root object of the companion information screen
    public GameObject companionUI;

    private void Awake()
    {
        // Singleton: keep the first instance, destroy any duplicate
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Remembers the given companion as the current one
    public void AssignHero(TeamNPC hero)
    {
        currentCompanion = hero;
    }

}
