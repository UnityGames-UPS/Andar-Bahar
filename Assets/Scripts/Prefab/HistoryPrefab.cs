
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HistoryPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text Index;
    [SerializeField] private TMP_Text RoundId;
    [SerializeField] private TMP_Text Stake;
    [SerializeField] private TMP_Text Win;
    [SerializeField] private TMP_Text PL;
    [SerializeField] private TMP_Text CardDelt;
    [SerializeField] private Image Middlecard;
    [SerializeField] private Image SideCard;

    internal void SetData(int index, History item, Sprite middle, Sprite side, int cardDelt)
    {
        Index.text = index.ToString();
        RoundId.text = item.round_id;
        Stake.text = FormatHelper.FormatAmount(item.bet_amount);
        Win.text = FormatHelper.FormatAmount(item.win_amount);
        CardDelt.text = cardDelt.ToString();
        double pl = item.win_amount - item.bet_amount;
        PL.text = FormatHelper.FormatAmount(pl);

        Middlecard.sprite = middle;
        SideCard.sprite = side;
    }
}
