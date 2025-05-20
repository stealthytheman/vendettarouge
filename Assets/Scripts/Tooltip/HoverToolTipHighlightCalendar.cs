using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverToolTipHighlightCalendar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    public AudioClip afterDialogueClip;
    public AudioClip followupStartClip;

    public AudioSource secondaryLoopSource;
    public AudioClip secondaryLoopClip;

    public GameObject objectToActivate;

    private bool waitingForCalendarJson = false;

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
            
            waitingForCalendarJson = true;
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
}
