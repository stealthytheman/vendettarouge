using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayDialogueRoom3 : MonoBehaviour
{
    public DialogueManager dialogueManager;      
    public Image screenOverlay;         

    public string firstDialogueFileName = "intro.json";
    public string firstStartID = "start";

    public string secondDialogueFileName = "followup.json";
    public string secondStartID = "next";

    public string nextSceneName = "NextScene";

    public AudioClip introAudioClip;
    public AudioClip endAudioClip; 
    public GameObject blackBox; 

    private AudioSource audioSource;
    private bool playedFirstDialogue = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // 🔁 Loop intro audio clip
        if (introAudioClip != null)
        {
            audioSource.clip = introAudioClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartCoroutine(BeginDialogueSequence());
    }

    private IEnumerator BeginDialogueSequence()
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
            playedFirstDialogue = false;
            dialogueManager.OnDialogueComplete += OnDialogueComplete;
            dialogueManager.LoadDialogue(firstDialogueFileName);
            dialogueManager.ShowDialogue(firstStartID);
        }
        else
        {
            Debug.LogWarning("DialogueManager not assigned on PlayDialogueRoom3!");
        }
    }

    private void OnDialogueComplete()
    {
        dialogueManager.OnDialogueComplete -= OnDialogueComplete;

        if (!playedFirstDialogue)
        {
            playedFirstDialogue = true;

            // 🛑 Stop looping intro audio
            if (audioSource.isPlaying && audioSource.clip == introAudioClip)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }

            if (blackBox != null)
                blackBox.SetActive(true);

            StartCoroutine(PlayAudioThenContinue());
        }
        else
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator PlayAudioThenContinue()
    {
        if (endAudioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(endAudioClip);
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        dialogueManager.OnDialogueComplete += OnDialogueComplete;
        dialogueManager.LoadDialogue(secondDialogueFileName);
        dialogueManager.ShowDialogue(secondStartID);
    }

    private IEnumerator FadeOutAndLoadScene()
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
