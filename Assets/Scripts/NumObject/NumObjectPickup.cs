using UnityEngine;

/// <summary>
/// 挂载在数字物体（meta 等）上的拾取触发器。
/// 玩家进入 Trigger 区域后，需按 F 键才能拾取（由 PlayerController2D 调用 TryPickup）。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NumObjectInfo))]
public sealed class NumObjectPickup : MonoBehaviour
{
    [Header("拾取设置")]
    [Tooltip("进入 Trigger 的碰撞体必须具备此 Tag 才可拾取")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("拾取后是否立即销毁该 GameObject")]
    [SerializeField] private bool destroyOnPickup = true;

    private NumObjectInfo info;
    private NumObjectType objectType;
    private bool pickedUp;      // 防止重复拾取
    private bool playerInRange; // 玩家是否在触发区内

    private void Awake()
    {
        info = GetComponent<NumObjectInfo>();

        // 自动检测物体类型：有 ColorNumObject 组件则为 ColorNumObject，否则为 NumObject
        objectType = GetComponent<ColorNumObject>() != null
            ? NumObjectType.ColorNumObject
            : NumObjectType.NumObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
    }

    /// <summary>玩家是否在拾取范围内且物体尚未被拾取</summary>
    public bool IsPlayerInRange() => playerInRange && !pickedUp;

    /// <summary>
    /// 尝试将该物体拾取到指定背包。由 PlayerController2D 按 F 键时调用。
    /// </summary>
    /// <returns>成功拾取返回 true；背包满、重复拾取等情况返回 false。</returns>
    public bool TryPickup(Inventory inventory)
    {
        if (pickedUp || inventory == null) return false;

        bool success = inventory.AddItem(info, objectType);
        if (!success) return false; // 背包已满，物体保留在场景中

        pickedUp = true;

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);

        return true;
    }
}
