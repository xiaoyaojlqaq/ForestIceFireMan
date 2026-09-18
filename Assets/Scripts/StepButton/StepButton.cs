using UnityEngine;
using UnityEngine.Events;

public class Button2D : MonoBehaviour
{
    [SerializeField] private Collider2D buttonCollider;
    [SerializeField] private float rayLength = 0.15f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private bool isPressed;

    private void Update()
    {
        bool pressed = CheckPlayer();

        if (pressed == isPressed)
            return;

        isPressed = pressed;

        if (pressed)
        {
            spriteRenderer.sprite = pressedSprite;
            onPressed?.Invoke();
        }
        else
        {
            spriteRenderer.sprite = unpressedSprite;
            onReleased?.Invoke();
        }
    }

    private bool CheckPlayer()
    {
        BoxCollider2D box = buttonCollider as BoxCollider2D;

        if (box == null)
            return false;

        Vector2 center = box.transform.TransformPoint(box.offset);
        Vector2 up = box.transform.up;

        float halfHeight = box.size.y * 0.5f * box.transform.lossyScale.y;

        Vector2 checkCenter = center + up * (halfHeight + rayLength);

        Collider2D hit = Physics2D.OverlapBox(
            checkCenter,
            new Vector2(
                box.size.x * box.transform.lossyScale.x,
                0.1f
            ),
            box.transform.eulerAngles.z,
            playerLayer
        );

        return hit != null;
    }
}