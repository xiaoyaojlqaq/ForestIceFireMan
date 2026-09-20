using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EndText : MonoBehaviour
{

    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(1190, 20f);
    }
}
