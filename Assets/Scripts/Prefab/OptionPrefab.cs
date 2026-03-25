using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionPrefab : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button btn;
    [SerializeField] internal GameObject HighlightedBG;
    [SerializeField] internal GameObject BG;
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Text;
    [SerializeField] internal RectTransform chiparea;
    [SerializeField] internal ImageAnimation winAnimation;
    [SerializeField] internal GameObject MyBetObj;
    [SerializeField] internal TMP_Text MyBetText;
    [SerializeField] internal GameObject TotalBetObj;
    [SerializeField] internal TMP_Text TotalBetText;

    internal int Optionindex;
    internal string VaridontWant;

    internal int totalBet;
    internal int playerBet;

    internal string NameT;
    void Start()
    {
        if (btn)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickBtn);
        }
    }

    internal void OnClickBtn()
    {
        gameManager.onClickOption(gameObject);
    }

    internal void SetData(int index, string name, string ratio, string OptionType)
    {
        if (Name) Name.text = name;
        if (ratio == "") Text.text = "";
        else Text.text = "1 : " + ratio;

        Optionindex = index;
        VaridontWant = OptionType;
    }
    internal void DontShowText()
    {
        totalBet = 0;
        playerBet = 0;
        TotalBetText.text = "";
        MyBetText.text = "";
        TotalBetObj.SetActive(false);
        MyBetObj.SetActive(false);
    }
    internal IEnumerator Highlighttext()
    {
        Color orignalColor = Name.color;
        Name.color = Color.yellow;
        Text.color = Color.yellow;
        yield return new WaitForSeconds(3f);
        Name.color = orignalColor;
        Text.color = orignalColor;

    }

    internal void DisableWinRatioText()
    {
        if (Text)
            Text.gameObject.SetActive(false);
    }

    internal void EnableWinRatioText()
    {
        if (Text)
            Text.gameObject.SetActive(true);
    }
}