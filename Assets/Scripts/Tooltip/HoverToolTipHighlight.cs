using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToolTipHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string description;
    public GameObject normalObject;
    public GameObject highlightObject;

    public DialogueManager dialogueManager;
    public RoomInteractionManager roomInteractionManager;
    public string dialogueId;
    public string jsonFilePath;

    public int flagName;

    void Start()
    {
        normalObject.SetActive(true);
        highlightObject.SetActive(false);

        if (dialogueManager == null)
        {
            dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        if (roomInteractionManager == null)
        {
            roomInteractionManager = FindAnyObjectByType<RoomInteractionManager>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse Entered");
        ToolTip.Instance.Show(description);

        normalObject.SetActive(false);
        highlightObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse Exited");
        ToolTip.Instance.Hide();

        normalObject.SetActive(true);
        highlightObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked! Calling DialogueManager.ShowDialogue with id: " + dialogueId);
        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);
        }
        else
        {
            Debug.LogWarning("DialogueManager or dialogueId not set!");
        }

        if (roomInteractionManager == null)
        {
            Debug.LogError("RoomInteractionManager is null!");
        }
        else
        {
            roomInteractionManager.MarkFlagDone(flagName);
        }
    
    }
}
