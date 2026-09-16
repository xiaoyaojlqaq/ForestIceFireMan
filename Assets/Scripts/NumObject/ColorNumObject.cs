using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(NumObjectInfo))]
public class ColorNumObject : MonoBehaviour
{
    public int number;
    public ColorType colorType;

    [Header("Color Sprites")]
    public Sprite RedSprite;
    public Sprite BlueSprite;

    [Header("碰撞标签")]
    [SerializeField] private string fireTag  = "fire";
    [SerializeField] private string waterTag = "water";

    // 触发去重：用 HashSet 记录已进入的 Collider，防止两个 BoxCollider2D 对同一触发区重复响应
    private readonly System.Collections.Generic.HashSet<int> _activeTriggers
        = new System.Collections.Generic.HashSet<int>();

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private NumObjectInfo info;
    private Text numText;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        info = GetComponent<NumObjectInfo>();

        // 安全获取 Canvas/numText 子节点
        if (transform.childCount > 0)
        {
            Transform canvas = transform.GetChild(0);
            if (canvas != null && canvas.childCount > 0)
                numText = canvas.GetChild(0).GetComponent<Text>();
        }

        // 根据颜色类型设置 Sprite
        spriteRenderer.sprite = colorType == ColorType.Red ? RedSprite : BlueSprite;

        SetNumber(number);
    }

public void SetNumber(int num)
    {
        number = Mathf.Clamp(num, 0, 100);

        if (numText != null)
            numText.text = number.ToString();

        UpdateInfo();

        // 数字等于 0 时延迟 1 秒摧毁
        if (number == 0)
        {
            Destroy(gameObject, 1f);
        }
    }

/// <summary>
/// 同时设置颜色和数字，并刷新 Sprite。
/// 由 ThrowSlot 在实例化后调用，确保 Sprite 正确显示。
/// </summary>
public void Init(int number, ColorType color)
{
    colorType = color;

    // 确保 SpriteRenderer 已初始化
    if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();

    spriteRenderer.sprite = colorType == ColorType.Red ? RedSprite : BlueSprite;

    SetNumber(number);
}


    private void UpdateInfo()
    {
        if (info != null && spriteRenderer != null)
        {
            // 调用带颜色参数的 SetInfo 重载，同步颜色信息
            info.SetInfo(number, spriteRenderer.sprite, colorType);
        }
    }

    // ── 碰撞检测 ──────────────────────────────────────────────────────────────

private void OnCollisionEnter2D(Collision2D col)
{
    // fire/water 是触发区域，不走此路径；此处仅处理真实物理碰撞中带目标标签的情况
    ApplyTagEffect(col.gameObject.tag);
}

private void OnTriggerEnter2D(Collider2D other)
{
    // HashSet.Add 返回 false 说明该 Collider 已经处理过（另一个 BoxCollider2D 先一步进入）
    if (!_activeTriggers.Add(other.GetInstanceID()))
        return;

    ApplyTagEffect(other.gameObject.tag);
}

private void OnTriggerExit2D(Collider2D other)
{
    _activeTriggers.Remove(other.GetInstanceID());
}



    /// <summary>
    /// 根据接触物体的标签和自身颜色类型改变数字。
    /// Red  : fire +1 / water -1
    /// Blue : fire -1 / water +1
    /// </summary>
    private void ApplyTagEffect(string tag)
    {
        int delta = 0;

        if (tag == fireTag)
            delta = colorType == ColorType.Red ? 1 : -1;
        else if (tag == waterTag)
            delta = colorType == ColorType.Red ? -1 : 1;
        else
            return; // 其他标签不处理

        SetNumber(number + delta);
        Debug.Log($"[ColorNumObject] 碰到 {tag}，颜色 {colorType}，数字 {number - delta} -> {number}");
    }
}

public enum ColorType
{
    Red = 0,
    Blue = 1,
}
