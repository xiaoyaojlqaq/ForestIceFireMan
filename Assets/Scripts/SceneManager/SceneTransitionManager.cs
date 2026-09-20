using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private float fadeDuration = 0.3f;

    private Canvas canvas;
    private Image fadeImage;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateFadeUI();
    }

    private void CreateFadeUI()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        fadeImage = imageObject.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public void LoadScene(string sceneName,UnityAction playerPosChange)
    {
        if (isTransitioning)
            return;

        StartCoroutine(LoadSceneCoroutine(sceneName, playerPosChange));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, UnityAction playerPosChange)
    {
        isTransitioning = true;

        // 淡出
        yield return Fade(0f, 1f);

        // 切换场景
        SceneManager.LoadScene(sceneName);

        // 等一帧，让新场景完成初始化
        yield return null;

        // 淡入
        yield return Fade(1f, 0f);
        playerPosChange();
        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(time / fadeDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(from, to, t);
            fadeImage.color = color;

            yield return null;
        }

        Color finalColor = fadeImage.color;
        finalColor.a = to;
        fadeImage.color = finalColor;

    }
}