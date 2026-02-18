using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerData : MonoBehaviour
{
    [SerializeField] internal Image PlayerIcon;
    [SerializeField] internal TMP_Text playername;
    [SerializeField] internal TMP_Text playerBalence;
    [SerializeField] internal string PlayerId;
    [Header("Anim Settings")]
    private float moveY = 20f;
    private float duration = 0.7f;
    private float delayBetween = 0.3f;
    [SerializeField] private bool isAnimationNeeded = true;
    private Vector3 nameOriginalPos;
    private Vector3 balOriginalPos;
    private bool positionsCached = false;


    private Sequence loopSeq;
    internal void SetData(string idz, string balance, Sprite icon)
    {
        gameObject.SetActive(true);
        PlayerIcon.sprite = icon;
        PlayerId = idz;
        playername.text = PlayerId;
        playerBalence.text = balance;
        if (isAnimationNeeded) PlayLoopAnimation();
    }

    void PlayLoopAnimation()
    {
        loopSeq?.Kill();

        RectTransform nameRT = playername.rectTransform;
        RectTransform balRT = playerBalence.rectTransform;

        if (!positionsCached)
        {
            nameOriginalPos = nameRT.localPosition;
            balOriginalPos = balRT.localPosition;
            positionsCached = true;
        }

        nameRT.localPosition = nameOriginalPos;
        balRT.localPosition = balOriginalPos;

        float fastDuration = 0.2f;     // Faster movement
        float stayTime = 0.4f;         // Stay at top
        float bottomDelay = 0.2f;      // Small delay before next

        loopSeq = DOTween.Sequence();

        // 🔹 Name animation
        loopSeq.Append(nameRT.DOLocalMoveY(nameOriginalPos.y + moveY, fastDuration)
            .SetEase(Ease.OutQuad));

        loopSeq.AppendInterval(stayTime);   // Stay at top

        loopSeq.Append(nameRT.DOLocalMoveY(nameOriginalPos.y, fastDuration)
            .SetEase(Ease.InQuad));

        loopSeq.AppendInterval(bottomDelay);

        // 🔹 Balance animation
        loopSeq.Append(balRT.DOLocalMoveY(balOriginalPos.y + moveY, fastDuration)
            .SetEase(Ease.OutQuad));

        loopSeq.AppendInterval(stayTime);

        loopSeq.Append(balRT.DOLocalMoveY(balOriginalPos.y, fastDuration)
            .SetEase(Ease.InQuad));

        loopSeq.AppendInterval(bottomDelay);

        loopSeq.SetLoops(-1);
    }




    private void OnDisable()
    {
        loopSeq?.Kill();
    }
}
