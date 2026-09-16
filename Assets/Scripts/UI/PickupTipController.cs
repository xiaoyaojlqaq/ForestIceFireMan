using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 拾取提示控制器。挂载在 TipsCanvas 上。
/// 自动检测玩家是否在任何可拾取物体（NumObjectPickup）范围内，
/// 若是则显示 TipText 并将 Canvas 移动到该物体上方，否则隐藏。
/// 适用于 WorldSpace Canvas。
/// </summary>
[DisallowMultipleComponent]
public sealed class PickupTipController : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("提示文字 Text 组件（自动从子节点查找）")]
    [SerializeField] private Text tipText;

    [Header("设置")]
    [Tooltip("提示文字内容")]
    [SerializeField] private string tipMessage = "F拾取";

    [Tooltip("Canvas 相对物体的世界偏移（Y 轴向上）")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.0f, 0f);

    private void Start()
    {
        // 如果未手动指定，自动从子节点查找 TipText
        if (tipText == null)
        {
            tipText = GetComponentInChildren<Text>();
        }

        // 初始隐藏
        if (tipText != null)
        {
            tipText.enabled = false;
        }
    }

private void LateUpdate()
    {
        if (tipText == null) return;

        // 查找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            HideTip();
            return;
        }

        // 背包已满时隐藏提示
        Inventory inventory = player.GetComponent<Inventory>();
        if (inventory != null && inventory.IsFull)
        {
            HideTip();
            return;
        }

        // 搜索所有 NumObjectPickup，找到第一个玩家在范围内的
        NumObjectPickup[] allPickups = FindObjectsOfType<NumObjectPickup>();
        NumObjectPickup targetPickup = null;

        foreach (NumObjectPickup pickup in allPickups)
        {
            if (pickup.IsPlayerInRange())
            {
                targetPickup = pickup;
                break;
            }
        }

        if (targetPickup != null)
        {
            // 显示提示并移动 Canvas 到物体上方
            ShowTipAt(targetPickup.transform.position + worldOffset);
        }
        else
        {
            HideTip();
        }
    }

    private void ShowTipAt(Vector3 worldPosition)
    {
        // 移动整个 Canvas（WorldSpace）
        transform.position = worldPosition;

        // 显示文字
        if (!tipText.enabled)
        {
            tipText.enabled = true;
            tipText.text = tipMessage;
        }
    }

    private void HideTip()
    {
        if (tipText != null && tipText.enabled)
        {
            tipText.enabled = false;
        }
    }
}
