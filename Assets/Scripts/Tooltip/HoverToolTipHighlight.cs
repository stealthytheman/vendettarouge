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

    public GameObject objectToActivate;      // Object to enable after dialogue
    public string followupJsonFile = "afterHide.json";
    public string followupStartId = "start";

    private bool waitingForWindowJson = false;

    void Start()
    {
        normalObject.SetActive(true);
        highlightObject.SetActive(false);

        if (dialogueManager == null)
            dialogueManager = FindAnyObjectByType<DialogueManager>();

        if (roomInteractionManager == null)
            roomInteractionManager = FindAnyObjectByType<RoomInteractionManager>();
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

            if (jsonFilePath == "window.json")
            {
                if (audioSource != null && windowAudioClip != null)
                    audioSource.PlayOneShot(windowAudioClip);

                dialogueManager.OnDialogueComplete += HandleWindowDialogueComplete;
                waitingForWindowJson = true;
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
    }

    private void HandleWindowDialogueComplete()
    {
        if (!waitingForWindowJson) return;

        dialogueManager.OnDialogueComplete -= HandleWindowDialogueComplete;
        waitingForWindowJson = false;

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        StartCoroutine(PlayFollowupStartThenAfterClipAndDialogue());
    }

    private IEnumerator PlayFollowupStartThenAfterClipAndDialogue()
    {
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

        // Start loading and showing the follow-up dialogue at the same time as afterDialogueClip
        if (!string.IsNullOrEmpty(followupJsonFile) && !string.IsNullOrEmpty(followupStartId))
        {
            dialogueManager.OnDialogueComplete += HandleFollowupComplete;
            dialogueManager.LoadDialogue(followupJsonFile);
            dialogueManager.ShowDialogue(followupStartId);
        }
    }

    private void HandleFollowupComplete()
    {
        dialogueManager.OnDialogueComplete -= HandleFollowupComplete;

        if (room2InteractionManager != null)
        {
            room2InteractionManager.OnDialogueComplete();
        }
        else
        {
            Debug.LogWarning("Room2InteractionManager is not assigned!");
        }
    }
}
