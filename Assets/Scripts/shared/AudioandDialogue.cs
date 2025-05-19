using UnityEngine;
using System.Collections;

public class AudioAndDialogue : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;  // Plays alongside firstClip if PlayFirstClipOnly is called

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
        if (currentSequence != null)
            StopCoroutine(currentSequence);

        if (audioSource.isPlaying)
            audioSource.Stop();

        currentSequence = StartCoroutine(PlayTwoClipsThenDialogue());
    }

    void PlayFirstClipOnly()
    {
        if (currentSequence != null)
            StopCoroutine(currentSequence);

        if (audioSource.isPlaying)
            audioSource.Stop();

        // Play thirdClip alongside firstClip using PlayOneShot (non-looping)
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

        audioSource.loop = false;

        // Play first clip
        audioSource.clip = firstClip;
        audioSource.Play();
        yield return new WaitForSeconds(firstClip.length);

        // Play second clip (ensure not looping)
        audioSource.loop = false;
        audioSource.clip = secondClip;
        audioSource.Play();
        yield return new WaitForSeconds(secondClip.length);

        // Start dialogue after both clips
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

        audioSource.loop = false;
        audioSource.clip = firstClip;
        audioSource.Play();
        yield return new WaitForSeconds(firstClip.length);

        footstepsPlaying = false;
        currentSequence = null;
    }
}
