using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeHandler : MonoBehaviour
{
    public HoverToolTipHighlightArt artScript;
    public Image fadeImage;
    public DialogueManager dialogueManager;
    public string dialogueId;
    public string jsonFilePath;

    public float fadeDuration = 1f;
    public float holdTime = 2f;

    private bool hasFaded = false;

    void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (artScript != null && HoverToolTipHighlightArt.artClicked && !hasFaded)
        {
            hasFaded = true;
            StartCoroutine(FadeSequence());
        }
    }

    IEnumerator FadeSequence()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(holdTime);
        yield return StartCoroutine(Fade(1f, 0f));

        if (dialogueManager != null && !string.IsNullOrEmpty(dialogueId))
        {
            Debug.Log("Loading dialogue...");
            dialogueManager.LoadDialogue(jsonFilePath);
            dialogueManager.ShowDialogue(dialogueId);
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
