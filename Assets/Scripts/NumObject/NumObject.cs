using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumObject : MonoBehaviour
{
    public int number;

    Rigidbody2D rigidbody;

    SpriteRenderer spriteRenderer;

    public Sprite MetaSprite;
    public Sprite WoodSprite;
    private Text numText;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer= GetComponent<SpriteRenderer>();
        numText=transform.GetChild(0).GetComponentInChildren<Text>();
        SetNumber(1);
    }

    public void SetNumber(int num)
    {
        number = Mathf.Clamp(num, 0, 10);
        UpdateSprite();
        UpdateMass();
        UpdateNumText();
    }

    void UpdateSprite()
    {
        if (number > 5)
        {
            spriteRenderer.sprite = MetaSprite;

        }
        else
        {
            spriteRenderer.sprite = WoodSprite;
        }
    }
    
    void UpdateMass()
    {
        rigidbody.mass = number;
    }

    void UpdateNumText()
    {
        numText.text = number.ToString();
    }

}
