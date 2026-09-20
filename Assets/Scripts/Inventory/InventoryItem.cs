using UnityEngine;

/// <summary>
/// 数字物体类型枚举
/// </summary>
public enum NumObjectType
{
    NumObject,      // 普通数字物体
    ColorNumObject  // 彩色数字物体
}

/// <summary>
/// 背包中单个数字物体的数据快照（值类型语义封装）。
/// 不继承 MonoBehaviour，可安全地跨上下文传递和序列化。
/// </summary>
[System.Serializable]
public sealed class InventoryItem
{
    [SerializeField] private int number;
    [SerializeField] private Sprite sprite;
    [SerializeField] private NumObjectType objectType;

    /// <summary>颜色类型；普通 NumObject 为 null，ColorNumObject 为 Red / Blue</summary>
    [SerializeField] private bool hasColorType;
    [SerializeField] private ColorType colorType;

    /// <summary>拾取时记录的数字值（0-10）</summary>
    public int Number => number;

    /// <summary>拾取时记录的 Sprite 图标</summary>
    public Sprite Sprite => sprite;

    /// <summary>物体类型，用于扔出时选择正确的预制体</summary>
    public NumObjectType ObjectType => objectType;

    /// <summary>颜色类型（仅 ColorNumObject 有效，普通 NumObject 返回 null）</summary>
    public ColorType? ColorType => hasColorType ? (ColorType?)colorType : null;

    /// <summary>普通 NumObject 构造：不携带颜色信息</summary>
    public InventoryItem(int number, Sprite sprite, NumObjectType objectType)
    {
        this.number     = number;
        this.sprite     = sprite;
        this.objectType = objectType;
        this.hasColorType = false;
    }

    /// <summary>ColorNumObject 构造：携带颜色信息</summary>
    public InventoryItem(int number, Sprite sprite, NumObjectType objectType, ColorType colorType)
    {
        this.number      = number;
        this.sprite      = sprite;
        this.objectType  = objectType;
        this.hasColorType = true;
        this.colorType   = colorType;
    }

    public override string ToString()
    {
        string spriteInfo = sprite != null ? sprite.name : "null";
        string colorInfo  = hasColorType ? colorType.ToString() : "None";
        return $"InventoryItem(number={number}, sprite={spriteInfo}, type={objectType}, color={colorInfo})";
    }
}