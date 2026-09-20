using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在 Poison/LiquidTrigger 上。
/// 进入毒液触发区时：
///   - Player        → 每 1 秒扣 1 点生命值，离开后停止
///   - ColorNumObject → 与 water 效果相同：Red -1 / Blue +1
///   - NumObject     → 与 water 效果相同（暂无额外逻辑）
/// 
/// 防重复触发：HashSet 记录根 GameObject InstanceID，
/// 避免同一物体因多个 Collider 触发多次。
/// </summary>
[DisallowMultipleComponent]
public sealed class PoisonTriggerHandler : MonoBehaviour
{
    [Header("触发设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField, Min(0.1f)] private float damageInterval = 1f;
    [SerializeField, Min(1)]    private int   damagePerTick  = 1;

    // 防重复：记录已处理的根 GameObject InstanceID
    private readonly HashSet<int> processedObjects = new HashSet<int>();

    // 玩家持续伤害协程引用（用于离开时停止）
    private Coroutine playerDamageCoroutine;

    // ── Trigger 事件 ──────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!other.CompareTag("Player")) return;
        // 取根物体去重（数字物体有 2 个 Collider，统一用根 GO 的 ID）
        GameObject root = GetRoot(other);
        if (!processedObjects.Add(root.GetInstanceID())) return;

        // ── 玩家：启动持续伤害协程 ─────────────────────────────────────────
        if (root.CompareTag(playerTag))
        {
            root.GetComponent<PoisoningState>().StartBurning();
            PlayerHealth health = root.GetComponent<PlayerHealth>();
            health.TakeDamage(damagePerTick);
            if (health != null)
            {
                playerDamageCoroutine = StartCoroutine(PlayerDamageLoop(health));
                Debug.Log("[PoisonTriggerHandler] 玩家进入毒液，开始持续伤害。");
            }
            return;
        }

        // ── ColorNumObject：与 water 效果相同 ─────────────────────────────
        ColorNumObject colorNum = root.GetComponent<ColorNumObject>();
        if (colorNum != null)
        {
            ApplyWaterEffect(colorNum);
            return;
        }

        // ── NumObject：与 water 效果相同（NumObject 自身无 water 逻辑，预留） ─
        NumObject numObj = root.GetComponent<NumObject>();
        if (numObj != null)
        {
            // NumObject 遇 water 暂无特殊逻辑，与 water 行为一致（不处理）
            Debug.Log($"[PoisonTriggerHandler] NumObject({numObj.number}) 进入毒液，与 water 效果相同（无变化）。");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (!other.CompareTag("Player")) return;
        GameObject root = GetRoot(other);

        // 只有当该根物体的最后一个 Collider 也离开时才清除
        // 用 OverlapCollider 检测是否还有碰撞体留在触发区内
        // 简化方案：直接移除并处理，因为 Exit 也可能被两个 Collider 各调一次
        if (!processedObjects.Contains(root.GetInstanceID())) return;

        // 检测是否该对象仍有 Collider 在触发区内（防止第一个 Collider Exit 就停止）
        if (IsAnyColliderStillInside(root)) return;

        processedObjects.Remove(root.GetInstanceID());

        // 玩家离开，停止持续伤害
        if (root.CompareTag(playerTag))
        {
            root.GetComponent<PoisoningState>().StopBurning();
            if (playerDamageCoroutine != null)
            {
                StopCoroutine(playerDamageCoroutine);
                playerDamageCoroutine = null;
            }
            Debug.Log("[PoisonTriggerHandler] 玩家离开毒液，停止持续伤害。");
        }
    }

    // ── 协程：玩家持续伤害 ────────────────────────────────────────────────────

    private IEnumerator PlayerDamageLoop(PlayerHealth health)
    {
        while (health != null && health.IsAlive)
        {
            yield return new WaitForSeconds(damageInterval);
            if (health == null || !health.IsAlive) break;

            health.TakeDamage(damagePerTick);
            Debug.Log($"[PoisonTriggerHandler] 毒液持续伤害，扣 {damagePerTick} 点，剩余 {health.CurrentHealth}/{health.MaxHealth}");
        }
    }

    // ── ColorNumObject water 效果 ─────────────────────────────────────────────

    /// <summary>
    /// 与 water 标签效果相同：Red -1 / Blue +1
    /// </summary>
    private void ApplyWaterEffect(ColorNumObject colorNum)
    {
        int delta = colorNum.colorType == ColorType.Red ? -1 : 1;
        colorNum.SetNumber(colorNum.number + delta);
        Debug.Log($"[PoisonTriggerHandler] ColorNumObject({colorNum.colorType}) 进入毒液（water 效果），number → {colorNum.number}");
    }

    // ── 辅助方法 ──────────────────────────────────────────────────────────────

    /// <summary>取 Collider 所在的根 GameObject（处理子物体 Collider 情况）。</summary>
    private static GameObject GetRoot(Collider2D col)
    {
        // 数字物体的 Collider 直接挂在根物体上，GetComponentInParent 安全兼容
        ColorNumObject c = col.GetComponentInParent<ColorNumObject>();
        if (c != null) return c.gameObject;
        NumObject n = col.GetComponentInParent<NumObject>();
        if (n != null) return n.gameObject;
        return col.gameObject;
    }

    /// <summary>
    /// 检测 root 物体的任意 Collider2D 是否仍与本触发器重叠。
    /// 用于在 OnTriggerExit2D 中判断"是否完全离开"。
    /// </summary>
    private bool IsAnyColliderStillInside(GameObject root)
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null) return false;

        Collider2D[] cols = root.GetComponents<Collider2D>();
        foreach (Collider2D col in cols)
        {
            if (col.IsTouching(myCollider)) return true;
        }
        return false;
    }
}
