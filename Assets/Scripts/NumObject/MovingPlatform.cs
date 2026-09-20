using System.Collections;
using UnityEngine;

/// <summary>
/// 移动平台：执行 MoveToTarget() 后平滑移动到 targetPosition（世界坐标）。
/// MoveToTarget 为无参公开函数，可直接在 Inspector 的 UnityEvent 中配置
/// （例如 NumberSocket.onMatched），也可在代码中调用。
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("目标位置（世界坐标）")]
    [Tooltip("平台最终移动到的世界坐标位置")]
    public Vector3 targetPosition = Vector3.zero;

    [Header("移动设置")]
    [Tooltip("移动速度（单位/秒）")]
    public float moveSpeed = 3f;

    private Coroutine moveRoutine;

    /// <summary>
    /// 开始向 targetPosition 平滑移动；移动中再次调用会从当前位置重启。
    /// </summary>
    public void MoveToTarget()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        // 速度非法（<=0）时直接传送到位，避免协程死循环
        if (moveSpeed <= 0f)
        {
            transform.position = targetPosition;
            moveRoutine = null;
            Debug.Log($"[MovingPlatform] 速度无效，已直接传送至 {targetPosition}");
            return;
        }

        moveRoutine = StartCoroutine(MoveRoutine(targetPosition));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        moveRoutine = null;
        Debug.Log($"[MovingPlatform] 已移动到 {target}");
    }

    // 编辑器中选中平台时可视化目标位置与路径，便于关卡摆放
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, targetPosition);

        Vector3 size = Vector3.one;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            size = (Vector3)sr.sprite.bounds.size + Vector3.forward;
        Gizmos.DrawWireCube(targetPosition, size);
    }
}
