using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LiquidSurface : MonoBehaviour
{
    [Header("Wave")]
    private float amplitude = 0.1f;
    private float frequency = 1f;
    private float speed = 1f;

    [Header("Mesh")]
    private float width = 5f;
    [SerializeField] private float height = 0.2f;
    [SerializeField] private int segments = 32;

    [SerializeField]
    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        CreateMesh();
    }

    private void Update()
    {
        UpdateWave();
    }

    public void ApplyParameters(
    float width,
    float amplitude,
    float frequency,
    float speed)
    {
        this.width = width;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.speed = speed;

        CreateMesh();
    }

    private void CreateMesh()
    {
        mesh = new Mesh();
        mesh.name = "LiquidSurfaceMesh";
        mesh.MarkDynamic();

        vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float x = Mathf.Lerp(-width / 2f, width / 2f, t);

            // 上面
            vertices[i] = new Vector3(x, 0f, 0f);

            // 下面
            vertices[segments + 1 + i] =
                new Vector3(x, -height, 0f);
        }

        for (int i = 0; i < segments; i++)
        {
            int topLeft = i;
            int topRight = i + 1;

            int bottomLeft = segments + 1 + i;
            int bottomRight = segments + 1 + i + 1;

            int index = i * 6;

            triangles[index + 0] = topLeft;
            triangles[index + 1] = topRight;
            triangles[index + 2] = bottomLeft;

            triangles[index + 3] = topRight;
            triangles[index + 4] = bottomRight;
            triangles[index + 5] = bottomLeft;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void UpdateWave()
    {
        float time = Time.time;

        for (int i = 0; i <= segments; i++)
        {
            float x = vertices[i].x;

            float y =
                amplitude *
                Mathf.Sin(
                    x * frequency +
                    time * speed
                );

            vertices[i].y = y;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    public void SetColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        meshRenderer.sortingOrder = sortingOrder;
    }
}
