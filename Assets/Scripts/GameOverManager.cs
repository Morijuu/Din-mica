using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private UnityEngine.UI.Button restartButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                int finalScore = gameManager.GetTotalScore();
                finalScoreText.text = $"Score: {finalScore}";
            }
        }

        Time.timeScale = 0f; // Pausar el juego
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reanudar el juego
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
