using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Chip : MonoBehaviour
{
    [SerializeField] internal Image chipImage;
    [SerializeField] internal TMP_Text Chiptext;
    private Tween popTween;
    internal int chipIndex;

    internal void SetData(Sprite chip, string amount, int ChipIndex)
    {
        chipImage.sprite = chip;
        Chiptext.text = amount;
        chipIndex = ChipIndex;
    }


    private void OnEnable()
    {
        PlayPopAnimation();
    }

    private void PlayPopAnimation()
    {
        // Kill previous tween if any (important for pooling)
        popTween?.Kill();

        transform.localScale = Vector3.zero;

        popTween = transform
            .DOScale(1.15f, 0.15f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.08f).SetEase(Ease.InOutSine);
            });
    }

    private void OnDisable()
    {
        popTween?.Kill();
    }
}   