using UnityEngine;

public class Goal : MonoBehaviour
{
    public float goalDistance = 1f;

    private Transform player;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                player.position,
                transform.position
            );

        if (distance <= goalDistance)
        {
            GameManager.Instance.PlayerWon();
        }
    }
}