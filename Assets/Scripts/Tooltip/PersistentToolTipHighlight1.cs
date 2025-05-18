using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PersistentToolTipHighlight1 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string description;

    public DialogueManager dialogueManager;
    public RoomInteractionManager roomInteractionManager;
    public string dialogueId;
    public string jsonFilePath;

    public GameObject watch;
    public UnityEngine.UI.Image whiteOverlay; // Assign in Inspector

    void Start()
    {
        if (dialogueManager == null)
            dialogueManager = FindAnyObjectByType<DialogueManager>();
        if (roomInteractionManager == null)
            roomInteractionManager = FindAnyObjectByType<RoomInteractionManager>();

        if (watch != null)
            watch.SetActive(false);

        if (whiteOverlay != null)
        {
            whiteOverlay.gameObject.SetActive(true); // Keep it active!
            var c = whiteOverlay.color;
            c.a = 0f;
            whiteOverlay.color = c;
            // Do NOT set whiteOverlay.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.Instance.Show(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);
        }
        else
        {
            Debug.LogWarning("DialogueManager or dialogueId not set!");
        }

        if (whiteOverlay != null)
            StartCoroutine(WhiteFlash());
    }

    private System.Collections.IEnumerator WhiteFlash()
    {
        // Wait 3 seconds before starting the flash
        yield return new WaitForSeconds(3f);

        whiteOverlay.gameObject.SetActive(true);

        // Flash in
        float duration = 0.2f;
        float t = 0;
        Color c = whiteOverlay.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            whiteOverlay.color = c;
            yield return null;
        }
        c.a = 1f;
        whiteOverlay.color = c;

        SceneManager.LoadScene("minigame");
    }
}