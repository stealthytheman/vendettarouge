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
        // Wait 3 seconds before starting the animation
        yield return new WaitForSeconds(3f);

        // Get all SpriteRenderers in this object and its children
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
        // Ensure fully visible
        foreach (var sr in sprites)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }

        // Flash (blink on/off)
        int flashes = 4;
        float flashDuration = 0.5f;
        for (int i = 0; i < flashes; i++)
        {
            // Hide
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

            // Show
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

        // Hide the object after animation
        obj.SetActive(false);

        // --- Wait until bedClicked is true ---
        if (bedHighlight != null)
        {
            while (!bedHighlight.bedClicked)
            {
                yield return null; // Wait until next frame
            }
        }
        else
        {
            Debug.LogWarning("bedHighlight reference not set on RoomInteractionManager!");
        }

        // --- Fade the screen to black ---
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

            // --- Pulse red ---
            int pulses = 4;
            float pulseDuration = 0.5f;
            for (int i = 0; i < pulses; i++)
            {
                // Fade to red
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(Color.black, Color.red, t / pulseDuration);
                    yield return null;
                }
                // Fade back to black
                t = 0;
                while (t < pulseDuration)
                {
                    t += Time.deltaTime;
                    screenOverlay.color = Color.Lerp(Color.red, Color.black, t / pulseDuration);
                    yield return null;
                }
            }
            // Ensure it's black at the end
            screenOverlay.color = Color.black;

            // --- Wait 5 seconds ---
            yield return new WaitForSeconds(5f);

            // --- Move room to center ---
            if (room != null)
            {
                room.transform.position = new Vector3(-0.88867f, -0.72f, room.transform.position.z);
            }

            // --- Show image on the floor ---
            if (floorImage != null)
            {
                floorImage.SetActive(true);
            }

            // --- Disable the black overlay ---
            if (screenOverlay != null)
            {
                screenOverlay.gameObject.SetActive(false);
            }

            foreach (var script in FindObjectsOfType<HoverToolTipHighlight>())
                script.enabled = false;
            foreach (var script in FindObjectsOfType<HoverToolTipHighlight1>())
                script.enabled = false;
        }
    }
}
