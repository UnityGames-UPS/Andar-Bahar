
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using System.Collections.Generic;

public class Homepage : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] SocketIOManager socketManager;
    [SerializeField] AudioManager audioManager;

    [Header("Player Details")]
    [SerializeField] private TMP_Text Playername;
    [SerializeField] internal TMP_Text PlayerBalance;
    [SerializeField] private Image PlayerImage;
    [Header("top Panel Details")]
    [SerializeField] internal TMP_Text TotalPlayerCount;

    [Header("Casual Room")]
    [SerializeField] Button Casualbutton;
    [SerializeField] private TMP_Text CMinBet;
    [SerializeField] private TMP_Text CMaxBet;
    [SerializeField] internal TMP_Text CPlayerCount;
    [SerializeField] internal GameObject CPlayerCountParent;
    [SerializeField] internal Image CPlayerCountImage;
    [SerializeField] internal GameObject CHotGame;

    [Header("Novice Room")]
    [SerializeField] Button Novicebutton;
    [SerializeField] private TMP_Text NMinBet;
    [SerializeField] private TMP_Text NMaxBet;
    [SerializeField] internal TMP_Text NPlayerCount;
    [SerializeField] internal GameObject NPlayerCountParent;
    [SerializeField] internal Image NPlayerCountImage;
    [SerializeField] internal GameObject NHotGame;

    [Header("Expert Room")]
    [SerializeField] Button Expertbutton;
    [SerializeField] private TMP_Text EMinBet;
    [SerializeField] private TMP_Text EMaxBet;
    [SerializeField] internal TMP_Text EPlayerCount;
    [SerializeField] internal GameObject EPlayerCountParent;
    [SerializeField] internal Image EPlayerCountImage;
    [SerializeField] internal GameObject EHotGame;

    [Header("HighRoller Room")]
    [SerializeField] Button HighRollerbutton;
    [SerializeField] private TMP_Text HMinBet;
    [SerializeField] private TMP_Text HMaxBet;
    [SerializeField] internal TMP_Text HPlayerCount;
    [SerializeField] internal GameObject HPlayerCountParent;
    [SerializeField] internal Image HPlayerCountImage;
    [SerializeField] internal GameObject HHotGame;

    [Header("Hot Game Sprites")]
    [SerializeField] internal Sprite HotGameRedSprite;
    [SerializeField] internal Sprite NormalGameGreenSprite;
    /*[Header("anouncement panel")]
    [SerializeField] private GameObject textAnnouncement;
    [SerializeField] private Transform AnnouncementStartpos;
    [SerializeField] private Transform AnnouncementEndPos;
    private float scrollDuration = 10f;*/

    [Header("Bottom panel")]
    [SerializeField] private List<Button> CommingSoonBtn;

    [SerializeField] private Image ComingSoonImage;

    [SerializeField] private float moveY = 60f;
    [SerializeField] private float duration = 0.6f;

    private RectTransform imgRect;
    private CanvasGroup cg;
    void Start()
    {
        //AnnouncementScrollAnim();




        imgRect = ComingSoonImage.rectTransform;

        cg = ComingSoonImage.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ComingSoonImage.gameObject.AddComponent<CanvasGroup>();

        foreach (Button btn in CommingSoonBtn)
        {
            btn.onClick.AddListener(() => ShowComingSoon(btn));
        }
    }
    internal void SetInitHomedata(GameData gameData)
    {

        if (Casualbutton)
        {

            Casualbutton.onClick.RemoveAllListeners();
            Casualbutton.onClick.AddListener(() => OnClickARoom(gameData.levels[0]));
        }
        if (Novicebutton)
        {

            Novicebutton.onClick.RemoveAllListeners();
            Novicebutton.onClick.AddListener(() => OnClickARoom(gameData.levels[1]));
        }
        if (Expertbutton)
        {

            Expertbutton.onClick.RemoveAllListeners();
            Expertbutton.onClick.AddListener(() => OnClickARoom(gameData.levels[2]));
        }
        if (HighRollerbutton)
        {

            HighRollerbutton.onClick.RemoveAllListeners();
            HighRollerbutton.onClick.AddListener(() => OnClickARoom(gameData.levels[3]));
        }

        CMinBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.casual.min_bet_limit);
        CMaxBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.casual.max_bet_limit);
        CPlayerCount.text = gameData.lobby.casual.ToString();
        CPlayerCountParent.gameObject.SetActive(gameData.lobby.casual != 0);

        NMinBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.novice.min_bet_limit);
        NMaxBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.novice.max_bet_limit);
        NPlayerCount.text = gameData.lobby.novice.ToString();
        NPlayerCountParent.gameObject.SetActive(gameData.lobby.novice != 0);

        EMinBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.expert.min_bet_limit);
        EMaxBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.expert.max_bet_limit);
        EPlayerCount.text = gameData.lobby.expert.ToString();
        EPlayerCountParent.gameObject.SetActive(gameData.lobby.expert != 0);

        HMinBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.high_roller.min_bet_limit);
        HMaxBet.text = FormatHelper.FormatAmount(socketManager.initialData.levelBetLimit.high_roller.max_bet_limit);
        HPlayerCount.text = gameData.lobby.high_roller.ToString();
        HPlayerCountParent.gameObject.SetActive(gameData.lobby.high_roller != 0);

        TotalPlayerCount.text = (gameData.lobby.casual + gameData.lobby.novice + gameData.lobby.expert + gameData.lobby.high_roller).ToString();

    }
    internal void setPlayerData(Player player)
    {
        PlayerBalance.text = player.balance.ToString();
        Playername.text = player.username.ToString();
    }




    internal void OnClickARoom(string room)
    {
        audioManager.PlayButtonAudio();
        socketManager.SendRoomSelection(room);
        gameManager.currentRoom = room;
        gameManager.SetLoadingPage(true);
    }





    /*void AnnouncementScrollAnim()
    {
        textAnnouncement.transform.localPosition =
            AnnouncementStartpos.localPosition;


        textAnnouncement.transform
            .DOLocalMove(AnnouncementEndPos.localPosition, scrollDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }*/


    void ShowComingSoon(Button btn)
    {
        imgRect.DOKill();
        ComingSoonImage.DOKill();

        // place image on button
        imgRect.position = btn.transform.position;

        // reset alpha
        Color c = ComingSoonImage.color;
        c.a = 1f;
        ComingSoonImage.color = c;

        // move up
        imgRect.DOMoveY(imgRect.position.y + moveY, duration)
               .SetEase(Ease.OutQuad);

        // fade out using color alpha
        ComingSoonImage.DOFade(0f, duration);
    }
}
