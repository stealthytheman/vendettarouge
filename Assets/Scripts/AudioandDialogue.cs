using UnityEngine;
using System.Collections;

public class AudioAndDialogue : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;  // New clip to play alongside firstClip

    public DialogueManager dialogueManager;
    public string dialogueId;
    public string jsonFilePath;

    public bool footstepsPlaying = false;

    private Coroutine currentSequence;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayFullSequence();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayFirstClipOnly();
        }
    }

    void PlayFullSequence()
    {
        if (currentSequence != null) StopCoroutine(currentSequence);
        if (audioSource.isPlaying) audioSource.Stop();

        currentSequence = StartCoroutine(PlayTwoClipsThenDialogue());
    }

    void PlayFirstClipOnly()
    {
        if (currentSequence != null) StopCoroutine(currentSequence);
        if (audioSource.isPlaying) audioSource.Stop();

        // Play thirdClip alongside firstClip
        if (thirdClip != null)
        {
            audioSource.PlayOneShot(thirdClip);
        }

        currentSequence = StartCoroutine(PlayOnlyFirstClip());
    }

    IEnumerator PlayTwoClipsThenDialogue()
    {
        if (audioSource == null || firstClip == null || secondClip == null)
        {
            Debug.LogWarning("Missing audio setup!");
            yield break;
        }

        audioSource.clip = firstClip;
        audioSource.Play();
        yield return new WaitForSeconds(firstClip.length);

        audioSource.clip = secondClip;
        audioSource.Play();
        yield return new WaitForSeconds(secondClip.length);

        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);
        }
        else
        {
            Debug.LogWarning("Missing DialogueManager or dialogue ID!");
        }

        currentSequence = null;
    }

    IEnumerator PlayOnlyFirstClip()
    {
        if (audioSource == null || firstClip == null)
        {
            Debug.LogWarning("Missing audio setup!");
            yield break;
        }

        footstepsPlaying = true;
        audioSource.clip = firstClip;
        audioSource.Play();
        yield return new WaitForSeconds(firstClip.length);
        footstepsPlaying = false;

        currentSequence = null;
    }
}
