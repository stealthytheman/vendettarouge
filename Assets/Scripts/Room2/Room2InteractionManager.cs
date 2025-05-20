using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Room2InteractionManager : MonoBehaviour
{
    public AudioAndDialogue audioAndDialogueScript;
    public DialogueManager dialogueManager;
    public GameObject hidetext;
    public Image screenOverlay;
    public GameObject floorImage;
    public GameObject room;                          

    private bool coroutineStarted = false;

    public GameObject blackBox;

    // Reference to your HoverToolTipHighlight for the window
    public HoverToolTipHighlight windowHighlight; // Assign in Inspector

    void Start()
    {
        blackBox.SetActive(false); // Hide the black box at the start 
    }
    


    
    public void OnDialogueComplete()
    {
        // This can be called externally to trigger fade out and scene load
        StartCoroutine(FadeOutAndLoadScene());
    }

    private System.Collections.IEnumerator FadeOutAndLoadScene()
    {
        if (screenOverlay != null)
        {
            screenOverlay.gameObject.SetActive(true);
            Color overlayColor = Color.black;
            overlayColor.a = 0f;
            screenOverlay.color = overlayColor;

            float fadeDuration = 1.2f;
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                screenOverlay.color = overlayColor;
                yield return null;
            }

            overlayColor.a = 1f;
            screenOverlay.color = overlayColor;
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("room2.5"); // Replace with your next scene name
    }


    public void StartHideTextFlash()
    {
        if (hidetext != null)
            StartCoroutine(HideTextFlashCoroutine(hidetext));
    }

    private System.Collections.IEnumerator HideTextFlashCoroutine(GameObject obj)
    {
        SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>(true);
        obj.SetActive(true);

        float duration = 1f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / duration);
            foreach (var sr in sprites)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = alpha;
                    sr.color = c;
                }
            }
            yield return null;
        }

        foreach (var sr in sprites)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }

        int flashes = 4;
        float flashDuration = 0.5f;
        for (int i = 0; i < flashes; i++)
        {
            foreach (var sr in sprites)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }
            }
            yield return new WaitForSeconds(flashDuration);

            foreach (var sr in sprites)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 1f;
                    sr.color = c;
                }
            }
            yield return new WaitForSeconds(flashDuration);
        }

        obj.SetActive(false);
    }

    public void StartGreenFlash()
    {
        Debug.Log("StartGreenFlash called");
        StartCoroutine(GreenFlashCoroutine());
    }

    private System.Collections.IEnumerator GreenFlashCoroutine()
    {
        Debug.Log("GreenFlashCoroutine started");
        if (screenOverlay != null)
        {
            screenOverlay.gameObject.SetActive(true);

            // Define dark olive green
            Color darkOliveGreen = new Color(85f / 255f, 107f / 255f, 47f / 255f, 1f);
            Color black = Color.black;

            // Fade in from transparent to black
            float fadeInDuration = 1.0f; // Slower fade in (was 0.5f)
            float t = 0;
            Color transparent = new Color(0, 0, 0, 0);
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                screenOverlay.color = Color.Lerp(transparent, black, t / fadeInDuration);
                yield return null;
            }
            screenOverlay.color = black;

            int pulses = 4;
            float pulseDuration = 0.6f; // Slower flash (was 0.3f)

            for (int i = 0; i < pulses; i++)
            {
                // Black to dark olive green
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(black, darkOliveGreen, t / pulseDuration);
                    yield return null;
                }
                // Dark olive green to black
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(darkOliveGreen, black, t / pulseDuration);
                    yield return null;
                }
            }

            // Hold black for 1 second
            screenOverlay.color = black;
            yield return new WaitForSeconds(1f);

            // Hide overlay
            screenOverlay.gameObject.SetActive(false);
        }
    }
}
