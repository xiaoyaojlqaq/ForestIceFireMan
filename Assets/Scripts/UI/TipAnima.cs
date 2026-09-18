using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TipAnima : MonoBehaviour
{
    RectTransform rectTransform;
    Tween tween;
    private void Awake()
    {
        rectTransform=GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        rectTransform.anchoredPosition = Vector2.zero;
        tween = rectTransform.DOAnchorPosY(0.25f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    private void OnDisable()
    {
        tween.Kill();
    }
}
