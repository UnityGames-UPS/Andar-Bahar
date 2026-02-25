using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPrefab : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] internal GameObject BlueBg;
    [SerializeField] internal GameObject RedBg;
    [SerializeField] internal GameObject HighBg;

    [SerializeField] internal TMP_Text cardnumber;
    [SerializeField] internal TMP_Text countNumber;
    internal Color winColor;
    internal string winner;

    internal void SetData(string cardNO, bool isBlue, bool isHighLited, string countNO = "idn")
    {
        cardnumber.text = cardNO;
        if (HighBg) HighBg.SetActive(isHighLited);

        if (countNO == "idn")
        {

            if (isBlue)
            {
                cardnumber.color = new Color32(0x0D, 0xB3, 0xEE, 255);
                winColor = Color.blue;
                // winColor = Color.blue;
                winner = "andar";
            }
            else
            {
                cardnumber.color = Color.red;
                winColor = Color.red;
                winner = "bahar";
            }
        }
        else
        {
            countNumber.text = countNO;
            if (isBlue) winner = "andar";
            else winner = "bahar";
            BlueBg.SetActive(isBlue);
            RedBg.SetActive(!isBlue);

        }
    }
    internal void CopyFrom(StatsPrefab other)
    {
        cardnumber.text = other.cardnumber.text;
        cardnumber.color = other.cardnumber.color;

        HighBg.SetActive(other.HighBg.activeSelf);

        winColor = other.winColor;
    }

}

