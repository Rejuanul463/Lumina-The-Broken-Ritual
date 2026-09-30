using System.Collections;
using UnityEngine;

/// <summary>
/// A cage that holds a companion NPC. Put this on a trigger collider around the cage.
/// Flow:
///  1. Player walks into the trigger -> the "unlock" button is shown.
///  2. Player presses unlock -> PlayerInputHandler.OpenCageGate event fires -> cageOpen().
///  3. The door swings open and the NPC is marked free (TeamNPC.isFree).
///  4. When the NPC reaches the player, TeamNPC calls ShowUI() to show the companion info screen.
/// </summary>
public class OpenCage : MonoBehaviour
{
    // True while the player is standing in the trigger area
    private bool playerInside;
    // The door object that will be rotated open
    public Transform door;
    private Quaternion doorRotation;

    // The NPC that is locked inside this cage
    [SerializeField] private TeamNPC npcFree;
    // UI button that appears when the player is close enough to unlock
    [SerializeField] private GameObject unlockButton;

    // Objective marker controller, advanced once the cage is opened
    [SerializeField] private LookAtTarget lookAtTarget;
    private void Start()
    {
        // NOTE: unused - the actual open rotation is defined in OpenAnimation()
        doorRotation = Quaternion.Euler(0, -80, 0);

    }

    // Player entered the cage area: allow unlocking and show the button
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = true;
            unlockButton.SetActive(true);
        }
    }

    // Player left the cage area: hide the button again
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInside = false;
            unlockButton.SetActive(false);
        }
    }

    // Subscribe/unsubscribe to the static unlock event so the cage only listens while enabled
    private void OnEnable()
    {
        PlayerInputHandler.OpenCageGate += cageOpen;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OpenCageGate -= cageOpen;
    }


    // Called when the unlock button is pressed. Only works if the player is inside the trigger.
    void cageOpen()
    {
        if (playerInside)
        {
            StartCoroutine(OpenAnimation());
            // Tells the NPC it may leave the cage and follow the player
            npcFree.isFree = true;
        }
    }

    // Smoothly rotates the door to 90 degrees around Y over several frames
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

    // Called by TeamNPC once it has walked out to the player.
    // Advances the objective marker and opens the companion info UI for this NPC.
    public void ShowUI()
    {
        lookAtTarget.UpdateTarget();
        UIManager.instance.companionUI.SetActive(true);
        UIManager.instance.AssignHero(npcFree);
    }
}
