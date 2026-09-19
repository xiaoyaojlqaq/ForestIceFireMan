using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数字物体生成器。负责在运行时动态创建数字物体 GameObject（meta 类型）。
/// 生成的物体包含完整组件（Rigidbody2D, Collider, NumObject, NumObjectInfo, NumObjectPickup）及 Canvas 子节点。
/// </summary>
public sealed class NumObjectSpawner : MonoBehaviour
{
    [Header("数字物体 Sprites")]
    [SerializeField] private Sprite metaSprite;
    [SerializeField] private Sprite woodSprite;

    [Header("预制体选项（可选）")]
    [Tooltip("如果设置了 prefab，直接实例化；否则运行时构造")]
    [SerializeField] private GameObject numObjectPrefab;

    [Header("移动音效")]
    [SerializeField] private AudioClip metalMoveLoop;
    [SerializeField] private AudioClip woodMoveLoop;

    /// <summary>
    /// 在指定位置生成一个数字物体，初始化数字值和 Sprite，并施加初速度。
    /// </summary>
    /// <param name="number">数字值（0-10）</param>
    /// <param name="sprite">显示的 Sprite（覆盖 meta/wood 默认规则）</param>
    /// <param name="position">生成位置</param>
    /// <param name="velocity">初始速度（抛出方向和力度）</param>
    /// <returns>生成的 GameObject</returns>
    public GameObject Spawn(int number, Sprite sprite, Vector3 position, Vector2 velocity)
    {
        GameObject obj;

        if (numObjectPrefab != null)
        {
            // 使用预制体
            obj = Instantiate(numObjectPrefab, position, Quaternion.identity);
        }
        else
        {
            // 运行时构造
            obj = CreateNumObjectFromScratch(position);
        }

        // 设置数字和 Sprite
        NumObject numObj = obj.GetComponent<NumObject>();
        if (numObj != null)
        {
            numObj.SetNumber(number);
        }

        NumObjectInfo info = obj.GetComponent<NumObjectInfo>();
        if (info != null && sprite != null)
        {
            info.SetInfo(number, sprite);
        }

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null && sprite != null)
        {
            sr.sprite = sprite;
        }

        // 施加初速度
        Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = velocity;
        }

        return obj;
    }

    /// <summary>
    /// 从零构造一个完整的数字物体 GameObject（meta 结构）。
    /// </summary>
    private GameObject CreateNumObjectFromScratch(Vector3 position)
    {
        GameObject obj = new GameObject("NumObject_Runtime");
        obj.transform.position = position;

        // 1. SpriteRenderer
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = metaSprite;

        // 2. Rigidbody2D
        Rigidbody2D body = obj.AddComponent<Rigidbody2D>();
        body.mass = 1f;
        body.gravityScale = 1f;
        body.freezeRotation = false;

        // 3. 物理 BoxCollider2D（非触发）
        BoxCollider2D physicsCol = obj.AddComponent<BoxCollider2D>();
        physicsCol.size = Vector2.one;
        physicsCol.isTrigger = false;

        // 4. 触发 BoxCollider2D（拾取用）
        BoxCollider2D triggerCol = obj.AddComponent<BoxCollider2D>();
        triggerCol.size = Vector2.one * 2f;
        triggerCol.isTrigger = true;

        // 5. NumObject 脚本
        NumObject numObj = obj.AddComponent<NumObject>();
        numObj.MetaSprite = metaSprite;
        numObj.WoodSprite = woodSprite;

        NumObjectMovementAudio movementAudio = obj.AddComponent<NumObjectMovementAudio>();
        movementAudio.Setup(metalMoveLoop, woodMoveLoop);

        // 6. NumObjectPickup
        // NumObjectInfo is added automatically by NumObject's RequireComponent.
        obj.AddComponent<NumObjectPickup>();

        // 7. Canvas 子节点（显示数字 UI）
        CreateCanvasChild(obj);

        return obj;
    }

    /// <summary>
    /// 为数字物体创建 Canvas 子节点，包含 numText（显示数字）。
    /// </summary>
    private void CreateCanvasChild(GameObject parent)
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(parent.transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1f, 1f);

        // numText 子节点
        GameObject textObj = new GameObject("numText");
        textObj.transform.SetParent(canvasObj.transform, false);

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = "1";

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
    }
}
