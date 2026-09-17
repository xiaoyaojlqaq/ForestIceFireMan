using UnityEngine;

[DisallowMultipleComponent]
public sealed class NumObjectInfo : MonoBehaviour
{
    [SerializeField] private int currentNumber;
    [SerializeField] private Sprite sprite;

    /// <summary>
    /// 是否携带颜色信息（普通 NumObject 为 false，ColorNumObject 为 true）
    /// </summary>
    [SerializeField] private bool hasColorType;

    /// <summary>
    /// 颜色类型（仅当 hasColorType == true 时有效）
    /// </summary>
    [SerializeField] private ColorType colorType;

    // ── 只读属性 ──────────────────────────────────────────────────────────────

    public int    CurrentNumber => currentNumber;
    public Sprite Sprite        => sprite;

    /// <summary>颜色类型。若物体不携带颜色信息则返回 null。</summary>
    public ColorType? ColorType => hasColorType ? (ColorType?)colorType : null;

    // ── 写入接口 ──────────────────────────────────────────────────────────────

    /// <summary>普通 NumObject 调用：仅设置数字和 Sprite，不携带颜色。</summary>
    public void SetInfo(int number, Sprite currentSprite)
    {
        currentNumber = number;
        sprite        = currentSprite;
        hasColorType  = false;
    }

    /// <summary>ColorNumObject 调用：设置数字、Sprite 以及颜色类型。</summary>
    public void SetInfo(int number, Sprite currentSprite, ColorType color)
    {
        currentNumber = number;
        sprite        = currentSprite;
        hasColorType  = true;
        colorType     = color;
    }
}
