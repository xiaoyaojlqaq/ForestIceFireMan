using System.Collections;
using UnityEngine;

/// <summary>
/// 挂载在带 fire 标签的触发器上。
/// 玩家进入触发区时，每 1 秒扣除 1 点生命值，离开后停止。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class FireTriggerHandler : MonoBehaviour
{
    [Header("触发设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField, Min(0.1f)] private float damageInterval = 1f;
    [SerializeField, Min(1)] private int damagePerTick = 1;

    // 玩家持续伤害协程引用（用于离开时停止）
    private Coroutine playerDamageCoroutine;
    private PlayerHealth currentPlayerHealth;

    private void Awake()
    {
        // 确保 Collider2D 是触发器
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"[FireTriggerHandler] {gameObject.name} 的 Collider2D 已自动设为 Trigger");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只处理玩家
        if (!other.CompareTag(playerTag)) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        other.GetComponent<BurningState>()?.StartBurning(); // 如果有 BurningState 组件，开始燃烧状态
        if (health != null)
        {
            currentPlayerHealth = health;
            playerDamageCoroutine = StartCoroutine(PlayerDamageLoop(health));
            Debug.Log("[FireTriggerHandler] 玩家进入火焰区域，开始持续伤害。");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 只处理玩家
        if (!other.CompareTag(playerTag)) return;
        other.GetComponent<BurningState>()?.StopBurning(); // 如果有 BurningState 组件，停止燃烧状态
        // 停止持续伤害
        if (playerDamageCoroutine != null)
        {
            StopCoroutine(playerDamageCoroutine);
            playerDamageCoroutine = null;
            currentPlayerHealth = null;
            Debug.Log("[FireTriggerHandler] 玩家离开火焰区域，停止持续伤害。");
        }
    }

    /// <summary>
    /// 协程：每隔指定时间对玩家造成伤害
    /// </summary>
    private IEnumerator PlayerDamageLoop(PlayerHealth health)
    {
        while (health != null && health.IsAlive)
        {
            yield return new WaitForSeconds(damageInterval);
            
            if (health == null || !health.IsAlive) break;

            health.TakeDamage(damagePerTick);
            Debug.Log($"[FireTriggerHandler] 火焰持续伤害，扣 {damagePerTick} 点，剩余 {health.CurrentHealth}/{health.MaxHealth}");
        }
    }

    private void OnDestroy()
    {
        // 清理协程
        if (playerDamageCoroutine != null)
        {
            StopCoroutine(playerDamageCoroutine);
            playerDamageCoroutine = null;
        }
    }
}
