using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HistoryPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text Index;
    [SerializeField] private TMP_Text RoundId;
    [SerializeField] private TMP_Text BetTime;
    [SerializeField] private TMP_Text Stake;
    [SerializeField] private TMP_Text Win;
    [SerializeField] private TMP_Text PL;
    [SerializeField] private TMP_Text CardDelt;
    [SerializeField] private Image Middlecard;
    [SerializeField] private Image MatchCard;
    [SerializeField] private Image MatchSideImage;

    internal void SetData(
        int index,
        History item,
        Sprite middle,
        Sprite matchCard,
        Sprite matchSideSprite,
        int cardDelt)
    {
        // Set index
        Index.text = index.ToString();

        // Set round ID (full)
        RoundId.text = item.round_id;

        // Format and set bet time: dd/mm/yyyy \n hh:mm:ss
        // FIXED: Now parsing from string instead of using DateTime directly
        BetTime.text = FormatBetTime(item.created_at);

        // Set stake
        Stake.text = FormatHelper.FormatAmount(item.bet_amount);

        // Set win
        Win.text = FormatHelper.FormatAmount(item.win_amount);

        // Calculate and set P/L with + or - sign
        double pl = item.win_amount - item.bet_amount;
        string plSign = pl >= 0 ? "+" : "";
        PL.text = plSign + FormatHelper.FormatAmount(pl);

        // Set card dealt count
        CardDelt.text = cardDelt.ToString();

        // Set card sprites
        Middlecard.sprite = middle;
        MatchCard.sprite = matchCard;

        // Set match side image (A for andar, B for bahar)
        MatchSideImage.sprite = matchSideSprite;
    }

    private string FormatBetTime(string createdAtString)
    {
        // Parse the ISO 8601 string to DateTime
        if (string.IsNullOrEmpty(createdAtString))
        {
            return "Invalid Date\nInvalid Time";
        }

        DateTime dateTime;

        // Try to parse the ISO 8601 format (e.g., "2026-03-20T06:31:33.367Z")
        if (!DateTime.TryParse(createdAtString, out dateTime))
        {
            return "Invalid Date\nInvalid Time";
        }

        // Format: dd/mm/yyyy \n hh:mm:ss
        string date = dateTime.ToString("dd/MM/yyyy");
        string time = dateTime.ToString("HH:mm:ss");
        return $"{date}\n{time}";
    }
}