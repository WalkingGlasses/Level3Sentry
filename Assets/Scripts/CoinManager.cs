using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Coin")]
    public GameObject coinPrefab;

    [Header("Coin UI")]
    public RectTransform coinTarget;
    public TMP_Text coinText;

    [Header("Starting Coins")]
    public int bankCoins = 100;

    [Header("Counter Animation")]
    public float coinCountSpeed = 20f;

    private int displayedCoins;

    private int targetCoins;

    private bool countingCoins = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        displayedCoins = bankCoins;
        targetCoins = bankCoins;

        UpdateCoinText();
    }

    void Update()
    {
        AnimateCoinCounter();
    }

    public void SpawnCoin(
        Vector3 worldPosition,
        int amount)
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning(
                "CoinManager: Coin Prefab is not assigned...areeee you stupid?"
            );

            return;
        }

        GameObject coinObject =
            Instantiate(
                coinPrefab,
                worldPosition,
                Quaternion.identity
            );

        Coin coin =
            coinObject.GetComponent<Coin>();

        if (coin == null)
        {
            Debug.LogWarning(
                "Coin Prefab does not have Coin.cs. The thing, you know the thing it needs to work..."
            );

            Destroy(coinObject);

            return;
        }

        coin.target =
            coinTarget;

        coin.amount =
            amount;
    }

    public void AddCoins(int amount)//me when money
    {
        targetCoins += amount;

        countingCoins = true;
    }
    

    void AnimateCoinCounter()
    {
        if (!countingCoins)
            return;

        if (displayedCoins >= targetCoins)
        {
            displayedCoins =
                targetCoins;

            countingCoins = false;

            UpdateCoinText();

            return;
        }

        float coinsThisFrame =
            coinCountSpeed *
            Time.deltaTime;

        displayedCoins +=
            Mathf.Max(
                1,
                Mathf.RoundToInt(
                    coinsThisFrame
                )
            );

        if (displayedCoins > targetCoins)
        {
            displayedCoins =
                targetCoins;
        }

        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text =
                displayedCoins.ToString();
        }
    }
    

    public void PunchCoinText()
    {
        StopAllCoroutines();

        StartCoroutine(
            PunchCoinTextRoutine()
        );
    }

    System.Collections.IEnumerator
        PunchCoinTextRoutine()//NOTE!!!!: due to how the code is written.. rapid consecutive coin increases WILL AND HAVE MAKE THIS TEXT REALLY BIG, it stays its too funny. kinda want other games to have this as a stupid feature now like an optional thing in settings like old games had cheats
    {//TLDR for the note above: big text has no limit, can get FUNNY big
        if (coinText == null)
            yield break;

        Transform textTransform =
            coinText.transform;

        Vector3 originalScale =
            textTransform.localScale;

        Vector3 biggerScale =
            originalScale * 1.25f;

        float duration = 0.15f;

        float timer = 0f;

        // Grow
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            float easedT =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            textTransform.localScale =
                Vector3.Lerp(
                    originalScale,
                    biggerScale,
                    easedT
                );

            yield return null;
        }

        timer = 0f;

        // Shrink
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            float easedT =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            textTransform.localScale =
                Vector3.Lerp(
                    biggerScale,
                    originalScale,
                    easedT
                );

            yield return null;
        }

        textTransform.localScale =
            originalScale;
    }
}