using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverToolTipHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string description;
    public GameObject normalObject;
    public GameObject highlightObject;

    public DialogueManager dialogueManager;
    public RoomInteractionManager roomInteractionManager;
    public Room2InteractionManager room2InteractionManager;

    public string dialogueId;
    public string jsonFilePath;

    public int flagName;

    public AudioSource audioSource;
    public AudioClip windowAudioClip;        // First sound on click
    public AudioClip afterDialogueClip;      // Second sound after first dialogue
    public AudioClip followupStartClip;      // Third sound before follow-up dialogue starts

    public AudioSource secondaryLoopSource;
    public AudioClip secondaryLoopClip;  

    public GameObject objectToActivate;      // Object to enable after dialogue
    public string followupJsonFile = "afterHide.json";
    public string followupStartId = "start";

    private bool waitingForWindowJson = false;
    public bool windowClicked = false;

    public static bool flag0Active = false;

    void Start()
    {
        normalObject.SetActive(true);
        highlightObject.SetActive(false);

        if (dialogueManager == null)
            dialogueManager = FindAnyObjectByType<DialogueManager>();

        if (roomInteractionManager == null)
            roomInteractionManager = FindAnyObjectByType<RoomInteractionManager>();

        // Start the secondary loop
        if (secondaryLoopSource != null && secondaryLoopClip != null)
        {
            secondaryLoopSource.clip = secondaryLoopClip;
            secondaryLoopSource.loop = true;
            secondaryLoopSource.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.Instance.Show(description);
        normalObject.SetActive(false);
        highlightObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.Hide();
        normalObject.SetActive(true);
        highlightObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);

            if (jsonFilePath == "Dialogue/window" && HoverToolTipHighlightArt.artClicked)
            {
                windowClicked = true;

                if (audioSource != null && windowAudioClip != null)
                    audioSource.PlayOneShot(windowAudioClip);

                dialogueManager.OnDialogueComplete += HandleWindowDialogueComplete;
                waitingForWindowJson = true;

                // Always start the green flash when the window is clicked
                if (room2InteractionManager != null)
                {
                    Debug.Log("Starting green flash from OnPointerClick");
                    room2InteractionManager.StartGreenFlash();
                }
                else
                {
                    Debug.LogWarning("Room2InteractionManager is not assigned!");
                }
            }
        }
        else
        {
            Debug.LogWarning("DialogueManager or dialogueId not set!");
        }

        if (roomInteractionManager != null)
        {
            roomInteractionManager.MarkFlagDone(flagName);
        }
        else
        {
            Debug.LogError("RoomInteractionManager is null!");
        }

        if (flagName == 0)
        {
            HoverToolTipHighlight.flag0Active = true;
        }
    }

    private void HandleWindowDialogueComplete()
    {
        if (!waitingForWindowJson) return;

        dialogueManager.OnDialogueComplete -= HandleWindowDialogueComplete;
        waitingForWindowJson = false;

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        // Do NOT set hidetext active here! (It is flashed after the first dialogue.)

        StartCoroutine(PlayFollowupStartThenAfterClipAndDialogue());
    }

    private IEnumerator PlayFollowupStartThenAfterClipAndDialogue()
    {
        // Stop the secondary loop before playing the followup clip
        if (secondaryLoopSource != null && secondaryLoopSource.isPlaying)
        {
            secondaryLoopSource.Stop();
        }

        // Play followupStartClip first and wait for it to finish
        if (audioSource != null && followupStartClip != null)
        {
            audioSource.PlayOneShot(followupStartClip);
            yield return new WaitForSeconds(followupStartClip.length);
        }

        // Start playing afterDialogueClip (no wait)
        if (audioSource != null && afterDialogueClip != null)
        {
            audioSource.PlayOneShot(afterDialogueClip);
        }

        // Start loading and showing the follow-up dialogue
        if (!string.IsNullOrEmpty(followupJsonFile) && !string.IsNullOrEmpty(followupStartId))
        {
            Debug.Log("Subscribing to OnDialogueComplete for followup");
            dialogueManager.OnDialogueComplete += HandleFollowupComplete;
            dialogueManager.LoadDialogue(followupJsonFile);
            dialogueManager.ShowDialogue(followupStartId);
            Debug.Log("Showing followup dialogue: " + followupStartId);
        }
    }

    private void HandleFollowupComplete()
    {
        dialogueManager.OnDialogueComplete -= HandleFollowupComplete;

        if (room2InteractionManager != null)
        {
            Debug.Log("Starting green flash in Room2InteractionManager");
            room2InteractionManager.OnDialogueComplete();
        }
        else
        {
            Debug.LogWarning("Room2InteractionManager is not assigned!");
        }
    }
}
