
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

    [Header("Novice Room")]
    [SerializeField] Button Novicebutton;
    [SerializeField] private TMP_Text NMinBet;
    [SerializeField] private TMP_Text NMaxBet;
    [SerializeField] internal TMP_Text NPlayerCount;

    [Header("Expert Room")]
    [SerializeField] Button Expertbutton;
    [SerializeField] private TMP_Text EMinBet;
    [SerializeField] private TMP_Text EMaxBet;
    [SerializeField] internal TMP_Text EPlayerCount;

    [Header("HighRoller Room")]
    [SerializeField] Button HighRollerbutton;
    [SerializeField] private TMP_Text HMinBet;
    [SerializeField] private TMP_Text HMaxBet;
    [SerializeField] internal TMP_Text HPlayerCount;
    [Header("anouncement panel")]
    [SerializeField] private GameObject textAnnouncement;
    [SerializeField] private Transform AnnouncementStartpos;
    [SerializeField] private Transform AnnouncementEndPos;
    private float scrollDuration = 10f;

    [Header("Bottom panel")]
    [SerializeField] private List<Button> CommingSoonBtn;

    [SerializeField] private Image ComingSoonImage;

    [SerializeField] private float moveY = 60f;
    [SerializeField] private float duration = 0.6f;

    private RectTransform imgRect;
    private CanvasGroup cg;
    void Start()
    {
        AnnouncementScrollAnim();




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

        CMinBet.text = gameData.bets.casual[0].ToString();
        CMaxBet.text = gameData.bets.casual[gameData.bets.casual.Count - 1].ToString();
        CPlayerCount.text = "<size=40>" + gameData.lobby.casual.ToString() + "</size>" + "<size=25> Players</size>";


        NMinBet.text = gameData.bets.novice[0].ToString();
        NMaxBet.text = gameData.bets.novice[gameData.bets.novice.Count - 1].ToString();
        NPlayerCount.text = "<size=40>" + gameData.lobby.novice.ToString() + "</size>" + "<size=25> Players</size>";


        EMinBet.text = gameData.bets.expert[0].ToString();
        EMaxBet.text = gameData.bets.expert[gameData.bets.expert.Count - 1].ToString();
        EPlayerCount.text = "<size=40>" + gameData.lobby.expert.ToString() + "</size>" + "<size=25> Players</size>";


        HMinBet.text = gameData.bets.high_roller[0].ToString();
        HMaxBet.text = gameData.bets.high_roller[gameData.bets.high_roller.Count - 1].ToString();
        HPlayerCount.text = "<size=40>" + gameData.lobby.high_roller.ToString() + "</size>" + "<size=25> Players</size>";

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





    void AnnouncementScrollAnim()
    {
        textAnnouncement.transform.localPosition =
            AnnouncementStartpos.localPosition;


        textAnnouncement.transform
            .DOLocalMove(AnnouncementEndPos.localPosition, scrollDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }


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
