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
/// 背包中单个数字物体的数据快照。
/// 新增 SourceObject 字段：记录被隐藏的原始 GameObject，
/// 扔出时直接激活该对象，而非重新实例化预制体。
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

    /// <summary>
    /// 拾取时被隐藏的原始 GameObject。
    /// 扔出时会将此对象重新激活并移动到玩家前方。
    /// </summary>
    [SerializeField] private GameObject sourceObject;

    // ── 只读属性 ─────────────────────────────────────────────────────────────

    /// <summary>拾取时记录的数字值（0-100）</summary>
    public int Number => number;

    /// <summary>拾取时记录的 Sprite 图标</summary>
    public Sprite Sprite => sprite;

    /// <summary>物体类型，用于扔出时选择逻辑</summary>
    public NumObjectType ObjectType => objectType;

    /// <summary>颜色类型（仅 ColorNumObject 有效，普通 NumObject 返回 null）</summary>
    public ColorType? ColorType => hasColorType ? (ColorType?)colorType : null;

    /// <summary>被隐藏的原始 GameObject（可能为 null，例如原对象已被销毁）</summary>
    public GameObject SourceObject => sourceObject;

    // ── 构造函数 ──────────────────────────────────────────────────────────────

    /// <summary>普通 NumObject 构造：不携带颜色信息</summary>
    public InventoryItem(int number, Sprite sprite, NumObjectType objectType, GameObject source = null)
    {
        this.number       = number;
        this.sprite       = sprite;
        this.objectType   = objectType;
        this.hasColorType = false;
        this.sourceObject = source;
    }

    /// <summary>ColorNumObject 构造：携带颜色信息</summary>
    public InventoryItem(int number, Sprite sprite, NumObjectType objectType, ColorType colorType, GameObject source = null)
    {
        this.number       = number;
        this.sprite       = sprite;
        this.objectType   = objectType;
        this.hasColorType = true;
        this.colorType    = colorType;
        this.sourceObject = source;
    }

    public override string ToString()
    {
        string spriteInfo = sprite != null ? sprite.name : "null";
        string colorInfo  = hasColorType ? colorType.ToString() : "None";
        string sourceInfo = sourceObject != null ? sourceObject.name : "null";
        return $"InventoryItem(number={number}, sprite={spriteInfo}, type={objectType}, color={colorInfo}, source={sourceInfo})";
    }
}
