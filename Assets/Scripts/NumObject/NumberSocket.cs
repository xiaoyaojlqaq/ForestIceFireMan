using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 数字插槽：自带一个数字并显示在物体中间。
/// 当 NumObject / ColorNumObject 进入触发区域且数字与插槽数字相等时，
/// 执行 onMatched 事件（可在 Inspector 中配置）并销毁该数字物体。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NumberSocket : MonoBehaviour
{
    public int number = 1;

    [Header("匹配成功事件（可配置）")]
    public UnityEvent onMatched = new UnityEvent();

    // 触发去重：数字物体可能带两个 BoxCollider2D，防止对同一插槽重复响应
    private readonly HashSet<int> _activeTriggers = new HashSet<int>();

    private Text numText;

    private void Start()
    {
        FindNumText();
        SetNumber(number);
    }

#if UNITY_EDITOR
    // 编辑器中修改 number 时同步刷新显示，便于摆放关卡时直接看到插槽数字
    private void OnValidate()
    {
        if (numText == null)
            FindNumText();

        if (numText != null)
            numText.text = Mathf.Clamp(number, 0, 100).ToString();
    }
#endif

    public void SetNumber(int num)
    {
        number = Mathf.Clamp(num, 0, 100);

        if (numText != null)
            numText.text = number.ToString();
    }

    private void FindNumText()
    {
        if (transform.childCount > 0)
        {
            Transform canvas = transform.GetChild(0);
            if (canvas != null && canvas.childCount > 0)
                numText = canvas.GetChild(0).GetComponent<Text>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // HashSet.Add 返回 false 说明该 Collider 已处理过（另一个 BoxCollider2D 先一步进入）
        if (!_activeTriggers.Add(other.GetInstanceID()))
            return;

        NumObject numObj = other.GetComponent<NumObject>();
        if (numObj != null)
        {
            TryMatch(numObj.number, other.gameObject);
            return;
        }

        ColorNumObject colorNumObj = other.GetComponent<ColorNumObject>();
        if (colorNumObj != null)
            TryMatch(colorNumObj.number, other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _activeTriggers.Remove(other.GetInstanceID());
    }

    private void TryMatch(int incomingNumber, GameObject target)
    {
        if (incomingNumber != number)
        {
            Debug.Log($"[NumberSocket] {target.name} 数字 {incomingNumber} ≠ {number}，不匹配");
            return;
        }

        onMatched?.Invoke();
        Destroy(target);
        Debug.Log($"[NumberSocket] {target.name} 数字 {number} 匹配成功，执行事件并销毁");
    }
}
