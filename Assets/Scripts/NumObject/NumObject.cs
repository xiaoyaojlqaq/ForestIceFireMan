using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(NumObjectInfo))]
public class NumObject : MonoBehaviour
{
    public int number = 1;

    [Header("Number Sprites")]
    public Sprite MetaSprite;
    public Sprite WoodSprite;

    private Rigidbody2D body;
    private float mass;
    private float drag;
    private float gravityScale;
    private SpriteRenderer spriteRenderer;
    private NumObjectInfo info;
    private Text numText;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        info = GetComponent<NumObjectInfo>();

        if (transform.childCount > 0)
        {
            Transform canvas = transform.GetChild(0);

            if (canvas.childCount > 0)
            {
                numText = canvas.GetChild(0).GetComponent<Text>();
            }
        }
    }

    private void Start()
    {
        SetNumber(number);
        ReadNumbObjectData();
    }

    public void SetNumber(int num)
    {
        number = Mathf.Clamp(num, 0, 100);
        UpdateSprite();
        UpdateMass();
        UpdateNumText();
        UpdateInfo();

        // 数字等于 0 时延迟 1 秒摧毁
        if (number == 0)
        {
            Destroy(gameObject, 1f);
        }
    }
    public void ReadNumbObjectData()
    {
        NumbDataReader.ReadNumbDataUseEPPlus(out mass, out drag, out gravityScale);
        body.mass = mass;
        body.drag = drag;
        body.gravityScale = gravityScale;
    }
    private void UpdateSprite()
    {
        spriteRenderer.sprite = number > 5 ? MetaSprite : WoodSprite;
    }

    private void UpdateMass()
    {
        body.mass = Mathf.Max(number, 0.01f);
    }

    private void UpdateNumText()
    {
        if (numText != null)
        {
            numText.text = number.ToString();
        }
    }

    private void UpdateInfo()
    {
        info.SetInfo(number, spriteRenderer.sprite);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 数字 <= 5 时碰到 fire 标签物体，1 秒后摧毁
        if (number <= 5 && other.CompareTag("fire"))
        {
            Destroy(gameObject, 1f);
        }
    }
}
