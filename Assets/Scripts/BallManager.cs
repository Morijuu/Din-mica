using UnityEngine;
using TMPro;
using System.Collections;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance;

    [SerializeField] private TextMeshProUGUI ballsStructureText;
    [SerializeField] private TextMeshProUGUI ballsTotalText;

    private int totalBalls = 30;
    private int ballsRemaining = 30;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void OnBallThrown()
    {
        if (ballsRemaining > 0)
        {
            ballsRemaining--;
            TowerSpawner spawner = FindFirstObjectByType<TowerSpawner>();
            if (spawner != null)
            {
                spawner.OnBallThrown();
                spawner.CheckIfBallsExhausted();
            }

            UpdateUI();
        }
    }

    public void CheckGameOver()
    {
        if (ballsRemaining <= 0)
        {
            Debug.Log("GAME OVER - Se acabaron las bolas");
            GameOverManager.Instance.ShowGameOver();
        }
    }

    private void UpdateUI()
    {
        if (ballsStructureText != null)
        {
            TowerSpawner spawner = FindFirstObjectByType<TowerSpawner>();
            if (spawner != null)
            {
                int used = spawner.GetBallsUsedThisStructure();
                int max = spawner.GetMaxBallsPerStructure();
                ballsStructureText.text = $"Bolas: {max - used}/{max}";
            }
        }

        if (ballsTotalText != null)
        {
            ballsTotalText.text = $"Total: {ballsRemaining}/{totalBalls}";
        }
    }

    public int GetBallsRemaining()
    {
        return ballsRemaining;
    }

    public int GetTotalBalls()
    {
        return totalBalls;
    }
}
