using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // 把所有面板拖进这个列表里
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject resultPanel;

    private void Awake()
    {
        Instance = this;
    }

    // 打开面板的通用方法
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true); 
        // 这里可以加上之前的动画代码：panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    // 关闭面板的通用方法
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        // 这里可以加上关闭动画
    }
}