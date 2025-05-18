using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;      // Assign in Inspector
    public Image screenOverlay;                  // Assign in Inspector
    public string dialogueFileName = "intro.json";
    public string startID = "start";
    public string nextSceneName = "NextScene";

    public AudioClip endAudioClip;               // Assign in Inspector
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(BeginDialogueSequence());
    }

    private System.Collections.IEnumerator BeginDialogueSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (screenOverlay != null)
        {
            screenOverlay.gameObject.SetActive(true);
            Color c = screenOverlay.color;
            c.a = 1f;
            screenOverlay.color = c;

            float fadeInDuration = 1f;
            float t = 0;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, t / fadeInDuration);
                screenOverlay.color = c;
                yield return null;
            }

            screenOverlay.color = new Color(c.r, c.g, c.b, 0f);
        }

        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete += OnDialogueComplete;
            dialogueManager.LoadDialogue(dialogueFileName);
            dialogueManager.ShowDialogue(startID);
        }
        else
        {
            Debug.LogWarning("DialogueManager not assigned on PlayDialogue!");
        }
    }

    private void OnDialogueComplete()
    {
        dialogueManager.OnDialogueComplete -= OnDialogueComplete;

        // 🔊 Play audio clip immediately when dialogue ends
        if (endAudioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(endAudioClip);
        }

        StartCoroutine(FadeOutAndLoadScene());
    }

    private System.Collections.IEnumerator FadeOutAndLoadScene()
    {
        
        yield return new WaitForSeconds(1f);

        if (screenOverlay != null)
        {
            screenOverlay.gameObject.SetActive(true);
            Color c = screenOverlay.color;
            c.a = 0f;
            screenOverlay.color = c;

            float fadeOutDuration = 1.2f;
            float t = 0;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeOutDuration);
                screenOverlay.color = c;
                yield return null;
            }

            screenOverlay.color = new Color(c.r, c.g, c.b, 1f);
        }

        yield return new WaitForSeconds(0.5f);
        GameFlags.cameFromRoom = 2;
        SceneManager.LoadScene(nextSceneName);
    }
}
