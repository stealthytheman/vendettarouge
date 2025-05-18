using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeTravelMinigame : MonoBehaviour
{
    public RectTransform flyingObject;
    public Sprite[] numberSprites;           // Assign your number sprites here
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI startText;
    public float speed = 300f;
    public float gameDuration = 10f;
    public int scoreToPass = 3;
    public string nextSceneName = "YourNextScene";

    private float timer;
    private int score;
    private Vector2 direction;
    private bool canScore = true;
    private bool objectActive = true;
    private bool gameStarted = false;
    private Image flyingObjectImage;


    public SceneFader sceneFader; // Assign in Inspector

    void Start()
    {
        timer = gameDuration;
        score = 0;
        gameStarted = false;

        flyingObjectImage = flyingObject.GetComponent<Image>();

        // Show start prompt, hide game UI
        startText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        flyingObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted)
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
        gameStarted = true;
        startText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        LaunchNewObject();
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
            score++;
            canScore = false;

            // Call the particle effect from MinigameParticles script
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
}
