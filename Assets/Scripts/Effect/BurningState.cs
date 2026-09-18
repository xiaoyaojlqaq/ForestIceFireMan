using UnityEngine;

/// <summary>
/// 燃烧特效控制器 - 管理火焰特效的激活与关闭
/// </summary>
public class BurningState : MonoBehaviour
{
    [Tooltip("燃烧特效预制体")]
    public GameObject fireEffectPrefab;

    [Tooltip("特效相对于物体的偏移位置")]
    public Vector3 effectOffset = Vector3.zero;

    private GameObject _currentFireEffect;

    /// <summary>
    /// 激活燃烧特效
    /// </summary>
    public void StartBurning()
    {
        if (_currentFireEffect != null) return;

        if (fireEffectPrefab == null)
        {
            Debug.LogError($"[BurningState] {gameObject.name}：未设置 FireEffect 预制体！");
            return;
        }

        _currentFireEffect = Instantiate(fireEffectPrefab, transform.position + effectOffset, Quaternion.identity, transform);
    }

    /// <summary>
    /// 关闭燃烧特效
    /// </summary>
    public void StopBurning()
    {
        if (_currentFireEffect == null) return;

        Destroy(_currentFireEffect);
        _currentFireEffect = null;
    }

    void OnDestroy()
    {
        StopBurning();
    }
}
