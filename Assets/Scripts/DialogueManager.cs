using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;
using System;

/*
    * DialogueManager.cs
    * Written by: Luke Baxter (2025)
    * Written for: Unity 6.1
    * Date: 2025-05-17
    * This script was written to provide a modular way to handle dialogue for VN's in Unity.
    * It loads dialogue data stored in a JSON file.
    * ------------------------------------------------
    * Things to note:*
    * - Dialogue is displayed using a TextMeshProUGUI component.
    * - Portraits are loaded from the Resources folder.
    * - In order to load the dialogue, call LoadDialogue(*jsonFilePath*).
    * - In order to show the dialogue, call ShowDialogue(*dialogueId*).
    * IMPORTANT NOTE: THe JSON file must retain the same structure as the DialogueDataWrapper (DialogueEntry) class.
    *-------------------------------------------------

*/
public class DialogueManager : MonoBehaviour
{   
    [System.Serializable]
    public class DialogueEntry
    {
        public string id;
        public string speaker;
        public string portrait;
        public string text;
        public string next;
    }

    [System.Serializable]
    public class DialogueDataWrapper
    {
        public DialogueEntry[] dialogue;
    }

    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    private Dictionary<string, DialogueEntry> dialogueMap;
    private DialogueEntry currentEntry;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;

    private bool dialogueActive = false;

    void Start()
    {
        LoadDialogue("dialogue.json");
        StartDialogue("start"); // start the dialogue with the ID "start"
    }

    void Update()
    {
        // check for input only if the dialogue is active
        if (!dialogueActive)
        {
            return;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                OnNext();
            }
        }
    }

    void LoadDialogue(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        string json = File.ReadAllText(path);
        DialogueDataWrapper dataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(json);

        dialogueMap = new Dictionary<string, DialogueEntry>();
        foreach (var entry in dataWrapper.dialogue)
        {
            dialogueMap[entry.id] = entry;
        }
    }

    public void ShowDialogue(string id)
    {
        if (!dialogueMap.ContainsKey(id))
        {
            return;
        }

        dialogueActive = true;

        currentEntry = dialogueMap[id];
        speakerText.transform.parent.gameObject.SetActive(true); // show the text
        speakerText.text = currentEntry.speaker;
        LoadPortrait(currentEntry.portrait);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(currentEntry.text));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char c in line)
        {
            if (skipTyping)
            {
                dialogueText.text = line;
                break;
            }

            dialogueText.text += c;
            yield return new WaitForSeconds(0.08f); // typing speed
        }

        isTyping = false;
    }

    public void OnNext()
    {
        if (currentEntry != null && !string.IsNullOrEmpty(currentEntry.next))
        {
            ShowDialogue(currentEntry.next);
        }
        else
        {
            dialogueActive = false;
            speakerText.transform.parent.gameObject.SetActive(false); // hide the text
        }
    }

    void LoadPortrait(string portraitName)
    {
        if (portraitImage == null || string.IsNullOrEmpty(portraitName))
        {
            return;
        }

        // If there is no portrait declared in the JSON, hide the image
        if (string.IsNullOrEmpty(portraitName))
        {
            StartCoroutine(FadePortrait(null));
            return;
        }

        Sprite sprite = Resources.Load<Sprite>("Portraits/" + portraitName);
        if(sprite == null)
        {
            Debug.LogError("Portrait not found: " + portraitName);
        }

        StartCoroutine(FadePortrait(sprite));
    }

    // call this method from anywhere where the dialogue algorithm should be summoned
    public void StartDialogue(string id)
    {
        ShowDialogue(id);
    }

    IEnumerator FadePortrait(Sprite newSprite, float fadeDuration = 0.5f)
    {
        if (portraitImage == null) yield break;

        // fade out
        float elapsed = 0f;
        Color color = portraitImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            portraitImage.color = color;
            yield return null;
        }

        // switch sprite
        portraitImage.sprite = newSprite;

        // fade in
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            portraitImage.color = color;
            yield return null;
        }

        // ensure alpha is 1 at the end
        color.a = 1f;
        portraitImage.color = color;
    }
}

