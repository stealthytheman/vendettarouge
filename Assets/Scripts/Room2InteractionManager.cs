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

    void Start()
    {
        blackBox.SetActive(false); // Hide the black box at the start 
    }
    void Update()
    {
        if (audioAndDialogueScript != null && audioAndDialogueScript.footstepsPlaying && !coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine(FadeAndFlash(hidetext));
        }
    }

    private System.Collections.IEnumerator FadeAndFlash(GameObject obj)
    {
        yield return new WaitForSeconds(3f);

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

        if (screenOverlay != null)
        {
            screenOverlay.gameObject.SetActive(true);
            Color overlayColor = Color.black;
            overlayColor.a = 0f;
            screenOverlay.color = overlayColor;

            float fadeDuration = 1f;
            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                screenOverlay.color = overlayColor;
                yield return null;
            }

            overlayColor.a = 1f;
            screenOverlay.color = overlayColor;

            Color darkOliveGreen = new Color(85f / 255f, 107f / 255f, 47f / 255f);
            int pulses = 4;
            float pulseDuration = 0.5f;

            for (int i = 0; i < pulses; i++)
            {
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(Color.black, darkOliveGreen, t / pulseDuration);
                    yield return null;
                }
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(darkOliveGreen, Color.black, t / pulseDuration);
                    yield return null;
                }
            }

            screenOverlay.color = Color.black;
            yield return new WaitForSeconds(1f);

            // Dialogue setup
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueComplete += OnDialogueComplete;
                dialogueManager.LoadDialogue("afterHide.json");
                dialogueManager.ShowDialogue("start"); 
                blackBox.SetActive(true); // Hide the black box         
            }
            else
            {
                Debug.LogWarning("DialogueManager not assigned!");
            }

            if (room != null)
            {
                room.transform.position = new Vector3(-0.88867f, -0.72f, room.transform.position.z);
            }

            if (floorImage != null)
            {
                floorImage.SetActive(true);
            }

            if (screenOverlay != null)
            {
                screenOverlay.gameObject.SetActive(false);  // Re-enable later during final fade
            }

            foreach (var script in FindObjectsOfType<HoverToolTipHighlight>())
            {
                script.enabled = false;
            }
        }
    }

    private void OnDialogueComplete()
    {
        dialogueManager.OnDialogueComplete -= OnDialogueComplete;

        // Start fade out and load scene
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
}
