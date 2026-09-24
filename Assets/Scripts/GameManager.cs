using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winPanel;

    private bool gameWon = false;

    void Awake()
    {
        Instance = this;
    }

    public void PlayerHit()
    {
        if (gameWon)
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerWon()
    {
        if (gameWon)
            return;

        gameWon = true;

        TurretController[] turrets =
            FindObjectsByType<TurretController>(FindObjectsSortMode.None);

        foreach (TurretController turret in turrets)
        {
            turret.StopTurret();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
}