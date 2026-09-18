using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// 玩家背包：挂载在 player GameObject 上。
/// 负责存储已拾取的 <see cref="InventoryItem"/> 列表，并对外广播变更事件。
/// </summary>
[DisallowMultipleComponent]
public sealed class Inventory : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────────
    [Header("容量设置")]
    [SerializeField, Min(1)] private int capacity = 20;

    // ── 事件 ───────────────────────────────────────────────────────────────────

    /// <summary>成功拾取时触发，携带本次加入的条目</summary>
    public event Action<InventoryItem> OnItemAdded;

    /// <summary>成功移除时触发，携带被移除的条目及其原来的槽索引</summary>
    public event Action<InventoryItem, int> OnItemRemoved;

    /// <summary>背包任意变化（添加/移除/清空）时触发</summary>
    public event Action OnChanged;

    // ── 内部数据 ───────────────────────────────────────────────────────────────
    private readonly List<InventoryItem> items = new List<InventoryItem>();

    // ── 只读视图（外部不可修改列表结构）──────────────────────────────────────
    private ReadOnlyCollection<InventoryItem> readOnlyItems;

    /// <summary>当前背包内所有条目（只读视图）</summary>
    public ReadOnlyCollection<InventoryItem> Items => readOnlyItems;

    /// <summary>当前已存入的条目数量</summary>
    public int Count => items.Count;

    /// <summary>背包最大容量</summary>
    public int Capacity => capacity;

    /// <summary>背包是否已满</summary>
    public bool IsFull => items.Count >= capacity;

    /// <summary>背包是否为空</summary>
    public bool IsEmpty => items.Count == 0;

    // ── 生命周期 ───────────────────────────────────────────────────────────────
    private void Awake()
    {
        readOnlyItems = items.AsReadOnly();
    }

    // ── 公共 API ───────────────────────────────────────────────────────────────

    /// <summary>
    /// 将一个 <see cref="NumObjectInfo"/> 的信息快照存入背包，同时记录源物体引用。
    /// </summary>
    /// <param name="info">物体信息组件</param>
    /// <param name="objectType">物体类型</param>
    /// <param name="source">被隐藏的原始 GameObject（扔出时复用）</param>
    /// <returns>成功返回 true；背包已满或 info 为 null 时返回 false。</returns>
    public bool AddItem(NumObjectInfo info, NumObjectType objectType, GameObject source = null)
    {
        if (info == null)
        {
            Debug.LogWarning("[Inventory] AddItem: info 为 null，忽略。");
            return false;
        }

        // 如果 info 携带颜色信息（ColorNumObject），透传给存储层
        if (info.ColorType.HasValue)
            return AddItem(info.CurrentNumber, info.Sprite, objectType, info.ColorType.Value, source);
        else
            return AddItem(info.CurrentNumber, info.Sprite, objectType, source);
    }

    /// <summary>
    /// 直接通过数值、Sprite、类型和<b>颜色</b>构造条目并存入背包。用于 ColorNumObject。
    /// </summary>
    public bool AddItem(int number, Sprite sprite, NumObjectType objectType, ColorType colorType, GameObject source = null)
    {
        if (IsFull)
        {
            Debug.LogWarning($"[Inventory] 背包已满（{capacity}），无法拾取数字 {number}。");
            return false;
        }

        var item = new InventoryItem(number, sprite, objectType, colorType, source);
        items.Add(item);

        OnItemAdded?.Invoke(item);
        OnChanged?.Invoke();

        Debug.Log($"[Inventory] 拾取 {item}，当前数量 {items.Count}/{capacity}");
        return true;
    }

    /// <summary>
    /// 直接通过数值、Sprite 和物体类型构造条目并存入背包。
    /// </summary>
    public bool AddItem(int number, Sprite sprite, NumObjectType objectType, GameObject source = null)
    {
        if (IsFull)
        {
            Debug.LogWarning($"[Inventory] 背包已满（{capacity}），无法拾取数字 {number}。");
            return false;
        }

        var item = new InventoryItem(number, sprite, objectType, source);
        items.Add(item);

        OnItemAdded?.Invoke(item);
        OnChanged?.Invoke();

        Debug.Log($"[Inventory] 拾取 {item}，当前数量 {items.Count}/{capacity}");
        return true;
    }

    /// <summary>
    /// 按槽索引移除一个条目。
    /// </summary>
    /// <param name="index">0-based 槽位索引</param>
    /// <returns>成功返回 true；索引越界返回 false。</returns>
    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning($"[Inventory] RemoveAt: 索引 {index} 越界（Count={items.Count}）。");
            return false;
        }

        InventoryItem removed = items[index];
        items.RemoveAt(index);

        OnItemRemoved?.Invoke(removed, index);
        OnChanged?.Invoke();

        Debug.Log($"[Inventory] 移除 {removed}，当前数量 {items.Count}/{capacity}");
        return true;
    }

    /// <summary>
    /// 移除第一个数字值与 <paramref name="number"/> 相符的条目。
    /// </summary>
    /// <returns>成功返回 true；未找到返回 false。</returns>
    public bool RemoveFirstByNumber(int number)
    {
        int idx = items.FindIndex(item => item.Number == number);
        if (idx < 0)
        {
            Debug.LogWarning($"[Inventory] RemoveFirstByNumber: 未找到 number={number}。");
            return false;
        }
        return RemoveAt(idx);
    }

    /// <summary>
    /// 按引用移除指定条目。
    /// </summary>
    public bool RemoveItem(InventoryItem item)
    {
        int idx = items.IndexOf(item);
        if (idx < 0) return false;
        return RemoveAt(idx);
    }

    /// <summary>清空背包所有条目。</summary>
    public void Clear()
    {
        if (items.Count == 0) return;
        items.Clear();
        OnChanged?.Invoke();
        Debug.Log("[Inventory] 背包已清空。");
    }

    /// <summary>
    /// 按槽位索引获取条目（越界返回 null）。
    /// </summary>
    public InventoryItem GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }

    /// <summary>
    /// 计算背包内所有条目数字值的总和。
    /// </summary>
    public int GetTotalNumber()
    {
        int total = 0;
        foreach (var item in items) total += item.Number;
        return total;
    }
}
