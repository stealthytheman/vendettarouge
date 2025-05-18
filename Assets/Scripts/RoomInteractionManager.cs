using UnityEngine;

public class RoomInteractionManager : MonoBehaviour
{
    private bool[] flags = new bool[4];
    public GameObject hidetext;
    public UnityEngine.UI.Image screenOverlay; // Assign in Inspector

    public HoverToolTipHighlight1 bedHighlight; // Assign in Inspector
    public GameObject floorImage; // Assign in Inspector
    public GameObject room; // Assign in Inspector

    public DialogueManager dialogueManager; // Assign in Inspector

    public AudioClip clip1; // Assign in Inspector
    public AudioClip clip2; // Assign in Inspector
    public AudioSource audioSource; // Assign in Inspector

    public AudioClip backgroundLoopClip; // Assign in Inspector
    public AudioSource backgroundLoopSource; // Assign in Inspector

    void Start()
    {
        if (backgroundLoopSource != null && backgroundLoopClip != null)
        {
            backgroundLoopSource.clip = backgroundLoopClip;
            backgroundLoopSource.loop = true;
            backgroundLoopSource.Play();
        }
    }

    public void MarkFlagDone(int identifier)
    {
        flags[identifier] = true;
        Debug.Log($"Flag {identifier} marked as done!");

        if (AllFlagsDone())
        {
            Debug.Log("All flags are done! Advancing room...");
            AdvanceRoom();
        }
    }

    private bool AllFlagsDone()
    {
        foreach (bool flag in flags)
        {
            if (!flag)
                return false;
        }
        return true;
    }

    private void AdvanceRoom()
    {
        if (hidetext != null)
        {
            StartCoroutine(FadeAndFlash(hidetext));
        }
    }

    private System.Collections.IEnumerator FadeAndFlash(GameObject obj)
    {
        yield return new WaitForSeconds(3f);

        SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>(true);
        obj.SetActive(true);

        // Fade in
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

        // Flash (blink)
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

        if (bedHighlight != null)
        {
            while (!bedHighlight.bedClicked)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("bedHighlight reference not set on RoomInteractionManager!");
        }

        // Fade to black
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

            // Pulse red
            int pulses = 4;
            float pulseDuration = 0.5f;
            for (int i = 0; i < pulses; i++)
            {
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(Color.black, Color.red, t / pulseDuration);
                    yield return null;
                }
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(Color.red, Color.black, t / pulseDuration);
                    yield return null;
                }
            }

            // Ensure black
            screenOverlay.color = Color.black;

            // --- Play audio clips before continuing ---
            if (audioSource != null)
            {
                // Stop background loop
                if (backgroundLoopSource != null && backgroundLoopSource.isPlaying)
                {
                    backgroundLoopSource.Stop();
                }

                if (clip1 != null)
                {
                    audioSource.clip = clip1;
                    audioSource.Play();
                    yield return new WaitForSeconds(clip1.length);
                }

                if (clip2 != null)
                {
                    audioSource.clip = clip2;
                    audioSource.Play();
                    yield return new WaitForSeconds(clip2.length);
                }
            }
            else
            {
                Debug.LogWarning("AudioSource is not assigned in RoomInteractionManager!");
            }

            yield return new WaitForSeconds(1f); // optional pause after audio

            // Move room
            if (room != null)
            {
                room.transform.position = new Vector3(-0.88867f, -0.72f, room.transform.position.z);
            }

            // Show image
            if (floorImage != null)
            {
                floorImage.SetActive(true);
            }

            // Disable overlay
            screenOverlay.gameObject.SetActive(false);

            foreach (var script in FindObjectsOfType<HoverToolTipHighlight>())
                script.enabled = false;
            foreach (var script in FindObjectsOfType<HoverToolTipHighlight1>())
                script.enabled = false;
        }
    }
}
