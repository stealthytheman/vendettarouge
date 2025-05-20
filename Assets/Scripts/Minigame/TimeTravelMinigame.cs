using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeTravelMinigame : MonoBehaviour
{
    public RectTransform flyingObject;
    public Sprite[] numberSprites;           // Assign your number sprites here
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI startText;
    public float speed = 300f;
    public float gameDuration = 20f;
    public int scoreToPass = 3;
    public string nextSceneName = "YourNextScene";

    private float timer;
    private int score;
    private Vector2 direction;
    private bool canScore = true;
    private bool objectActive = true;
    private Image flyingObjectImage;

    public SceneFader sceneFader; // Assign in Inspector

    // New: Looping audio source for sound to start on scene load
    public AudioSource loopingAudioSource;

    // Red flash UI elements
    public Image redFlashImage;   // Assign your fullscreen red UI Image here (alpha 0 initially)
    public float flashDuration = 0.2f;

    private Coroutine flashCoroutine;

    public ClockHands clockHands; // Assign in Inspector

    void Start()
    {
        // Adjust difficulty and next scene based on flag
        if (GameFlags.cameFromRoom == 1)
        {
            scoreToPass = 10;
            speed = 400f;
            nextSceneName = "bedroom";
        }
        else if (GameFlags.cameFromRoom == 2)
        {
            scoreToPass = 15;
            gameDuration = 25f;
            speed = 550f;
            nextSceneName = "room3";
        }

        timer = gameDuration;
        score = 0;
        clockHands.gameStarted = false;

        flyingObjectImage = flyingObject.GetComponent<Image>();

        // Show start prompt, hide game UI
        startText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        flyingObject.gameObject.SetActive(false);

        // Make sure red flash is fully transparent at start
        if (redFlashImage != null)
        {
            Color c = redFlashImage.color;
            redFlashImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void Update()
    {
        if (!clockHands.gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
            return;
        }

        // Update timer
        timer -= Time.deltaTime;
        timerText.text = $"Time: {timer:F1}";
        scoreText.text = $"Score: {score}/{scoreToPass}";

        if (timer <= 0f)
        {
            EndGame(false);
            return;
        }

        // Move flying object
        if (objectActive)
        {
            flyingObject.anchoredPosition += direction * speed * Time.deltaTime;

            if (flyingObject.anchoredPosition.magnitude > 800f)
            {
                LaunchNewObject();
            }
        }

        // Input for hitting
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryHit();
        }
    }

    void StartGame()
    {
        if (loopingAudioSource != null)
        {
            loopingAudioSource.loop = true;
            loopingAudioSource.Play();
        }

        clockHands.gameStarted = true;
        startText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        LaunchNewObject();

        if (clockHands != null)
            clockHands.gameStarted = true;
    }

    void LaunchNewObject()
    {
        Vector2[] spawnPoints = {
            new Vector2(-400, 400),
            new Vector2(400, 400),
            new Vector2(-400, -400),
            new Vector2(400, -400)
        };

        Vector2 start = spawnPoints[Random.Range(0, spawnPoints.Length)];
        flyingObject.anchoredPosition = start;
        direction = (Vector2.zero - start).normalized;
        canScore = true;
        objectActive = true;
        flyingObject.gameObject.SetActive(true);

        // Assign a random number sprite to the flying object
        if (numberSprites != null && numberSprites.Length > 0 && flyingObjectImage != null)
        {
            flyingObjectImage.sprite = numberSprites[Random.Range(0, numberSprites.Length)];
        }
    }

    void TryHit()
    {
        float distance = Vector2.Distance(flyingObject.anchoredPosition, Vector2.zero);

        if (distance < 60f && canScore)
        {
            // Successful hit
            score++;
            canScore = false;

            // Particle effect
            MinigameParticles particles = FindAnyObjectByType<MinigameParticles>();
            if (particles != null)
            {
                particles.SpawnParticles();
            }

            flyingObject.gameObject.SetActive(false);
            objectActive = false;

            if (score >= scoreToPass)
            {
                EndGame(true);
            }
            else
            {
                Invoke(nameof(LaunchNewObject), 0.3f);
            }
        }
        else if (distance >= 60f && objectActive)
        {
            // Clicked when object NOT in center zone — invalidate it
            canScore = false;
            flyingObject.gameObject.SetActive(false);
            objectActive = false;

            FlashRed();  // Trigger red flash feedback

            // No score added, just spawn a new object shortly
            Invoke(nameof(LaunchNewObject), 0.3f);
        }
    }

    void EndGame(bool success)
    {
        if (success)
        {
            Debug.Log("Minigame Complete!");
            sceneFader.FadeToScene(nextSceneName);
        }
        else
        {
            Debug.Log("Minigame Failed!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart on fail
        }

        enabled = false;
    }

    void FlashRed()
    {
        if (redFlashImage == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(RedFlashRoutine());
    }

    IEnumerator RedFlashRoutine()
    {
        Color originalColor = redFlashImage.color;
        // Set alpha to 0.5 (semi-transparent red)
        redFlashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            // Fade alpha from 0.5 to 0 smoothly
            float alpha = Mathf.Lerp(0.5f, 0f, elapsed / flashDuration);
            redFlashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure fully transparent at the end
        redFlashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
