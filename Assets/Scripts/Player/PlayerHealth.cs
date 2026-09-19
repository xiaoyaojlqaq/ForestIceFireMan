using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 玩家生命值管理。挂载在 player 上。
/// 默认最大生命值 3，通过 TakeDamage / Heal 增减，
/// 并通过 Inspector 拖拽绑定 healthBarIcons 来同步生命图标显隐。
/// </summary>
[DisallowMultipleComponent]
public sealed class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField, Min(1)] private int maxHealth = 3;
    public Image hurtImage;

    // ── 内部状态 ──────────────────────────────────────────────────────────────
    [SerializeField]private int currentHealth;
    [Header("生命值UI")]
    [SerializeField] private GameObject[] healthBarIcons;
    private bool missingIconsWarningLogged;

    // ── 事件 ─────────────────────────────────────────────────────────────────
    /// <summary>生命值变化时触发（当前值, 最大值）</summary>
    public event System.Action<int, int> OnHealthChanged;
    /// <summary>生命值归零时触发</summary>
    public event System.Action OnDeath;

    // ── 只读属性 ──────────────────────────────────────────────────────────────
    public int CurrentHealth => currentHealth;
    public int MaxHealth     => maxHealth;
    public bool IsAlive      => currentHealth > 0;

    // ── 生命周期 ──────────────────────────────────────────────────────────────
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        RefreshUI();
    }

    // ── 公共 API ──────────────────────────────────────────────────────────────

    /// <summary>受到伤害，amount 为正整数。归零时触发 OnDeath。</summary>
    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        hurtImage.DOFade(0.35f, 0.15f).OnComplete(() => hurtImage.DOFade(0f, 0.15f));
        RefreshUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[PlayerHealth] 受到 {amount} 点伤害，剩余 {currentHealth}/{maxHealth}");

        if (currentHealth == 0)
        {
            Debug.Log("[PlayerHealth] 玩家死亡！");
            OnDeath?.Invoke();
        }
    }

    /// <summary>恢复生命值，amount 为正整数，不超过上限。</summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        RefreshUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[PlayerHealth] 恢复 {amount} 点生命，当前 {currentHealth}/{maxHealth}");
    }

    /// <summary>立即重置为满血。</summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        RefreshUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // ── 私有辅助 ──────────────────────────────────────────────────────────────

    /// <summary>将当前血量同步到 Inspector 拖拽绑定的生命图标。</summary>
    private void RefreshUI()
    {
        if (healthBarIcons == null || healthBarIcons.Length == 0)
        {
            if (!missingIconsWarningLogged)
            {
                missingIconsWarningLogged = true;
                Debug.LogWarning("[PlayerHealth] 未在 Inspector 中拖拽赋值 healthBarIcons，生命值图标 UI 将不更新。");
            }
            return;
        }

        for (int i = 0; i < healthBarIcons.Length; i++)
        {
            if (healthBarIcons[i] == null) continue;
            healthBarIcons[i].SetActive(i < currentHealth);
        }
    }
}
