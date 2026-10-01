using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Reward")]
    public int amount = 10;

    [Header("Movement")]
    public float speed = 8f;

    [Header("Arrival")]//Coming also works but I don't wanna sound like an eroge protag
    public float arrivalDistance = 20f;

    [HideInInspector]
    public RectTransform target;

    private Vector3 targetPosition;

    private bool collected = false;

    void Update()
    {
        if (collected)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdateTargetPosition();
        
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

        float distance =
            Vector3.Distance(
                transform.position,
                targetPosition
            );

        if (distance <= arrivalDistance)
        {
            CollectCoin();
        }
    }


    void UpdateTargetPosition()
    {
        targetPosition =
            target.position;
    }

    void CollectCoin()
    {
        if (collected)
            return;

        collected = true;

        CoinManager.Instance.AddCoins(
            amount
        );

        CoinManager.Instance.PunchCoinText();

        Destroy(gameObject);
    }
}