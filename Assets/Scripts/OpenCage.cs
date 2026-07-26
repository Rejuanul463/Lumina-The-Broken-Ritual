using System.Collections;
using UnityEngine;

public class OpenCage : MonoBehaviour
{
    private bool playerInside;
    public Transform door;
    private Quaternion doorRotation;
    
    [SerializeField] private TeamNPC npcFree;
    [SerializeField] private GameObject unlockButton;
    
    [SerializeField] private LookAtTarget lookAtTarget;
    private void Start()
    {
        doorRotation = Quaternion.Euler(0, -80, 0);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = true;
            unlockButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = false;
            unlockButton.SetActive(false);
        }
    }

    private void OnEnable()
    {
        PlayerInputHandler.OpenCageGate += cageOpen;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OpenCageGate -= cageOpen;
    }


    void cageOpen()
    {
        if (playerInside)
        {
            StartCoroutine(OpenAnimation());
            npcFree.isFree = true;
        }
    }

    IEnumerator OpenAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 90f, 0f);

        while (Quaternion.Angle(door.localRotation, targetRotation) > 0.1f)
        {
            door.localRotation = Quaternion.Slerp(
                door.localRotation,
                targetRotation,
                2f * Time.deltaTime
            );

            yield return null;
        }
        // Snap exactly to target at the end
        door.localRotation = targetRotation;
    }

    public void ShowUI()
    {
        lookAtTarget.UpdateTarget();
        UIManager.instance.companionUI.SetActive(true);
        UIManager.instance.AssignHero(npcFree);
    }
}
