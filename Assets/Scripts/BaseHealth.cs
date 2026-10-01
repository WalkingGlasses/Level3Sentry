using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance;

    [Header("Health")]
    public int maxHealth = 20;

    [Header("HP Bar")]
    public Image realHP;
    public Image ghostHP;

    [Header("Ghost HP")]
    public float ghostDelay = 0.5f;
    public float ghostSpeed = 2f;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private float currentHealth;
    private float ghostHealth;

    private float ghostTimer;

    private bool gameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        ghostHealth = maxHealth;

        UpdateBars();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        UpdateGhostHealth();
    }

    public void TakeDamage(int damage)
    {
        if (gameOver)
            return;

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );

        ghostTimer = ghostDelay;//Guardians make their own fate

        UpdateBars();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }


    void UpdateGhostHealth()
    {
        if (ghostTimer > 0f)
        {
            ghostTimer -= Time.deltaTime;
            return;
        }

        if (ghostHealth > currentHealth)
        {
            float t =
                Mathf.Clamp01(
                    Time.deltaTime * ghostSpeed
                );

            float easedT =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            ghostHealth =
                Mathf.Lerp(
                    ghostHealth,
                    currentHealth,
                    easedT
                );

            UpdateBars();
        }
    }

    void UpdateBars()
    {
        if (realHP != null)
        {
            realHP.fillAmount =
                currentHealth / maxHealth;
        }

        if (ghostHP != null)
        {
            ghostHP.fillAmount =
                ghostHealth / maxHealth;
        }
    }

    void GameOver()
    {
        gameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}