using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Liquid : MonoBehaviour
{

    [Header("Appearance")]
    [SerializeField]
    private Color bodyColor = Color.cyan;

    [SerializeField]
    private Color surfaceColor = Color.white;

    [Header("Wave")]
    [SerializeField]
    private float waveAmplitude = 0.1f;

    [SerializeField]
    private float waveFrequency = 2f;

    [SerializeField]
    private float waveSpeed = 2f;

    [Header("Flow")]
    [SerializeField]
    private float flowSpeed = 1f;

    [Header("Size")]
    [SerializeField]
    private float width = 5f;

    [Header("Liquid Level")]
    [SerializeField, Range(0f, 1f)]
    private float level = 1f;

    [SerializeField]
    private float levelChangeDuration = 1f;

    [Header("Filling")]
    [SerializeField, Range(0f, 1f)]
    private float targetLevel = 1f;

    [SerializeField]
    private float fillSpeed = 0.2f;

    private bool filling;
    private float levelStart;
    private float levelChangeTime;
    private bool changingLevel;

    [SerializeField]
    private float maxHeight = 2f;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    private MeshRenderer surfaceRenderer;

    [SerializeField]
    private LiquidSurface surface;

    public Color BodyColor => bodyColor;
    public Color SurfaceColor => surfaceColor;

    public float WaveAmplitude => waveAmplitude;
    public float WaveFrequency => waveFrequency;
    public float WaveSpeed => waveSpeed;

    public float FlowSpeed => flowSpeed;

    public float Width => width;
    public float MaxHeight => maxHeight;
    public float Level => level;

    private float CurrentHeight => maxHeight * level;

    [Header("Rendering")]
    [SerializeField] private int surfaceSortingOrder = 10;

    [SerializeField] private BoxCollider2D liquidTrigger;

    private void Awake()
    {
        ApplyParameters();
    }

    private void Update()
    {
        if (!filling)
            return;

        if (level >= targetLevel)
            return;

        level += fillSpeed * Time.deltaTime;
        level = Mathf.Min(level, targetLevel);

        UpdateLiquidLevel();
    }

    [ContextMenu("ApplyParameters")]
    private void ApplyParameters()
    {
        float currentHeight = maxHeight * level;

        // Body 颜色
        bodyRenderer.color = bodyColor;

        // Body 尺寸
        bodyRenderer.transform.localScale = new Vector3(
            width,
            currentHeight,
            1f
        );

        // Body 保持底部不动
        bodyRenderer.transform.localPosition = new Vector3(
            0f,
            currentHeight / 2f,
            0f
        );

        // 水面放到当前液面
        surface.transform.localPosition = new Vector3(
            0f,
            currentHeight + 0.2f,
            0f
        );

        // 水面宽度
        surface.ApplyParameters(
            width,
            waveAmplitude,
            waveFrequency,
            waveSpeed
        );

        surface.SetColor(surfaceColor);
        surface.SetSortingOrder(surfaceSortingOrder);

        // Trigger
        liquidTrigger.size = new Vector2(
            width,
            currentHeight
        );

        liquidTrigger.offset = new Vector2(
            0f,
            currentHeight / 2f
        );
    }

    private void UpdateLiquidLevel()
    {
        float currentHeight = maxHeight * level;

        // Body
        bodyRenderer.transform.localScale = new Vector3(
            width,
            currentHeight,
            1f
        );

        bodyRenderer.transform.localPosition = new Vector3(
            0f,
            currentHeight / 2f,
            0f
        );

        // Surface
        surface.transform.localPosition = new Vector3(
            0f,
            currentHeight + 0.2f,
            0f
        );

        // Trigger
        liquidTrigger.size = new Vector2(
            width,
            currentHeight
        );

        liquidTrigger.offset = new Vector2(
            0f,
            currentHeight / 2f
        );
    }

    public void SetLevel(float value)
    {
        SetLevel(value, levelChangeDuration);
    }

    public void SetLevel(float value, float duration)
    {
        targetLevel = Mathf.Clamp01(value);

        if (duration <= 0f)
        {
            level = targetLevel;
            changingLevel = false;
            ApplyParameters();
            return;
        }

        levelStart = level;
        levelChangeTime = 0f;
        levelChangeDuration = duration;
        changingLevel = true;
    }

    [ContextMenu("StartFilling")]
    public void StartFilling()
    {
        filling = true;
    }

    [ContextMenu("StopFilling")]
    public void StopFilling()
    {
        filling = false;
    }
}
