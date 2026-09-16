using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包 UI 控制器。
/// 挂载在 BagUI GameObject 上。
/// 自动监听 Player 上的 <see cref="Inventory"/> 事件，刷新 3 个背包槽的图标与数字显示。
/// 槽结构：slot1/object1（Image）+ slot1/object1/Text (Legacy)（Text），slot2 / slot3 同理。
/// </summary>
[DisallowMultipleComponent]
public sealed class BagUIController : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────────
    [Header("背包槽 — Image（数字物体图标）")]
    [SerializeField] private Image slot1Image;
    [SerializeField] private Image slot2Image;
    [SerializeField] private Image slot3Image;

    [Header("背包槽 — Text（数字）")]
    [SerializeField] private Text slot1Text;
    [SerializeField] private Text slot2Text;
    [SerializeField] private Text slot3Text;

    [Header("空槽占位（留空则隐藏 Image）")]
    [SerializeField] private Sprite emptySlotSprite;

    // ── 内部引用 ───────────────────────────────────────────────────────────────
    private Inventory inventory;

    // ── 生命周期 ───────────────────────────────────────────────────────────────
    private void Start()
    {
        // 自动从场景中的 Player 找到 Inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            inventory = player.GetComponent<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning("[BagUIController] 未找到带 Inventory 的 Player，UI 不会刷新。");
            return;
        }

        inventory.OnChanged += RefreshUI;

        // 启动时立即刷新一次
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChanged -= RefreshUI;
    }

    // ── 刷新 ──────────────────────────────────────────────────────────────────
    /// <summary>根据当前 Inventory 内容刷新 3 个槽位显示。</summary>
    private void RefreshUI()
    {
        RefreshSlot(0, slot1Image, slot1Text);
        RefreshSlot(1, slot2Image, slot2Text);
        RefreshSlot(2, slot3Image, slot3Text);
    }

    private void RefreshSlot(int index, Image img, Text txt)
    {
        InventoryItem item = (inventory != null) ? inventory.GetItem(index) : null;

        if (item != null)
        {
            // 有物品：显示图标和数字
            if (img != null)
            {
                img.sprite = item.Sprite != null ? item.Sprite : emptySlotSprite;
                img.enabled = true;
            }
            if (txt != null)
            {
                txt.text = item.Number.ToString();
                txt.enabled = true;
            }
        }
        else
        {
            // 槽位为空：恢复占位或隐藏
            if (img != null)
            {
                if (emptySlotSprite != null)
                {
                    img.sprite = emptySlotSprite;
                    img.enabled = true;
                }
                else
                {
                    img.enabled = false;
                }
            }
            if (txt != null)
            {
                txt.text = "";
                txt.enabled = false;
            }
        }
    }
}
