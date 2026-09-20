using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds 1 to the mirror shard count UI when the player touches this trigger.
/// Attach it to the trigger child of a Mirror_Puzzle_Shard object.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class MirrorShardPickup : MonoBehaviour
{
    private const string CanvasName = "MainUICanvas";
    private const string MirrorTextPath = "Mirror/Text (Legacy)";
    private const string PlayerTag = "Player";

    private GameObject shardRoot;
    private Text mirrorCountText;
    private bool wasCollected;

    private RectTransform mirrorUI;

    private void Awake()
    {
        mirrorUI = GameObject.FindWithTag("mirrorUI").GetComponent<RectTransform>();

        shardRoot = transform.parent != null ? transform.parent.gameObject : gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (wasCollected || !other.CompareTag(PlayerTag))
            return;

        wasCollected = true;
        if (!TryUpdateMirrorCount(other.transform.GetChild(1).gameObject))
        {
            wasCollected = false;
            Debug.LogWarning(
                "MirrorShardPickup: mirror count Text not found at MainUICanvas/Mirror/Text (Legacy).",
                this);
            return;
        }

        Destroy(shardRoot,0.6f);
    }

    private bool TryUpdateMirrorCount(GameObject MirrorFinishOB)
    {
        if (mirrorCountText == null)
        {
            GameObject canvas = GameObject.Find(CanvasName);
            mirrorCountText = canvas != null
                ? canvas.transform.Find(MirrorTextPath)?.GetComponent<Text>()
                : null;
        }

        if (mirrorCountText == null)
            return false;

        int.TryParse(mirrorCountText.text.Trim(), out int currentCount);
        mirrorCountText.text = (currentCount + 1).ToString();
        if (currentCount >= 2)
        {
            Debug.Log("碎片大于3");
            MirrorFinishOB.SetActive(true);
        }
        transform.parent.DOMove(Camera.main.ScreenPointToRay(mirrorUI.position).origin, 0.4f);

        return true;
    }
}
