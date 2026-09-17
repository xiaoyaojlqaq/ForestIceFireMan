using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Liquid : MonoBehaviour
{
    [Header("Liquid")]
    [SerializeField]
    private LiquidType liquidType = LiquidType.Water;

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

    [SerializeField]
    private float height = 2f;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    private MeshRenderer surfaceRenderer;

    [SerializeField]
    private LiquidSurface surface;

    public LiquidType LiquidType => liquidType;

    public Color BodyColor => bodyColor;
    public Color SurfaceColor => surfaceColor;

    public float WaveAmplitude => waveAmplitude;
    public float WaveFrequency => waveFrequency;
    public float WaveSpeed => waveSpeed;

    public float FlowSpeed => flowSpeed;

    public float Width => width;
    public float Height => height;

    [Header("Rendering")]
    [SerializeField] private int surfaceSortingOrder = 10;

    private void Awake()
    {
        ApplyParameters();
    }

    [ContextMenu("ApplyParameters")]
    private void ApplyParameters()
    {
        bodyRenderer.color = bodyColor;

        bodyRenderer.transform.localScale = new Vector3(
            width,
            height,
            1f
        );

        surface.transform.localPosition = new Vector3(
            0f,
            height / 2f + 0.2f,
            0f
        );

        surface.ApplyParameters(
            width,
            waveAmplitude,
            waveFrequency,
            waveSpeed
        );

        surface.SetColor(surfaceColor);
        surface.SetSortingOrder(surfaceSortingOrder);
    }
}
