using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NumDoor : MonoBehaviour
{
    public enum DoorType
    {
        Multiply,  // ×2 门
        Divide     // ÷2 门
    }

    [Header("门的类型")]
    public DoorType doorType = DoorType.Multiply;

    

private System.Collections.Generic.List<GameObject> nearbyObjects = new System.Collections.Generic.List<GameObject>();
    private GameObject lastProcessedObject = null;

    [Header("Collider 设置")]
    [Tooltip("用作触发器的 BoxCollider2D（检测进入）")]
    public BoxCollider2D triggerCollider;
    [Tooltip("用作物理阻挡的 BoxCollider2D（÷2门阻挡用）")]
    public BoxCollider2D blockCollider;

    private void Awake()
    {
        // 自动获取两个 BoxCollider2D
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        
        if (colliders.Length >= 2)
        {
            // 第一个作为触发器
            if (triggerCollider == null)
                triggerCollider = colliders[0];
            
            // 第二个作为阻挡器
            if (blockCollider == null)
                blockCollider = colliders[1];
            
            // 确保正确设置
            triggerCollider.isTrigger = true;
            
            // 根据门类型初始化阻挡器状态
            if (doorType == DoorType.Multiply)
            {
                // ×2 门：阻挡器设为触发器（不阻挡）
                blockCollider.isTrigger = true;
            }
            else
            {
                // ÷2 门：阻挡器初始关闭（等待检测后决定）
                blockCollider.isTrigger = true;
            }
            
            Debug.Log($"[NumDoor] 已配置 Trigger: {triggerCollider.name}, Block: {blockCollider.name}");
        }
        else
        {
            Debug.LogError($"[NumDoor] {gameObject.name} 需要至少 2 个 BoxCollider2D！");
        }
    }

    

private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是数字物体
        if (other.GetComponent<NumObject>() != null || other.GetComponent<ColorNumObject>() != null)
        {
            if (!nearbyObjects.Contains(other.gameObject))
            {
                nearbyObjects.Add(other.gameObject);
                Debug.Log($"[NumDoor] {other.gameObject.name} 进入触发区域，当前附近物体数: {nearbyObjects.Count}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (nearbyObjects.Contains(other.gameObject))
        {
            nearbyObjects.Remove(other.gameObject);
            Debug.Log($"[NumDoor] {other.gameObject.name} 离开触发区域，当前附近物体数: {nearbyObjects.Count}");
        }
    }

private void Update()
    {
        // 清理已销毁的物体
        nearbyObjects.RemoveAll(obj => obj == null);

        if (nearbyObjects.Count == 0)
        {
            lastProcessedObject = null;
            
            // ÷2 门：没有物体时，关闭阻挡器（允许通过）
            if (doorType == DoorType.Divide && blockCollider != null)
            {
                blockCollider.isTrigger = true;
            }
            
            return;
        }

        // 找到距离最近的物体
        GameObject closestObject = GetClosestObject();

        // 如果最近的物体发生了变化，处理新的最近物体
        if (closestObject != lastProcessedObject)
        {
            lastProcessedObject = closestObject;
            ProcessObject(closestObject);
        }
    }

    private GameObject GetClosestObject()
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in nearbyObjects)
        {
            if (obj == null) continue;

            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

    private void ProcessObject(GameObject obj)
    {
        if (obj == null) return;

        // 尝试获取 NumObject
        NumObject numObj = obj.GetComponent<NumObject>();
        if (numObj != null)
        {
            ProcessNumObject(numObj, obj.GetComponent<Collider2D>());
            return;
        }

        // 尝试获取 ColorNumObject
        ColorNumObject colorNumObj = obj.GetComponent<ColorNumObject>();
        if (colorNumObj != null)
        {
            ProcessColorNumObject(colorNumObj, obj.GetComponent<Collider2D>());
            return;
        }
    }

private void ProcessNumObject(NumObject numObj, Collider2D collider)
    {
        int currentNumber = numObj.number;

        switch (doorType)
        {
            case DoorType.Multiply:
                // ×2 门：直接翻倍
                numObj.SetNumber(currentNumber * 2);
                Debug.Log($"[NumDoor-×2] NumObject 数字 {currentNumber} → {numObj.number}");
                break;

            case DoorType.Divide:
                // ÷2 门：检查能否整除
                if (currentNumber % 2 == 0)
                {
                    // 可以整除，允许通过
                    if (blockCollider != null)
                        blockCollider.isTrigger = true;
                    
                    numObj.SetNumber(currentNumber / 2);
                    Debug.Log($"[NumDoor-÷2] NumObject 数字 {currentNumber} → {numObj.number}，允许通过");
                }
                else
                {
                    // 不能整除，启用物理阻挡
                    if (blockCollider != null)
                        blockCollider.isTrigger = false;
                    
                    Debug.Log($"[NumDoor-÷2] NumObject 数字 {currentNumber} 不能被 2 整除，阻挡");
                }
                break;
        }
    }

private void ProcessColorNumObject(ColorNumObject colorNumObj, Collider2D collider)
    {
        int currentNumber = colorNumObj.number;

        switch (doorType)
        {
            case DoorType.Multiply:
                // ×2 门：直接翻倍
                colorNumObj.SetNumber(currentNumber * 2);
                Debug.Log($"[NumDoor-×2] ColorNumObject 数字 {currentNumber} → {colorNumObj.number}");
                break;

            case DoorType.Divide:
                // ÷2 门：检查能否整除
                if (currentNumber % 2 == 0)
                {
                    // 可以整除，允许通过
                    if (blockCollider != null)
                        blockCollider.isTrigger = true;
                    
                    colorNumObj.SetNumber(currentNumber / 2);
                    Debug.Log($"[NumDoor-÷2] ColorNumObject 数字 {currentNumber} → {colorNumObj.number}，允许通过");
                }
                else
                {
                    // 不能整除，启用物理阻挡
                    if (blockCollider != null)
                        blockCollider.isTrigger = false;
                    
                    Debug.Log($"[NumDoor-÷2] ColorNumObject 数字 {currentNumber} 不能被 2 整除，阻挡");
                }
                break;
        }
    }

    /// <summary>
    /// 阻挡物体（仅用于÷2门）
    /// </summary>

}
