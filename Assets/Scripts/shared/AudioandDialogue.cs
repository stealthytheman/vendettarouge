using UnityEngine;
using System.Collections;

public class AudioAndDialogue : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;  // Plays alongside firstClip at the end

    public DialogueManager dialogueManager;
    public string dialogueId;
    public string jsonFilePath;

    public bool footstepsPlaying = false;

    private Coroutine currentSequence;
    private bool shouldTriggerPostDialogueSequence = false;

    // Add this field at the top of AudioAndDialogue.cs
    public Room2InteractionManager room2InteractionManager; // Assign in Inspector

    void Start()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete += OnDialogueFinished;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartFullSequence();
        }
    }

    void StartFullSequence()
    {
        if (currentSequence != null)
            StopCoroutine(currentSequence);

        if (audioSource.isPlaying)
            audioSource.Stop();

        shouldTriggerPostDialogueSequence = true;
        currentSequence = StartCoroutine(PlayTwoClipsThenDialogue());
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

        // Play second clip
        audioSource.clip = secondClip;
        audioSource.Play();
        yield return new WaitForSeconds(secondClip.length);

        // Start dialogue
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

    void OnDialogueFinished()
    {
        if (shouldTriggerPostDialogueSequence)
        {
            shouldTriggerPostDialogueSequence = false;

            // Flash hidetext after first dialogue
            if (room2InteractionManager != null)
                room2InteractionManager.StartHideTextFlash();

            RunFootstepAndAudioSequence();
        }
    }

    void RunFootstepAndAudioSequence()
    {
        if (audioSource == null || firstClip == null)
        {
            Debug.LogWarning("Missing audioSource or firstClip");
            return;
        }

        // Set the flag like original P logic
        footstepsPlaying = true;

        // Play thirdClip as overlay
        if (thirdClip != null)
        {
            audioSource.PlayOneShot(thirdClip);
        }

        StartCoroutine(PlayFirstClipThenResetFlag());
    }

    IEnumerator PlayFirstClipThenResetFlag()
    {
        audioSource.clip = firstClip;
        audioSource.loop = false;
        audioSource.Play();
        yield return new WaitForSeconds(firstClip.length);

        footstepsPlaying = false;
    }

    void OnDestroy()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete -= OnDialogueFinished;
        }
    }
}
