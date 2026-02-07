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
    [SerializeField] private float moveY = 15f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float delayBetween = 0.1f;

    private Sequence loopSeq;
    internal void SetData(string idz, string balance, Sprite icon)
    {
        gameObject.SetActive(true);
        PlayerIcon.sprite = icon;
        PlayerId = idz;
        playername.text = PlayerId;
        playerBalence.text = balance;
    }

    void PlayLoopAnimation()
    {
        // kill old animation if any
        loopSeq?.Kill();

        RectTransform nameRT = playername.rectTransform;
        RectTransform balRT = playerBalence.rectTransform;

        Vector3 nameStart = nameRT.localPosition;
        Vector3 balStart = balRT.localPosition;

        loopSeq = DOTween.Sequence();

        // Player name up & down
        loopSeq.Append(nameRT.DOLocalMoveY(nameStart.y + moveY, duration).SetEase(Ease.OutQuad));
        loopSeq.Append(nameRT.DOLocalMoveY(nameStart.y, duration).SetEase(Ease.InQuad));
        loopSeq.AppendInterval(delayBetween);

        // Balance up & down
        loopSeq.Append(balRT.DOLocalMoveY(balStart.y + moveY, duration).SetEase(Ease.OutQuad));
        loopSeq.Append(balRT.DOLocalMoveY(balStart.y, duration).SetEase(Ease.InQuad));
        loopSeq.AppendInterval(delayBetween);

        loopSeq.SetLoops(-1);
    }

    private void OnDisable()
    {
        loopSeq?.Kill();
    }
}
