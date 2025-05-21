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
    public HoverToolTipHighlightArt hoverToolTipHighlightArt; // Assign in Inspector

    void Start()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete += OnDialogueFinished;
        }
    }

    void Update()
    {
        if (HoverToolTipHighlightArt.artClicked && HoverToolTipHighlightCalendar.flag1Active)
        {
            Debug.Log("Art clicked and flag0Active is true");
            // Reset the flags so it only triggers once
            //HoverToolTipHighlightArt.artClicked = false;
            HoverToolTipHighlightCalendar.flag1Active = false;

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

        // Start dialogue and wait for it to finish
        bool dialogueDone = false;
        System.Action handler = () => { dialogueDone = true; };
        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            dialogueManager.OnDialogueComplete += handler;
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);

            // Wait until dialogue is finished
            while (!dialogueDone)
                yield return null;

            dialogueManager.OnDialogueComplete -= handler;
        }
        else
        {
            Debug.LogWarning("Missing DialogueManager or dialogue ID!");
        }

        // Now flash hidetext
        if (room2InteractionManager != null)
            room2InteractionManager.StartHideTextFlash();

        currentSequence = null;
    }

    void OnDialogueFinished()
    {
        if (shouldTriggerPostDialogueSequence)
        {
            shouldTriggerPostDialogueSequence = false;


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

        footstepsPlaying = true;

        // Play thirdClip as overlay
        if (thirdClip != null)
        {
            audioSource.PlayOneShot(thirdClip);
        }

        // REMOVE or comment out this coroutine if you don't want firstClip to play again:
        // StartCoroutine(PlayFirstClipThenResetFlag());
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
