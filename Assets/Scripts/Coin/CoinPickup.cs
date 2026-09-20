using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// Adds 1 to the gold count UI when the player enters the attached trigger,
/// then destroys the coin.
/// Attach this component to the Coin trigger child object.
/// </summary>
[DisallowMultipleComponent]
public sealed class CoinPickup : MonoBehaviour
{
    private const string CanvasName = "MainUICanvas";
    private const string GoldCoinTextPath = "GoldCoin/Text (Legacy)";
    private const string PlayerTag = "Player";

    private GameObject coinRoot;
    private Text goldCoinText;
    private RectTransform coinUI;

    private void Awake()
    {
        coinUI=GameObject.FindWithTag("coinUI").GetComponent<RectTransform>();
        coinRoot = transform.parent != null ? transform.parent.gameObject : gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerTag))
            return;

        if (!TryUpdateGoldCount())
        {
            Debug.LogWarning(
                "CoinPickup: gold count Text not found at MainUICanvas/GoldCoin/Text (Legacy).",
                this);
        }
        coinAudioManager.instance.PlayCoinSound();
        transform.parent.DOMove(Camera.main.ScreenPointToRay(coinUI.position).origin, 0.4f);
        Destroy(coinRoot,0.4f);
    }

    private bool TryUpdateGoldCount()
    {
        if (goldCoinText == null)
        {
            GameObject canvas = GameObject.Find(CanvasName);
            goldCoinText = canvas != null
                ? canvas.transform.Find(GoldCoinTextPath)?.GetComponent<Text>()
                : null;
        }

        if (goldCoinText == null)
            return false;

        int.TryParse(goldCoinText.text, out int currentCount);
        goldCoinText.text = (currentCount + 1).ToString();
        return true;
    }
}
