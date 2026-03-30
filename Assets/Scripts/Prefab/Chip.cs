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
    internal Sprite postPopSprite;
    internal void SetData(Sprite chip, string amount, int ChipIndex)
    {
        chipImage.sprite = chip;
        Chiptext.text = amount;
        chipIndex = ChipIndex;
    }

    /// <summary>
    /// Change the chip sprite dynamically (used during win animations)
    /// </summary>
    internal void ChangeSprite(Sprite newSprite)
    {
        if (chipImage != null && newSprite != null)
        {
            chipImage.sprite = newSprite;
        }
    }

    private void OnEnable()
    {
        PlayPopAnimation();
    }

    private void PlayPopAnimation()
    {
        popTween?.Kill();
        transform.localScale = Vector3.zero;

        popTween = transform
            .DOScale(1.15f, 0.15f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.08f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        // Swap to colored sprite after pop finishes
                        if (postPopSprite != null)
                        {
                            chipImage.sprite = postPopSprite;
                            postPopSprite = null; // clear so it doesn't re-trigger on re-enable
                        }
                    });
            });
    }

    private void OnDisable()
    {
        popTween?.Kill();
    }
}