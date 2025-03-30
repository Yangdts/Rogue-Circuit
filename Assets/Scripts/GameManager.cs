using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isTimerRunning = false;
    public int CurrentLevel => level;
    private int level;
    private int lives = 3;
    private int score = 0;
    public float totalPlayTime = 0f;
    private TMP_Text livesText;
    private TMP_Text scoreText;
    private TMP_Text timerText;
    public GameObject gameUI;
    public UIManager uiManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            if(SceneManager.GetActiveScene().buildIndex != 0)
            {
                InitializeGame();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        ResetTimer();
        LoadLevel(1);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadLevel(1);
    }

    private void Update()
    {
        if(isTimerRunning)
        {
            totalPlayTime += Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = "Time: " + Mathf.FloorToInt(totalPlayTime).ToString();
            }
        }
    }

    public void LoadLevel(int index)
    {
        level = index;

        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 0.1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void ResetTimer()
    {
        totalPlayTime = 0f;
        isTimerRunning = false;
        if (timerText != null)
        {
            timerText.text = "Timer: 0";
        }
    }
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplayLevel = scene.buildIndex >= 1 && scene.buildIndex <= SceneManager.sceneCountInBuildSettings;
        if (isGameplayLevel )
        {
            if (!isTimerRunning)
            {
                StartTimer();
            }
        }
        else
        {
            isTimerRunning = false;
        }
        UpdateUIReferences();
    }

    

    public void LevelComplete()
    {
        score += 1000;
        float timeBonus = Mathf.Max(0, 500 - (totalPlayTime * 2));
        score += Mathf.FloorToInt(timeBonus);

        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            isTimerRunning = false;
            Destroy(gameObject);
            LoadLevel(0);
        }

        UpdateUI();
    }

    public void LevelFailed()
    {
        lives--;
        UpdateUI();
        if (lives > 0)
        {
            LoadLevel(level);
        }
        else
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        lives = 3;
        score = 0;
        totalPlayTime = 0f;
        UpdateUI();

        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        SceneManager.LoadScene(0);
        Invoke(nameof(UpdateUIReferences), 0.1f);
    }

    private void UpdateUIReferences()
    {
        if (gameUI == null)
        {
            gameUI = GameObject.Find("GameUI");
        }
        if (gameUI != null)
        {
            gameUI.SetActive(true);

            livesText = gameUI.transform.Find("LivesText")?.GetComponent<TMP_Text>();
            scoreText = gameUI.transform.Find("ScoreText")?.GetComponent<TMP_Text>();
            timerText = gameUI.transform.Find("TimerText")?.GetComponent<TMP_Text>();


            uiManager = gameUI.GetComponent<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager component not found on GameUI");
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLives(lives);
            uiManager.UpdateScore(score);
        }
    }
}
