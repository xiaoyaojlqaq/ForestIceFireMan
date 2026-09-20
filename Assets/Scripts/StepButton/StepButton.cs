using UnityEngine;
using UnityEngine.Events;

public class Button2D : MonoBehaviour
{
    [SerializeField] private Collider2D buttonCollider;
    [SerializeField] private float rayLength = 0.15f;
    [SerializeField] private LayerMask playerLayer;

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
            onPressed?.Invoke();
        else
            onReleased?.Invoke();
    }

    private bool CheckPlayer()
    {
        Bounds bounds = buttonCollider.bounds;

        Vector2 left = new Vector2(bounds.min.x, bounds.max.y);
        Vector2 right = new Vector2(bounds.max.x, bounds.max.y);

        RaycastHit2D hit = Physics2D.Linecast(
            left + Vector2.up * rayLength,
            right + Vector2.up * rayLength,
            playerLayer
        );

        return hit.collider != null;
    }
}