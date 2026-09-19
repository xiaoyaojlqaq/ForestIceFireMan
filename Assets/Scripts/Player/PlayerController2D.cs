using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(AudioSource))]
public sealed class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float jumpSpeed = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField, Min(0.001f)] private float groundCheckDistance = 0.08f;

    [Header("抛出设置")]
    [Tooltip("普通数字物体预制体（NumObject）- 仅在源物体缺失时使用")]
    [SerializeField] private GameObject numObjectPrefab;
    [Tooltip("彩色数字物体预制体（ColorNumObject）- 仅在源物体缺失时使用")]
    [SerializeField] private GameObject colorNumObjectPrefab;
    [Tooltip("水平抛出速度")]
    [SerializeField, Min(0f)] private float throwSpeedH = 1f;
    [Tooltip("竖直抛出速度")]
    [SerializeField, Min(0f)] private float throwSpeedV = 1f;
    [Tooltip("生成时离玩家的偏移（本地坐标）")]
    [SerializeField] private Vector2 throwOffset = new Vector2(0.8f, 0.3f);

    [Header("交互音效")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip releaseSound;

    [Header("运动音效")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];

    private Rigidbody2D body;
    private AudioSource actionAudio;
    private float mass;
    private float drag;
    private float gravityScale;

    private Collider2D bodyCollider;
    private SpriteRenderer spriteRenderer;
    private Inventory inventory;
    private float horizontalInput;
    private bool jumpRequested;
    private bool hasGroundState;
    private bool wasGrounded;


    private GameObject BagCanvas;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        actionAudio = GetComponent<AudioSource>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventory = GetComponent<Inventory>();
        BagCanvas=transform.GetChild(0).gameObject;
        // Keep the character upright while it is driven by 2D physics.
        body.freezeRotation = true;
        actionAudio.playOnAwake = false;
        actionAudio.spatialBlend = 0f;
        actionAudio.Stop();
    }

    private void Start()
    {
        ReadPlayerData();
    }
    private void Update()
    {
        bool moveLeft  = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);
        horizontalInput = (moveRight ? 1f : 0f) - (moveLeft ? 1f : 0f);

        if (Input.GetKeyDown(KeyCode.W))
            jumpRequested = true;

        if (spriteRenderer != null && horizontalInput != 0f)
            spriteRenderer.flipX = horizontalInput < 0f;

        if (Input.GetKeyDown(KeyCode.F))
            TryPickupNearby();

        // 按键 1/2/3 抛出对应背包槽
        if (Input.GetKeyDown(KeyCode.Alpha1)) ThrowSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ThrowSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ThrowSlot(2);
    }

    private void FixedUpdate()
    {
        Vector2 velocity = body.velocity;
        velocity.x = horizontalInput * moveSpeed;

        bool shouldJump = jumpRequested;
        jumpRequested = false;

        bool isGrounded = IsGrounded();
        if (hasGroundState && !wasGrounded && isGrounded)
            PlayActionSound(landSound);

        wasGrounded = isGrounded;
        hasGroundState = true;

        if (shouldJump && isGrounded)
        {
            velocity.y = jumpSpeed;
            PlayActionSound(jumpSound);
        }

        body.velocity = velocity;
    }


    public void ReadPlayerData()
    {
        PlayerDataConfigTable.ReadDataUseEPPlus(out mass, out drag, out gravityScale, out moveSpeed, out jumpSpeed);
        body.mass = mass;
        body.drag = drag;
        body.gravityScale = gravityScale;
    }

    /// <summary>
    /// 从背包取出指定槽位的物体并向前抛出。
    /// 优先激活原始物体（拾取时隐藏的），如果原始物体不存在则回退到实例化预制体。
    /// </summary>
    private void ThrowSlot(int slotIndex)
    {
        if (inventory == null) return;

        InventoryItem item = inventory.GetItem(slotIndex);
        if (item == null) return;

        // 移除背包槽位
        inventory.RemoveAt(slotIndex);

        // 计算朝向和生成位置
        float dir = (spriteRenderer != null && spriteRenderer.flipX) ? -1f : 1f;
        Vector3 spawnPos = transform.position + new Vector3(throwOffset.x * dir, throwOffset.y, 0f);

        GameObject obj = null;

        // 优先使用原始物体（拾取时隐藏的）
        if (item.SourceObject != null)
        {
            BagCanvas.SetActive(true);

            obj = item.SourceObject;
            
            // 激活物体
            obj.SetActive(true);
            
            // 移动到玩家前方
            obj.transform.position = spawnPos;
            
            Debug.Log($"[PlayerController2D] 激活并抛出原始物体: {obj.name}，槽位 {slotIndex + 1}");
        }
        else
        {
            // 原始物体丢失（例如场景重载），回退到实例化预制体
            GameObject prefabToUse;
            if (item.ObjectType == NumObjectType.ColorNumObject)
            {
                prefabToUse = colorNumObjectPrefab;
                if (prefabToUse == null)
                {
                    Debug.LogWarning("[PlayerController2D] colorNumObjectPrefab 未设置且原始物体缺失，无法抛出 ColorNumObject。");
                    return;
                }
            }
            else
            {
                prefabToUse = numObjectPrefab;
                if (prefabToUse == null)
                {
                    Debug.LogWarning("[PlayerController2D] numObjectPrefab 未设置且原始物体缺失，无法抛出 NumObject。");
                    return;
                }
            }

            obj = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
            Debug.LogWarning($"[PlayerController2D] 原始物体缺失，实例化预制体 {prefabToUse.name}，槽位 {slotIndex + 1}");
        }

        // 更新物体的数字和颜色（无论是激活的原始物体还是新实例化的）
        NumObject numObj = obj.GetComponent<NumObject>();
        if (numObj != null)
        {
            // 普通 NumObject：只需设置数字
            numObj.SetNumber(item.Number);
        }
        else
        {
            ColorNumObject colorNumObj = obj.GetComponent<ColorNumObject>();
            if (colorNumObj != null)
            {
                // Init() 同时写入 colorType 并刷新 Sprite
                if (item.ColorType.HasValue)
                    colorNumObj.Init(item.Number, item.ColorType.Value);
                else
                    colorNumObj.SetNumber(item.Number);
            }
        }

        // 施加抛出速度
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 重置速度，避免隐藏前的旧速度影响
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
            // 施加新的抛出速度
            rb.velocity = new Vector2(throwSpeedH * dir, throwSpeedV);
        }

        Debug.Log($"[PlayerController2D] 抛出槽位 {slotIndex + 1}，类型 {item.ObjectType}，数字 {item.Number}，颜色 {item.ColorType}");
        PlayActionSound(releaseSound);
    }

    /// <summary>
    /// 搜索周围所有在范围内的 NumObjectPickup，拾取第一个可拾取的物体。
    /// </summary>
    private void TryPickupNearby()
    {
        if (inventory == null) return;

        NumObjectPickup[] candidates = FindObjectsOfType<NumObjectPickup>();
        foreach (NumObjectPickup pickup in candidates)
        {
            if (pickup.IsPlayerInRange())
            {
                bool success = pickup.TryPickup(inventory);
                if (success)
                {
                    BagCanvas.SetActive(true);
                    PlayActionSound(pickupSound);
                    Debug.Log("[PlayerController2D] F 键拾取成功: " + pickup.name);
                    break;
                }
            }
        }
    }

    private void PlayActionSound(AudioClip clip)
    {
        if (clip != null)
            actionAudio.PlayOneShot(clip);
    }

    private bool IsGrounded()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayers);
        filter.useTriggers = false;

        int hitCount = bodyCollider.Cast(Vector2.down, filter, groundHits, groundCheckDistance);
        for (int i = 0; i < hitCount; i++)
        {
            if (groundHits[i].normal.y > 0.5f)
                return true;
        }
        return false;
    }
}
