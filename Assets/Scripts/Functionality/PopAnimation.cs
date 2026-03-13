using UnityEngine;
using DG.Tweening;

public class PopAnimation : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool isPoping = true; // true = pop, false = slide
    [SerializeField] private bool isPoupInGameSlide = false;
    [Header("Pop Settings")]
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private Ease popEase = Ease.OutBack;

    [Header("Slide Settings")]
    [SerializeField] private float startOffsetX = -800f;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private Ease slideEase = Ease.OutCubic;
    [SerializeField] private float OffsetX = 20f;
    [SerializeField] private float OffsetY = 20f;
    private RectTransform rectTransform;
    private Vector2 endPos;

    private Tween popTween;
    private Tween slideTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        endPos = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        if (isPoping)
            PlayPop();
        else
            PlaySlide();
    }

    private void OnDisable()
    {
        popTween?.Kill();
        slideTween?.Kill();
    }

    private void PlayPop()
    {
        popTween?.Kill();

        rectTransform.localScale = Vector3.one * startScale;

        popTween = rectTransform
            .DOScale(endScale, popDuration)
            .SetEase(popEase)
            .SetUpdate(true);
    }

    private void PlaySlide()
    {
        Vector2 endOffset;
        slideTween?.Kill();        
        rectTransform.anchoredPosition =
            new Vector2(endPos.x - startOffsetX, endPos.y);
        if (isPoupInGameSlide)
        {
            endOffset = new Vector2(endPos.x - OffsetX, endPos.y + OffsetY);
        }
        else
        {
            endOffset = new Vector2(endPos.x - 20f, endPos.y);
        }

        slideTween = rectTransform
            .DOAnchorPos(endOffset, slideDuration)
            .SetEase(slideEase)
            .SetUpdate(true);
    }


}
