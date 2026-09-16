using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public sealed class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField, Min(0f)] private float jumpSpeed = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField, Min(0.001f)] private float groundCheckDistance = 0.08f;

    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];

    private Rigidbody2D body;
    private float mass;
    private float drag;
    private float gravityScale;

    private Collider2D bodyCollider;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private bool jumpRequested;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Keep the character upright while it is driven by 2D physics.
        body.freezeRotation = true;
    }

    private void Start()
    {
        ReadPlayerData();
    }
    private void Update()
    {
        bool moveLeft = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);
        horizontalInput = (moveRight ? 1f : 0f) - (moveLeft ? 1f : 0f);

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpRequested = true;
        }

        if (spriteRenderer != null && horizontalInput != 0f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = body.velocity;
        velocity.x = horizontalInput * moveSpeed;

        bool shouldJump = jumpRequested;
        jumpRequested = false;

        if (shouldJump && IsGrounded())
        {
            velocity.y = jumpSpeed;
        }

        body.velocity = velocity;
    }


    void ReadPlayerData()
    {
        PlayerDataConfigTable.ReadDataUseEPPlus(out mass, out drag, out gravityScale, out moveSpeed, out jumpSpeed);
        body.mass = mass;
        body.drag = drag;
        body.gravityScale = gravityScale;
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
            {
                return true;
            }
        }

        return false;
    }
}
