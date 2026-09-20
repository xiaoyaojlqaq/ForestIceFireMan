using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        // 这里可以加上动画
    }

    // 关闭面板的通用方法
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        // 这里可以加上关闭动画
    }
    public void DebugText(string text)
    {
    if (string.IsNullOrWhiteSpace(text))
    {
        Debug.Log("暂无作用");
    }
    else
    {
        Debug.Log(text);
    }
    }

   
    public void QuitGame()
    {
        Debug.Log("退出游戏");

#if UNITY_EDITOR
       UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}