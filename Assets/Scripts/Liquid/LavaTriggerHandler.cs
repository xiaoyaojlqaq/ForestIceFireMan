using UnityEngine;

/// <summary>
/// 挂载在 Lava/LiquidTrigger 上。
/// 任何碰撞体进入 lava 触发区时：
///   - Player        → 扣除 1 点生命值（PlayerHealth.TakeDamage）
///   - NumObject     → number < 10 时销毁
///   - ColorNumObject → Blue 销毁；Red 的 number +1
/// 
/// 防重复触发：使用 HashSet 记录已处理的 GameObject，
/// 避免同一物体因多个 Collider 而触发多次。
/// </summary>
[DisallowMultipleComponent]
public sealed class LavaTriggerHandler : MonoBehaviour
{
    [Header("触发设置")]
    [Tooltip("玩家的 Tag")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("每次进入 lava 扣除的生命值")]
    [SerializeField, Min(1)] private int damageAmount = 1;

    // 防重复触发：记录已经处理过的物体 InstanceID
    // 当物体完全离开触发区后清除记录，允许再次进入时触发
    private readonly System.Collections.Generic.HashSet<int> processedObjects 
        = new System.Collections.Generic.HashSet<int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!other.CompareTag("Player")) return;
        GameObject go = other.gameObject;
        int instanceID = go.GetInstanceID();

        // 检查是否已经处理过该物体（防止多个 Collider 重复触发）
        if (!processedObjects.Add(instanceID))
        {
            // 已处理过，跳过
            return;
        }

        // ── 1. 玩家 ──────────────────────────────────────────────────────────
        if (go.CompareTag(playerTag))
        {
            PlayerHealth health = go.GetComponent<PlayerHealth>();
            BurningState burningState = go.GetComponent<BurningState>();
            if (health != null)
            {
                burningState.StartBurning();
                health.TakeDamage(damageAmount);
                Debug.Log($"[LavaTriggerHandler] 玩家触碰 Lava，扣除 {damageAmount} 点生命，剩余 {health.CurrentHealth}/{health.MaxHealth}");
            }
            return;
        }

        // ── 2. ColorNumObject ─────────────────────────────────────────────────
        ColorNumObject colorNum = go.GetComponentInParent<ColorNumObject>();
        if (colorNum == null)
            colorNum = go.GetComponent<ColorNumObject>();

        if (colorNum != null)
        {
            if (colorNum.colorType == ColorType.Blue)
            {
                colorNum.GetComponent<BurningState>().StartBurning();
                Debug.Log($"[LavaTriggerHandler] Blue ColorNumObject({colorNum.number}) 进入 Lava，销毁。");
                Destroy(colorNum.gameObject,1f);
            }
            else if (colorNum.colorType == ColorType.Red)
            {
                BurningState burningState = colorNum.GetComponent<BurningState>();
                burningState.StartBurning();
                colorNum.SetNumber(colorNum.number + 1);

                Debug.Log($"[LavaTriggerHandler] Red ColorNumObject 进入 Lava，number +1 → {colorNum.number}");
            }
            return;
        }

        // ── 3. NumObject ──────────────────────────────────────────────────────
        NumObject numObj = go.GetComponentInParent<NumObject>();
        if (numObj == null)
            numObj = go.GetComponent<NumObject>();

        if (numObj != null)
        {
            if (numObj.number < 10)
            {
                BurningState burningState = numObj.GetComponent<BurningState>();
                burningState.StartBurning();
                Debug.Log($"[LavaTriggerHandler] NumObject(number={numObj.number}) 进入 Lava，number < 10，销毁。");
                Destroy(numObj.gameObject,1f);
            }
            else
            {
                Debug.Log($"[LavaTriggerHandler] NumObject(number={numObj.number}) 进入 Lava，number >= 10，保留。");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (!other.CompareTag("Player")) return;
        GameObject go = other.gameObject;
        int instanceID = go.GetInstanceID();
        go.GetComponent<BurningState>().StopBurning();
        // 物体完全离开触发区后，清除记录，允许再次进入时重新触发
        processedObjects.Remove(instanceID);
    }
}
