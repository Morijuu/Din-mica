using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private int totalScore = 0;
    private int blocksDestroyedThisShot = 0;
    private float comboResetTimer = 0f;
    private float comboResetDelay = 3f;

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

    private void Update()
    {
        if (blocksDestroyedThisShot > 0)
        {
            comboResetTimer += Time.deltaTime;
            if (comboResetTimer >= comboResetDelay)
            {
                ResetCombo();
            }
        }
    }

    public void AddPoints(int basePoints)
    {
        blocksDestroyedThisShot++;
        comboResetTimer = 0f;

        int comboMultiplier = blocksDestroyedThisShot;
        int earnedPoints = basePoints * comboMultiplier;

        totalScore += earnedPoints;

        Debug.Log($"Bloque #{blocksDestroyedThisShot} | +{earnedPoints} puntos (combo x{comboMultiplier})");

        UpdateUI();
    }

    private void ResetCombo()
    {
        if (blocksDestroyedThisShot > 0)
        {
            Debug.Log($"Combo terminado: {blocksDestroyedThisShot} bloques destruidos");
        }
        blocksDestroyedThisShot = 0;
        comboResetTimer = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + totalScore;

        if (comboText != null)
        {
            if (blocksDestroyedThisShot > 1)
                comboText.text = "Combo x" + blocksDestroyedThisShot;
            else
                comboText.text = "";
        }
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public int GetCurrentCombo()
    {
        return blocksDestroyedThisShot;
    }
}
