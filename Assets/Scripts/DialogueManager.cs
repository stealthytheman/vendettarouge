using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;
using System;

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

    public GameObject nameBox;
    public GameObject textBox;

    private Dictionary<string, DialogueEntry> dialogueMap;
    private DialogueEntry currentEntry;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;

    private bool dialogueActive = false;

    public GameObject dialogueBox;
    public GameObject nameBox2;
    public GameObject textName;
    public GameObject textText;
    public GameObject portrait2;

    // New event that fires when dialogue finishes completely
    public event Action OnDialogueComplete;

    void Start()
    {
        dialogueBox.SetActive(false);
        nameBox2.SetActive(false);
        textName.SetActive(false);
        textText.SetActive(false);
        portrait2.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive) return;

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

    public void LoadDialogue(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        string json = File.ReadAllText(path);
        DialogueDataWrapper dataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(json);

        dialogueMap = new Dictionary<string, DialogueEntry>();
        foreach (var entry in dataWrapper.dialogue)
        {
            dialogueMap[entry.id] = entry;
        }

        dialogueBox.SetActive(true);
        nameBox2.SetActive(true);
        textName.SetActive(true);
        textText.SetActive(true);
        portrait2.SetActive(true);
    }

    public void ShowDialogue(string id)
    {
        if (!dialogueMap.ContainsKey(id)) return;

        dialogueActive = true;

        if (speakerText.transform.parent != null && !speakerText.transform.parent.gameObject.activeSelf)
        {
            speakerText.transform.parent.gameObject.SetActive(true);
        }

        if (nameBox != null) nameBox.SetActive(true);
        if (textBox != null) textBox.SetActive(true);

        currentEntry = dialogueMap[id];
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
            yield return new WaitForSeconds(0.08f);
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
            if (speakerText.transform.parent != null)
                speakerText.transform.parent.gameObject.SetActive(false);

            if (nameBox != null) nameBox.SetActive(false);
            if (textBox != null) textBox.SetActive(false);

            // Fire the dialogue complete event here
            OnDialogueComplete?.Invoke();
        }
    }

    void LoadPortrait(string portraitName)
    {
        if (portraitImage == null)
            return;

        if (string.IsNullOrEmpty(portraitName))
        {
            StartCoroutine(FadePortrait(null));
            return;
        }

        Sprite sprite = Resources.Load<Sprite>("Portraits/" + portraitName);
        if (sprite == null)
        {
            Debug.LogError("Portrait not found: " + portraitName);
        }

        StartCoroutine(FadePortrait(sprite));
    }

    public void StartDialogue(string id)
    {
        ShowDialogue(id);
    }

    IEnumerator FadePortrait(Sprite newSprite, float fadeDuration = 0.5f)
    {
        if (portraitImage == null) yield break;

        float elapsed = 0f;
        Color color = portraitImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            portraitImage.color = color;
            yield return null;
        }

        portraitImage.sprite = newSprite;

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            portraitImage.color = color;
            yield return null;
        }

        color.a = 1f;
        portraitImage.color = color;
    }
}
