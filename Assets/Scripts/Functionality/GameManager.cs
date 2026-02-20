using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Best.SocketIO;
using System.Linq;



public class GameManager : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] internal GameObject LoadingPage;
    [SerializeField] internal GameObject HomePage;
    [SerializeField] internal GameObject GamePage;

    [Header("ScriptRef")]
    [SerializeField] private Homepage homepage;
    [SerializeField] SocketIOManager socketManager;
    [SerializeField] internal UiManager uiManager;
    [SerializeField] AudioManager audioManager;


    [Header("Texts")]
    [SerializeField] internal TMP_Text LoadingPage_text;
    [SerializeField] private TMP_Text RoundInfo_Text;
    [SerializeField] private TMP_Text CardCount_Text;
    [SerializeField] private TMP_Text TotalPlayer_text;
    [SerializeField] private TMP_Text minBet_text;
    [SerializeField] private TMP_Text maxBet_text;
    // [SerializeField] private TMP_Text WinChance_text;
    // [SerializeField] private TMP_Text ResponseMult_text;

    [Header("Options Text")]
    [SerializeField] private OptionPrefab AndarTxt;
    [SerializeField] private OptionPrefab BaharTxt;
    [SerializeField] private OptionPrefab FirstAndarTxt;
    [SerializeField] private OptionPrefab FirstThreeTxt;
    [SerializeField] private OptionPrefab FirstBaharTxt;
    [SerializeField] private OptionPrefab OptionQTxt;
    [SerializeField] private OptionPrefab OptionWTxt;
    [SerializeField] private OptionPrefab OptionETxt;
    [SerializeField] private OptionPrefab OptionRTxt;
    [SerializeField] private OptionPrefab OptionTTxt;
    [SerializeField] private OptionPrefab OptionYTxt;
    [SerializeField] private OptionPrefab OptionUTxt;
    [SerializeField] private OptionPrefab OptionITxt;
    [SerializeField] private OptionPrefab OptionOTxt;
    [SerializeField] private List<OptionPrefab> AllOptions;
    [SerializeField] private List<OptionPrefab> BiggerOptions;

    [Header("Hand Animation")]
    //  [SerializeField] private GameObject Hand;
    [SerializeField] private Animator Handanimator;
    [SerializeField] private AnimationCall animHand;
    [SerializeField] private List<Sprite> HeartSpriteList;
    [SerializeField] private List<Sprite> DiamondSpriteList;
    [SerializeField] private List<Sprite> ClubSpriteList;
    [SerializeField] private List<Sprite> SpadeSpriteList;
    [Header("Hand Animation")]
    [SerializeField] private List<Image> AndarCardparent;
    [SerializeField] private List<Image> BaharCardparent;
    private int AndarIndex;
    private int BaharIndex;

    [Header("Flush Animation")]
    [SerializeField] private List<Sprite> FlushSprite;
    [SerializeField] private List<Sprite> StraightFlushSprite;
    [SerializeField] private List<Sprite> StraightSprite;
    [SerializeField] private GameObject MainFlushObj;
    [SerializeField] private ImageAnimation FlushImageAnim;
    [SerializeField] private Image FlushCardOne;
    [SerializeField] private Image FlushCardTwo;
    [SerializeField] private Image FlushCardThree;
    [SerializeField] private Animator FlushCard;
    [SerializeField] private ImageAnimation DropImageAnim;
    [SerializeField] private List<Sprite> purpleDrop;
    [SerializeField] private List<Sprite> yellowDrop;
    [SerializeField] private List<Sprite> pinkDrop;

    [Header("Chip")]
    [SerializeField] private List<Sprite> PlayerChipSprite;
    [SerializeField] private List<Sprite> otherChipSprite;
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private int initialCount = 20;
    [SerializeField] private Transform poolParent;
    [SerializeField] private Transform OtherPlayerChipPoolParent;

    private readonly List<GameObject> pool = new List<GameObject>();

    [Header("popup")]
    [SerializeField] private Button BetBlocker;
    [SerializeField] private GameObject BlockerObj;
    [SerializeField] private TMP_Text BlockerText;
    [SerializeField] private Transform popStart;
    [SerializeField] private Transform popCenter;
    [SerializeField] private Transform popEnd;
    [SerializeField] private TMP_Text coinAddText;
    [SerializeField] private float moveY = 60f;
    [SerializeField] private float duration = 0.8f;

    private Vector3 startPos;
    private Color startColor;
    private Tween coinTween;
    private Tween popTween;

    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float holdDuration = 1f;
    private Coroutine animRoutine;

    [Header("Result ")]
    [SerializeField] private GameObject AndarHighLight;
    [SerializeField] private GameObject BaharHighLight;
    [SerializeField] private GameObject AndarBtnHighLight;
    [SerializeField] private GameObject BaharBtnHighLight;
    [SerializeField] private GameObject AndarbaharBetReset;
    [SerializeField] private ImageAnimation NewRoundAnim;
    [SerializeField] private Sprite HandResetSprite;
    [Header("Round Info ")]
    [SerializeField] private GameObject RoundInfoImage;
    [SerializeField] private TMP_Text pulseText;
    [SerializeField] private List<Sprite> roundInfoSprites;
    [Header("Bonus  ")]
    [SerializeField] private GameObject BonusObject;

    internal List<OptionPrefab> resultsOptions = new List<OptionPrefab>();

    private float popScale = 1.15f;
    private float animTime = 0.15f;

    internal int BetCounter;
    internal int MultiplierCounter;
    internal string currentRoom;
    internal string nextRoom;
    internal bool directJump = false;
    internal double currentTotalBet = 0;
    private double currentWin;
    private double animationduration = 2f;

    internal bool isRepeatbetActive = false;
    private Coroutine StartGameCorutine;
    private Coroutine EndGameCorutine;

    private Vector3 startPoscoin = new Vector3(0, -138, 0);
    private Vector3 endPos = new Vector3(0, -10, 0);


    private List<ChipData> PlayerChips = new List<ChipData>();
    private List<ChipData> OtherPlayerChips = new List<ChipData>();


    void Awake()
    {

        for (int i = 0; i < initialCount; i++)
            AddChip();
    }
    private void Start()
    {
        //  Handanimator.Play("MiddleCard");
        BetCounter = 0;
        HomePage.SetActive(true);
        LoadingPage.SetActive(false);
        GamePage.SetActive(false);
        uiManager.MenuInGame_button.gameObject.SetActive(false);
        BetBlocker.onClick.RemoveAllListeners();
        BetBlocker.onClick.AddListener(() => PlayPopup("This Round is already closed.\nPlease wait for next round."));
        startPoscoin = coinAddText.rectTransform.localPosition;
        startColor = coinAddText.color;
        coinAddText.gameObject.SetActive(false);

    }


    #region  DataSetup
    internal void SetInitialData()
    {
        uiManager.SetgameRulePanel();
        homepage.SetInitHomedata(socketManager.initialData);
        SetPlayerData(socketManager.playerdata);
    }
    internal IEnumerator ShowLoadingPage(string loadingPageText, int activeTime = 6, bool active = false)
    {
        ImageAnimation anim = LoadingPage.GetComponentInChildren<ImageAnimation>();
        anim.StopAnimation();
        anim.StartAnimation();
        LoadingPage_text.text = loadingPageText;
        LoadingPage.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        LoadingPage.SetActive(active);

    }
    internal void SetLoadingPage(bool isActive)
    {
        ImageAnimation anim = LoadingPage.GetComponentInChildren<ImageAnimation>();
        anim.StopAnimation();
        anim.StartAnimation();

        LoadingPage.SetActive(isActive);
        if (isActive) LoadingPage_text.text = "Joining A Table....."; ;
    }
    IEnumerator ManageloadingPageText()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < 8) LoadingPage_text.text = "Joining A Table.....";
            else LoadingPage_text.text = "Waiting For New Round To Start.....";

            yield return new WaitForSeconds(1f);
        }
    }
    internal void OnGameLoaded()
    {
        GamePage.SetActive(true);

        // SetOptionData();
        //  SetCoinData();
        SetLoadingPage(false);

    }
    internal void SetOptionData()
    {
        AndarTxt.SetData(0, "andar", "", "main_bets");
        BaharTxt.SetData(1, "bahar", "", "main_bets");

        FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[0].ToString(), "op_bets");
        FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0].ToString(), "op_bets");
        FirstThreeTxt.SetData(4, "firstThree", socketManager.initialData.wagers.op_bets.first_3.payout.straight.ToString(), "op_bets");
        OptionQTxt.SetData(5, "1 - 5 Cards", socketManager.initialData.wagers.side_bets.s_1_5.payout.ToString(), "side_bets");
        OptionWTxt.SetData(6, "6 - 10 Cards", socketManager.initialData.wagers.side_bets.s_6_10.payout.ToString(), "side_bets");
        OptionETxt.SetData(7, "11 - 15 Cards", socketManager.initialData.wagers.side_bets.s_11_15.payout.ToString(), "side_bets");
        OptionRTxt.SetData(8, "15 - 20 Cards", socketManager.initialData.wagers.side_bets.s_16_20.payout.ToString(), "side_bets");
        OptionTTxt.SetData(9, "21 - 25 Cards", socketManager.initialData.wagers.side_bets.s_21_25.payout.ToString(), "side_bets");
        OptionYTxt.SetData(10, "26 - 30 Cards", socketManager.initialData.wagers.side_bets.s_26_30.payout.ToString(), "side_bets");
        OptionUTxt.SetData(11, "31 - 35 Cards", socketManager.initialData.wagers.side_bets.s_31_35.payout.ToString(), "side_bets");
        OptionITxt.SetData(12, "36 - 40 Cards", socketManager.initialData.wagers.side_bets.s_36_40.payout.ToString(), "side_bets");
        OptionOTxt.SetData(13, "41 or more", socketManager.initialData.wagers.side_bets.s_41_53.payout.ToString(), "side_bets");

    }

    internal void SetCoinData()
    {
        ResetCoinsToDefault();
        TotalPlayer_text.text = socketManager.roomData.payload.playerCount.ToString();
        string room = currentRoom;
        List<int> data = null;

        switch (room)
        {
            case "casual":
                data = socketManager.initialData.bets.casual;
                break;

            case "novice":
                data = socketManager.initialData.bets.novice;
                break;

            case "expert":
                data = socketManager.initialData.bets.expert;
                break;

            case "high_roller":
                data = socketManager.initialData.bets.high_roller;
                break;
        }

        if (data == null) return;

        uiManager.coinSelector.Chiptext.text = data[0].ToString();
        uiManager.coinSelector.chipIndex = 0;
        minBet_text.text = data[0].ToString();
        uiManager.MinBet.text = data[0].ToString();
        maxBet_text.text = data[uiManager.Coins.Count].ToString();

        for (int i = 0; i < uiManager.Coins.Count; i++)
        {
            uiManager.Coins[i].Chiptext.text = data[i + 1].ToString();
            uiManager.Coins[i].chipIndex = i + 1;
        }
    }
    public void ResetCoinsToDefault()
    {
        // Force select 0 index
        uiManager.coinSelector.chipImage.sprite = PlayerChipSprite[0];

        // Reset all coins visuals
        for (int i = 0; i < uiManager.Coins.Count; i++)
        {
            uiManager.Coins[i].chipImage.sprite = PlayerChipSprite[i + 1];
        }

    }


    void SetPlayerData(Player player)
    {
        homepage.setPlayerData(socketManager.playerdata);
        uiManager.MainPlayers.SetData(player.username, player.balance.ToString(), uiManager.UserIcons[0]);
        uiManager.Username.text = "ID: " + player.username;
    }
    internal void SetOtherplayerData(Leaderboards leaderboard)
    {
        Debug.Log("Setting Leaderboard");
        if (leaderboard == null)
        {
            Debug.Log("Leaderboards is NULL");
            return;
        }

        string mainPlayerName = uiManager.MainPlayers.playername.text;
        Sprite mainPlayerIcon = uiManager.MainPlayers.PlayerIcon.sprite;

        // ------------------- RICHEST -------------------
        if (leaderboard.richest == null || leaderboard.richest.Count == 0)
        {
            Debug.Log("richest is null");
            foreach (var item in uiManager.RichestPlayers)
                item.gameObject.SetActive(false);

            return;
        }

        foreach (var item in uiManager.RichestPlayers)
            item.gameObject.SetActive(false);

        int richestCount = Mathf.Min(leaderboard.richest.Count, uiManager.RichestPlayers.Count);

        for (int i = 0; i < richestCount; i++)
        {
            Richest rich = leaderboard.richest[i];

            // ✔ Use player's own icon if usernames match
            Sprite iconToUse = (rich.username == mainPlayerName)
                ? mainPlayerIcon
                : uiManager.UserIcons[UnityEngine.Random.Range(0, uiManager.UserIcons.Count)];

            uiManager.RichestPlayers[i].SetData(
                rich.username,
                rich.balance.ToString(),
                iconToUse
            );
            uiManager.RichestPlayers[i].gameObject.SetActive(true);
        }

        // ------------------- WINNERS -------------------
        if (leaderboard.winners == null || leaderboard.winners.Count == 0)
        {
            Debug.Log("winner is null");
            foreach (var item in uiManager.WinnerPlayers)
                item.gameObject.SetActive(false);

            return;
        }

        foreach (var item in uiManager.WinnerPlayers)
            item.gameObject.SetActive(false);

        int winnersCount = Mathf.Min(leaderboard.winners.Count, uiManager.WinnerPlayers.Count);

        for (int i = 0; i < winnersCount; i++)
        {
            Winner win = leaderboard.winners[i];

            // ✔ Same logic here — use player icon if matched
            Sprite iconToUse = (win.username == mainPlayerName)
                ? mainPlayerIcon
                : uiManager.UserIcons[UnityEngine.Random.Range(0, uiManager.UserIcons.Count)];

            uiManager.WinnerPlayers[i].SetData(
                win.username,
                win.totalWins.ToString(),
                iconToUse
            );

            uiManager.WinnerPlayers[i].gameObject.SetActive(true);
        }
    }


    internal List<string> GetAllMaxLimits(string room)
    {
        List<string> result = new List<string>();
        var wagers = socketManager.initialData.wagers;
        var betOptions = socketManager.initialData.betOptions;

        foreach (var betType in betOptions)
        {
            MaxBetLimit limit = null;

            switch (betType)
            {
                case "andar":
                    limit = wagers.main_bets.andar.max_bet_limit;
                    break;

                case "bahar":
                    limit = wagers.main_bets.bahar.max_bet_limit;
                    break;

                case "first_1_andar":
                    limit = wagers.op_bets.first_1_andar.max_bet_limit;
                    break;

                case "first_1_bahar":
                    limit = wagers.op_bets.first_1_bahar.max_bet_limit;
                    break;

                case "first_3":
                    limit = wagers.op_bets.first_3.max_bet_limit;
                    break;

                case "s_1_5":
                    limit = wagers.side_bets.s_1_5.max_bet_limit;
                    break;

                case "s_6_10":
                    limit = wagers.side_bets.s_6_10.max_bet_limit;
                    break;

                case "s_11_15":
                    limit = wagers.side_bets.s_11_15.max_bet_limit;
                    break;

                case "s_16_20":
                    limit = wagers.side_bets.s_16_20.max_bet_limit;
                    break;

                case "s_21_25":
                    limit = wagers.side_bets.s_21_25.max_bet_limit;
                    break;

                case "s_26_30":
                    limit = wagers.side_bets.s_26_30.max_bet_limit;
                    break;

                case "s_31_35":
                    limit = wagers.side_bets.s_31_35.max_bet_limit;
                    break;

                case "s_36_40":
                    limit = wagers.side_bets.s_36_40.max_bet_limit;
                    break;

                case "s_41_53":
                    limit = wagers.side_bets.s_41_53.max_bet_limit;
                    break;
            }

            int maxValue = GetLimitByRoom(limit, room);
            result.Add(maxValue.ToString());
        }

        return result;
    }

    private int GetLimitByRoom(MaxBetLimit limit, string room)
    {
        switch (room)
        {
            case "casual": return limit.casual;
            case "novice": return limit.novice;
            case "expert": return limit.expert;
            case "high_roller": return limit.high_roller;
        }

        return 0;
    }


    #endregion




    #region GamePlay
    internal void SetMainCard()
    {
        if (StartGameCorutine != null)
        {
            StopCoroutine(StartGameCorutine);
            StartGameCorutine = null;
        }
        if (EndGameCorutine != null)
        {
            StopCoroutine(EndGameCorutine);
            EndGameCorutine = null;
        }

        TotalPlayer_text.text = socketManager.gameLoopData.playerCount.ToString();


        currentTotalBet = 0;
        audioManager.PlayGirlAudio("placeyourbet");
        BetBlocker.gameObject.SetActive(false);
        uiManager.setCoins(true);
        uiManager.SetChipoption(false);
        if (isRepeatbetActive && !uiManager.isExpanded) uiManager.Repeatpanel.SetActive(true);
        uiManager.SetNetBetPanel(false);
        MainFlushObj.SetActive(false);
        ResetAllBetUI();
        FirstThreeTxt.HighlightedBG.SetActive(false);
        FirstAndarTxt.HighlightedBG.SetActive(false);
        FirstBaharTxt.HighlightedBG.SetActive(false);


        PlayMiddleCardAnim();
        // yield return new WaitForSeconds(2f);

        bool startWithAndar = socketManager.gameLoopData.middleCard.color == "black";
        if (startWithAndar)
        {
            AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[0].ToString(), "main_bets");
            BaharTxt.SetData(1, "bahar", "1", "main_bets");

        }
        else
        {
            AndarTxt.SetData(0, "andar", "1", "main_bets");
            BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[0].ToString(), "main_bets");

        }

    }

    internal void SetBetTimer()
    {
        ResetCardHistory();
        BetBlocker.gameObject.SetActive(false);
        RoundInfo_Text.gameObject.SetActive(true);
        animHand.MiddleCard.sprite = CardSet(socketManager.TimeRemaining.middleCard.suit, socketManager.TimeRemaining.middleCard.rank);
        animHand.MiddleCard.gameObject.SetActive(true);
        animHand.LeftCard.gameObject.SetActive(false);
        animHand.RightCard.gameObject.SetActive(false);

        int time = socketManager.TimeRemaining.timeRemaining / 1000;
        if (time > 5)
        {
            // RoundInfo_Text.text = "<size=30>Place bet Now</size>\n " + "<size=50><color=yellow>" + time + "</color></size>";
            RoundInfo_Text.text = "<size=30>Place bet Now</size>\n ";
            pulseText.text = "<color=yellow>" + time.ToString() + "</color>";
            pulseText.gameObject.SetActive(true);
        }
        else
        {
            RoundInfo_Text.text = "<size=30>Place bet Now</size>\n ";
            pulseText.text = "<color=yellow>" + time.ToString() + "</color>";
            pulseText.gameObject.SetActive(true);
            PopTMP(pulseText);

        }
        if (time == 5)
        {
            RoundInfoAnim(2);
            audioManager.PlayGirlAudio("timeisrunning");
        }
        else if (time == 0)
        {
            pulseText.gameObject.SetActive(false);
            RoundInfoAnim(1);
            BetBlocker.gameObject.SetActive(true);
            audioManager.PlayGirlAudio("nomorebets");
            //RoundInfo_Text.gameObject.SetActive(false);
            uiManager.SetChipoption(false);
            uiManager.setCoins(false);
            uiManager.Repeatpanel.SetActive(false);
            uiManager.SetNetBetPanel(true, currentTotalBet.ToString());
            RoundInfo_Text.text = "<size=30>Bet Locked!</size>";
            BetBlocker.gameObject.SetActive(true);
        }
        if (time % 5 == 4)
        {
            Handanimator.Play("NoHand");

        }
    }

    public void PopTMP(TMP_Text tmp)
    {
        if (tmp == null) return;

        popTween?.Kill(); // stop previous

        RectTransform rt = tmp.rectTransform;

        rt.localScale = Vector3.one;

        popTween = rt.DOScale(1.4f, 0.15f)
                     .SetEase(Ease.OutQuad)
                     .SetLoops(2, LoopType.Yoyo);
    }
    internal void ManageCardDelt(Root cardDelt)
    {

        pulseText.gameObject.SetActive(false);
        BetBlocker.gameObject.SetActive(true);
        uiManager.SetChipoption(false);
        uiManager.setCoins(false);
        uiManager.Repeatpanel.SetActive(false);
        uiManager.SetNetBetPanel(true, currentTotalBet.ToString());
        RoundInfo_Text.text = "<size=30>Bet Locked!</size>";

        ManageCardCounts(cardDelt.cardsDealt);
        animHand.MiddleCard.sprite = CardSet(cardDelt.middleCard.suit, cardDelt.middleCard.rank);
        animHand.MiddleCard.gameObject.SetActive(true);

        if (cardDelt.side == "andar")
        {
            PlayAndarCardAnim(cardDelt.cardsDealt);

        }
        else
        {
            PlayBagarCardAnim(cardDelt.cardsDealt);

        }

        StartCoroutine(SetHistory(cardDelt));

    }
    IEnumerator SetHistory(Root cardDelt)
    {
        yield return new WaitForSeconds(0.5f);
        List<Sprite> andarSpriteList = new List<Sprite>();
        List<Sprite> baharSpriteList = new List<Sprite>();

        int andarCount = Mathf.Max(0, cardDelt.andarCards.Count - 1);
        int baharCount = Mathf.Max(0, cardDelt.baharCards.Count - 1);

        // ANDAR history
        for (int i = 0; i < andarCount; i++)
        {
            andarSpriteList.Add(
                CardSet(cardDelt.andarCards[i].suit, cardDelt.andarCards[i].rank)
            );
        }

        // BAHAR history
        for (int i = 0; i < baharCount; i++)
        {
            baharSpriteList.Add(
                CardSet(cardDelt.baharCards[i].suit, cardDelt.baharCards[i].rank)
            );
        }

        SetHistoryCards(andarSpriteList, baharSpriteList);

    }
    internal void EndLoop()
    {

        EndGameCorutine = StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        uiManager.Repeatpanel.SetActive(false);
        RoundInfo_Text.text = "<size=30>Bet Locked!</size>";
        if (StartGameCorutine != null)
        {
            StopCoroutine(StartGameCorutine);
            StartGameCorutine = null;
        }
        int delivered = socketManager.CardDelt.cardsDealt;

        if (socketManager.gameLoopData.matchSide == "andar")
        {
            uiManager.UpdateStats(socketManager.gameLoopData.middleCard.rank, true, delivered.ToString());
            if (delivered < 3)
            {
                FirstAndarTxt.HighlightedBG.SetActive(true);
                resultsOptions.Add(FirstAndarTxt);
            }
            resultsOptions.Add(AndarTxt);

        }
        else
        {
            resultsOptions.Add(BaharTxt);
            if (delivered < 3)
            {
                FirstBaharTxt.HighlightedBG.SetActive(true);
                resultsOptions.Add(FirstBaharTxt);
            }
            uiManager.UpdateStats(socketManager.gameLoopData.middleCard.rank, false, delivered.ToString());
        }
        uiManager.CalculateAndShowPercentage();
        PlayWinAnimations();


        yield return new WaitForSeconds(1f);


        Handanimator.Play("RemoveAllCard");
        ResetCardHistory();
        AndarHighLight.SetActive(false);
        BaharHighLight.SetActive(false);
        AndarBtnHighLight.SetActive(false);
        BaharBtnHighLight.SetActive(false);
        yield return new WaitForSeconds(1f);
        PlayResetAnimation();
        if (currentWin >= 1)
        {
            int winInt = Mathf.FloorToInt((float)currentWin);

            PlayPopup("You Won\n" + winInt);
            playtheCoin("+" + winInt);
        }

        currentWin = 0;
        ResetAllBetUI();
        // RoundInfoAnim(1);
        animHand.MiddleCard.gameObject.SetActive(false);
    }
    internal IEnumerator ManageFlushAnimation()
    {
        PlayFlushAnim(socketManager.FlushData.firstThreeResult, animHand.LeftSprite, animHand.MiddleSprite, animHand.RightSprite);
        yield return new WaitForSeconds(1f);
        FirstThreeTxt.HighlightedBG.SetActive(true);
        yield return new WaitForSeconds(4f);
        MainFlushObj.SetActive(false);
        resultsOptions.Add(FirstThreeTxt);
    }
    internal void ManagePayouts()
    {
        StartCoroutine(ManagePayout());
        StartCoroutine(NextRoundtext());
    }
    IEnumerator NextRoundtext(int time = 4)
    {
        pulseText.gameObject.SetActive(true);
        RoundInfoAnim(3);
        for (int i = time; i >= 0; i--)
        {
            RoundInfo_Text.text = "<size=30>Next Round</size>\n ";

            pulseText.text = $"<color=#00AB15>{i}</color>";

            yield return new WaitForSeconds(1f);
        }
        RoundInfoAnim(0);
        RoundInfo_Text.text = "  ";

        pulseText.text = " ";
    }
    IEnumerator ManagePayout()
    {

        MoveAllChipstohomeNew();
        yield return new WaitForSeconds(1f);

        DistributeAllPayout();
        SetOtherplayerData(socketManager.CashoutData.leaderboards);
        resultsOptions.Clear();
        yield return null;

        // MoveAllChipstohomeNew();
        // yield return new WaitForSeconds(1f);


        // SpawnPayoutOnWinningOptions(socketManager.CashoutData.payouts);
        // yield return new WaitForSeconds(1f);


        // MoveWinningChipsToPlayers(socketManager.CashoutData.payouts);
        // SetOtherplayerData(socketManager.CashoutData.leaderboards);
    }
    void MoveAllChipstohome()
    {
        foreach (var item in PlayerChips)
        {
            MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true);
        }
        foreach (var item in OtherPlayerChips)
        {
            MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true);
        }
    }
    void DistributeAllPayout()
    {
        DistributePayouts(socketManager.CashoutData.payouts);
    }




    void MoveAllChipstohomeNew()
    {
        foreach (var item in PlayerChips)
        {
            if (!resultsOptions.Contains(item.betoptions))
            {
                MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true);
            }
        }
        foreach (var item in OtherPlayerChips)
        {
            if (!resultsOptions.Contains(item.betoptions))
            {
                MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true);
            }
        }
    }


    public void DistributePayoutsNew(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            Transform target = FindPlayerTransform(payout.username);
            if (uiManager.MainPlayers.playername.text == payout.username)
            {
                int oldBalance = int.Parse(uiManager.MainPlayers.playerBalence.text);
                double newBalance = payout.balance;

                currentWin = newBalance - oldBalance;
                uiManager.MainPlayers.playerBalence.text = payout.balance.ToString();
                Debug.Log("Managing playerBet" + payout.balance);
                socketManager.playerdata.balance = payout.balance;
            }
            if (target == null)
                target = TotalPlayer_text.transform;

            SpawnPayoutChips(payout.win, target);
        }
    }

    void SpawnPayoutOnWinningOptions(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            if (payout.win <= 0)
                continue;

            // 🔹 Find player's winning bet
            var playerWinningChip = PlayerChips
                .FirstOrDefault(x =>
                    x.betId == payout.username &&
                    resultsOptions.Contains(x.betoptions));

            if (playerWinningChip == null)
            {
                // Mid-game join case
                // UpdateBalanceIfLocal(payout);
                continue;
            }
            List<int> roomChips = FindRoom();
            // 🔹 Break win into chip pieces
            List<int> chipPieces =
                BreakAmountIntoChips((int)payout.win, roomChips);
            foreach (int piece in chipPieces)
            {
                int index = findChipindex(piece, roomChips);
                ChipData data = new ChipData();
                data.betId = "";
                data.amount = piece;
                // 🔹 Spawn EXACTLY like bet placement
                data.chip = SpawnChip(
                      findChipSprite(piece, roomChips),
                     piece.ToString(),
                     index,
                     RoundInfo_Text.transform,              // START = Dealer
                     playerWinningChip.betoptions,          // TARGET OPTION
                     false,
                     0.4f
                 );

                // 🔹 Add to correct list
                PlayerChips.Add(data);
            }
        }
    }



    void MoveWinningChipsToPlayers(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            Transform target = FindPlayerTransform(payout.username);

            if (target == null)
                target = TotalPlayer_text.transform;

            var chipsToMove = PlayerChips
                .Where(x => resultsOptions.Contains(x.betoptions))
                .ToList();

            foreach (var chipData in chipsToMove)
            {
                Chip chip = chipData.chip.GetComponent<Chip>();
                MoveChip(chip, target, true);
            }

            // Update balance here (AFTER animation if you want)
            if (uiManager.MainPlayers.playername.text == payout.username)
            {
                int oldBalance = int.Parse(uiManager.MainPlayers.playerBalence.text);
                double newBalance = payout.balance;

                currentWin = newBalance - oldBalance;
                uiManager.MainPlayers.playerBalence.text = payout.balance.ToString();
                socketManager.playerdata.balance = payout.balance;
            }
        }
    }

    internal void ManageBonus()
    {
        Transform spawnPos = FindOption(socketManager.BonusData.bonus).gameObject.transform;
        BonusObject.transform.position = spawnPos.position;
        BonusObject.gameObject.SetActive(true);
    }



    void ManageCardCounts(int count)
    {
        CardCount_Text.text = count + "\nCards";


        int optionIndex = (count - 1) / 5;

        //  Debug.Log("Highlight option index = " + optionIndex);

        HighlightOption(optionIndex);
    }
    void HighlightOption(int index)
    {
        for (int i = 0; i < AllOptions.Count; i++)
        {
            bool active = (i == index);

            AllOptions[i].HighlightedBG.SetActive(active);
            AllOptions[i].BG.SetActive(!active);
        }
    }


    void PlayMiddleCardAnim()
    {
        animHand.MiddleSprite = CardSet(socketManager.gameLoopData.middleCard.suit, socketManager.gameLoopData.middleCard.rank);
        Handanimator.Play("MiddleCard");
        uiManager.CalculateStringProbability(socketManager.gameLoopData.middleCard.rank);
        // Debug.Log(socketManager.gameLoopData.middleCard.suit);
    }
    void PlayAndarCardAnim(int cardCount)
    {
        if (cardCount > 2)
        {
            AndarHighLight.SetActive(true);
            BaharHighLight.SetActive(false);
            AndarBtnHighLight.SetActive(true);
            BaharBtnHighLight.SetActive(false);
        }
        else StartCoroutine(delayedactive(true, false));
        animHand.LeftSprite = CardSet(socketManager.CardDelt.card.suit, socketManager.CardDelt.card.rank);
        Handanimator.Play("LeftCard");
    }
    void PlayBagarCardAnim(int cardCount)
    {
        if (cardCount > 2)
        {
            AndarHighLight.SetActive(false);
            BaharHighLight.SetActive(true);
            AndarBtnHighLight.SetActive(false);
            BaharBtnHighLight.SetActive(true);
        }
        else StartCoroutine(delayedactive(false, true));
        animHand.RightSprite = CardSet(socketManager.CardDelt.card.suit, socketManager.CardDelt.card.rank);
        Handanimator.Play("RightCard");
    }
    IEnumerator delayedactive(bool andar, bool bahar)
    {
        yield return new WaitForSeconds(1f);
        AndarHighLight.SetActive(andar);
        BaharHighLight.SetActive(bahar);
        AndarBtnHighLight.SetActive(andar);
        BaharBtnHighLight.SetActive(bahar);
    }
    private void MoveChip(Transform chip, Transform startPos, Transform endPos, bool disableOnEnd, float duration = 0.4f)
    {
        if (chip == null || startPos == null || endPos == null)
        {
            Debug.LogError("ChipMover: One of the transforms is null!");
            return;
        }

        chip.position = startPos.position;
        chip.DOMove(endPos.position, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (disableOnEnd)
                {
                    ReturnChip(chip.gameObject.GetComponent<Chip>());
                }
            });
    }

    #endregion




    #region  beting
    internal void onClickOption(GameObject option)
    {

        OptionPrefab optionprefab = option.GetComponent<OptionPrefab>();


        int index = uiManager.coinSelector.chipIndex;
        double chipValue;
        if (double.TryParse(uiManager.coinSelector.Chiptext.text, out chipValue))
        {
            if (chipValue > socketManager.playerdata.balance)
            {
                PlayPopup("Low Balance");
                // Low balance logic here
                Debug.Log("Insufficient balance");

                return;
            }
        }
        socketManager.BetPlaced(index, optionprefab.VaridontWant, socketManager.initialData.betOptions[optionprefab.Optionindex]);
        uiManager.RetractCoins();
        isRepeatbetActive = true;

    }
    internal void ManageBrodcastBetsPlayer()
    {
        uiManager.SetChipoption(true);
        uiManager.Repeatpanel.SetActive(false);
        int totalAmount = socketManager.BetChipData.payload.amount;

        // Get chip denominations for current room
        List<int> roomChips = FindRoom();

        // Break into multiple chips
        List<int> chipPieces = BreakAmountIntoChips(totalAmount, roomChips);

        foreach (int chipAmount in chipPieces)
        {
            ChipData data = new ChipData();
            data.betId = socketManager.BetChipData.payload.betId;
            data.amount = chipAmount;
            data.betoptions = FindOption(socketManager.BetChipData.payload.betOption);

            int index = findChipindex(chipAmount, roomChips);
            string val = chipAmount.ToString();

            Debug.Log("Spawning Chip index=" + index + " amount=" + val);

            data.chip = SpawnChip(
                findChipSprite(chipAmount, roomChips),
                val,
                index,
                uiManager.coinSelector.transform,
                FindOption(socketManager.BetChipData.payload.betOption),
                true
            );

            PlayerChips.Add(data);
            UpdateMyBetOnOption(socketManager.BetChipData.payload.betOption, chipAmount);
            UpdateTotalBetOnOption(socketManager.BetChipData.payload.betOption, chipAmount);
        }

        audioManager.PlayWLAudio("double");
    }
    internal void ManageBrodcastBetsOtherPlayers(Root chipdata)
    {
        if (chipdata.amount < 0)
        {
            ClearOtherPlayerbets(chipdata);

            return;
        }
        // Do not show own chip here
        if (chipdata.username == uiManager.MainPlayers.playername.text)
            return;

        List<int> roomChips = FindRoom();
        int totalAmount = chipdata.amount;

        // Break large amount into individual chips
        List<int> chipPieces = BreakAmountIntoChips(totalAmount, roomChips);

        foreach (int piece in chipPieces)
        {
            int index = findChipindex(piece, roomChips);

            string val = piece.ToString();

            ChipData data = new ChipData();
            data.betId = chipdata.betId;
            data.amount = piece;
            data.betoptions = FindOption(chipdata.payload.betOption);

            data.chip = SpawnChip(
                findOtherPlayerChipSprite(piece, roomChips),
                val,
                index,
                TotalPlayer_text.transform,
                FindOption(chipdata.betOption)
            );
            data.chip.transform.SetParent(OtherPlayerChipPoolParent);
            OtherPlayerChips.Add(data);
            UpdateTotalBetOnOption(chipdata.betOption, piece);
        }
    }

    void ClearOtherPlayerbets(Root chipdata)
    {


        foreach (var chips in OtherPlayerChips)
        {
            if (chips.betId == chipdata.betId)
            {
                if (chips.chip != null)
                {
                    Chip c = chips.chip.GetComponent<Chip>();
                    ReturnChip(c);
                    UpdateTotalBetOnOption(chipdata.betOption, chipdata.amount);
                    break;
                }
            }
        }
        // PlayerChips.Clear();
        //  ResetAllBetUI();

    }


    GameObject SpawnChip(Sprite sprite, string amount, int chipindex, Transform startPoint, OptionPrefab op, bool isPlayerbet = false, float moveTime = 0.4f)
    {
        Chip chip = GetChip();
        chip.SetData(sprite, amount, chipindex);

        RectTransform chipRT = chip.GetComponent<RectTransform>();
        chipRT.SetParent(poolParent);
        chipRT.localScale = Vector3.one;

        Vector2 size = op.chiparea.rect.size;

        Vector2 randomPos = new Vector2(
            UnityEngine.Random.Range(-size.x * 0.5f, size.x * 0.5f),
            UnityEngine.Random.Range(-size.y * 0.5f, size.y * 0.5f)
        );

        Vector3 worldRandomPos = op.chiparea.TransformPoint(randomPos);

        if (!isPlayerbet && startPoint != null)
        {
            // Animated spawn
            chipRT.position = startPoint.position;
            chipRT.DOMove(worldRandomPos, moveTime);
        }
        else
        {
            // Direct spawn at correct world position
            chipRT.position = worldRandomPos;
        }

        return chip.gameObject;

        // Chip chip = GetChip();
        // chip.SetData(sprite, amount, chipindex);

        // RectTransform chipRT = chip.GetComponent<RectTransform>();
        // chipRT.SetParent(poolParent);
        // chipRT.localScale = Vector3.one;

        // Vector2 size = op.chiparea.rect.size;
        // Vector2 randomPos = new Vector2(
        //     UnityEngine.Random.Range(-size.x * 0.5f, size.x * 0.5f),
        //     UnityEngine.Random.Range(-size.y * 0.5f, size.y * 0.5f)
        // );

        // if (!isPlayerbet)
        // {
        //     chipRT.position = startPoint.position;
        //     chipRT.DOMove(op.chiparea.TransformPoint(randomPos), moveTime);
        // }
        // else chipRT.position = randomPos;

        // return chip.gameObject;
    }








    #endregion


    #region Manage Result and reset
    void PlayWinAnimations()
    {
        AndarTxt.winAnimation.StopAnimation();
        BaharTxt.winAnimation.StopAnimation();
        if (socketManager.gameLoopData.matchSide == "andar") AndarTxt.winAnimation.StartAnimation();
        else BaharTxt.winAnimation.StartAnimation();

        foreach (var item in AllOptions)
        {
            item.winAnimation.StopAnimation();
            if (item.HighlightedBG.activeInHierarchy)
            {
                item.HighlightedBG.SetActive(true);
                item.winAnimation.StartAnimation();
                StartCoroutine(item.Highlighttext());
            }
        }
    }
    void ResetCardHistory()
    {
        foreach (var item in AndarCardparent)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in BaharCardparent)
        {
            item.gameObject.SetActive(false);

        }
    }
    void PlayResetAnimation()
    {
        audioManager.PlayGirlAudio("newround");
        RectTransform rt = AndarbaharBetReset.GetComponent<RectTransform>();

        float startX = rt.anchoredPosition.x;
        float targetX = -766f;

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPosX(targetX, 0.8f).SetEase(Ease.OutBounce))


           .AppendInterval(0.01f)
            .AppendCallback(() =>
           {
               NewRoundAnim.StopAnimation();
               NewRoundAnim.StartAnimation();
           })
           .AppendInterval(1.5f)
           .AppendCallback(() =>
           {

               CardCount_Text.text = "";
               RoundInfo_Text.text = "";
               AndarTxt.SetData(0, "andar", "", "main_bets");
               BaharTxt.SetData(1, "bahar", "", "main_bets");
               AndarTxt.DontShowText();
               BaharTxt.DontShowText();
               AndarTxt.winAnimation.StopAnimation();
               BaharTxt.winAnimation.StopAnimation();
           })


           .Append(rt.DOAnchorPosX(startX, 0.6f).SetEase(Ease.InOutSine));


        foreach (var item in AllOptions)
        {
            item.BG.SetActive(true);
            item.HighlightedBG.SetActive(false);
        }
        BonusObject.gameObject.SetActive(false);
    }


    private Chip SpawnChipFromPool()
    {
        Chip chip = GetChip();   // your pool code
        chip.transform.SetParent(poolParent);
        chip.transform.position = RoundInfo_Text.transform.position;
        return chip;
    }

    private void MoveChip(Chip chip, Transform target, bool returnToPool)
    {
        chip.transform.DOMove(target.position, 0.6f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (returnToPool)
                {
                    ReturnChip(chip);
                }
            });
    }

    private Transform FindPlayerTransform(string playerId)
    {
        Debug.Log($"[FindPlayerTransform] Searching for PlayerId: {playerId}");

        // --- Check Main Player ---
        if (uiManager.MainPlayers != null)
        {
            Debug.Log($"[MainPlayer] ID: {uiManager.MainPlayers.playername.text}");
            if (uiManager.MainPlayers.playername.text == playerId)
            {
                Debug.Log("[RESULT] Found in MainPlayers");
                return uiManager.MainPlayers.transform;
            }
        }
        else
        {
            Debug.LogWarning("[FindPlayerTransform] MainPlayers is NULL!");
        }

        // --- Check Richest Players ---
        foreach (var p in uiManager.RichestPlayers)
        {
            if (p == null)
            {
                Debug.LogWarning("[RichestPlayers] One entry is NULL!");
                continue;
            }

            Debug.Log($"[Richest] ID: {p.PlayerId}");
            if (p.playername.text == playerId)
            {
                Debug.Log("[RESULT] Found in RichestPlayers");
                return p.transform;
            }
        }

        // --- Check Winner Players ---
        foreach (var p in uiManager.WinnerPlayers)
        {
            if (p == null)
            {
                Debug.LogWarning("[WinnerPlayers] One entry is NULL!");
                continue;
            }

            Debug.Log($"[Winners] ID: {p.PlayerId}");
            if (p.playername.text == playerId)
            {
                Debug.Log("[RESULT] Found in WinnerPlayers");
                return p.transform;
            }
        }

        Debug.LogWarning($"[FindPlayerTransform] PlayerId {playerId} NOT FOUND!");
        return null;
    }


    public void DistributePayouts(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            Transform target = FindPlayerTransform(payout.username);
            if (uiManager.MainPlayers.playername.text == payout.username)
            {
                int oldBalance = int.Parse(uiManager.MainPlayers.playerBalence.text);
                double newBalance = payout.balance;

                currentWin = newBalance - oldBalance;
                uiManager.MainPlayers.playerBalence.text = payout.balance.ToString();
                Debug.Log("Managing playerBet" + payout.balance);
                socketManager.playerdata.balance = payout.balance;
            }
            if (target == null)
                target = TotalPlayer_text.transform;

            SpawnPayoutChips(payout.win, target);
        }
    }

    private void SpawnPayoutChips(float amount, Transform target)
    {
        if (amount <= 0) return;

        Chip chip = SpawnChipFromPool();
        MoveChip(chip, target, true);
    }










    #endregion






    #region  manage BEt Double bet cancle &&& undo
    internal void RepeAtBet(List<Bet> bets)
    {
        audioManager.PlayWLAudio("double");
        Debug.Log("RepeatBet started");

        List<int> roomChips = FindRoom(); // chip denominations

        foreach (var bet in bets)
        {

            int amount = bet.amount;

            // Break amount into multiple chips
            List<int> chipPieces = BreakAmountIntoChips(amount, roomChips);

            foreach (int piece in chipPieces)
            {
                ChipData data = new ChipData();
                data.betId = bet.betId;
                data.amount = piece;
                data.betoptions = FindOption(bet.betOption);

                string val = piece.ToString();
                int index = findChipindex(piece, roomChips);

                if (index <= 5)   // your existing condition
                {
                    data.chip = SpawnChip(
                        findChipSprite(piece, roomChips),
                        val,
                        index,
                        uiManager.coinSelector.transform,
                        FindOption(bet.betOption),
                        true
                    );

                    PlayerChips.Add(data);
                }
                UpdateMyBetOnOption(bet.betOption, piece);
                UpdateTotalBetOnOption(bet.betOption, piece);

            }
        }
        uiManager.SetChipoption(true);
    }
    internal void DoubleBets(List<Bet> bets)
    {
        audioManager.PlayWLAudio("double");

        List<int> roomChips = FindRoom(); // chip denominations

        foreach (var bet in bets)
        {
            if (bet.delta > 0)
            {
                int amount = bet.oldAmount;

                // Break amount into multiple chips
                List<int> chipPieces = BreakAmountIntoChips(amount, roomChips);

                foreach (int piece in chipPieces)
                {
                    ChipData data = new ChipData();
                    data.betId = bet.betId;
                    data.amount = piece;
                    data.betoptions = FindOption(bet.betOption);

                    string val = piece.ToString();
                    int index = findChipindex(piece, roomChips);

                    if (index <= 5)   // your existing condition
                    {
                        data.chip = SpawnChip(
                            findChipSprite(piece, roomChips),
                            val,
                            index,
                            uiManager.coinSelector.transform,
                            FindOption(bet.betOption),
                            true
                        );

                        PlayerChips.Add(data);
                    }
                    UpdateMyBetOnOption(bet.betOption, piece);
                    UpdateTotalBetOnOption(bet.betOption, piece);
                }
            }
        }
    }

    internal void ClearAllBets()
    {
        StartCoroutine(CancleBets());
        foreach (var chips in OtherPlayerChips)
        {
            Chip c = chips.chip.GetComponent<Chip>();
            if (chips != null)
                ReturnChip(c);
        }
        OtherPlayerChips.Clear();
        ResetAllBetUI();
    }
    internal IEnumerator CancleBets()
    {
        foreach (var chipObj in PlayerChips)
        {
            chipObj.chip.transform.DOMove(uiManager.MainPlayers.gameObject.transform.position, 0.3f);


        }
        yield return new WaitForSeconds(0.5f);
        foreach (var chips in PlayerChips)
        {
            Chip c = chips.chip.GetComponent<Chip>();
            if (chips != null)
                ReturnChip(c);
        }
        PlayerChips.Clear();
        //  ResetAllBetUI();
        for (int i = 0; i < AllOptions.Count; i++)
        {
            int newAmount = AllOptions[i].totalBet - AllOptions[i].playerBet;
            if (newAmount <= 0) AllOptions[i].TotalBetObj.SetActive(false);
            AllOptions[i].TotalBetText.text = newAmount.ToString();
            AllOptions[i].totalBet = newAmount;
            AllOptions[i].MyBetObj.SetActive(false);
            AllOptions[i].MyBetText.text = "0";
            AllOptions[i].playerBet = 0;


        }
        for (int i = 0; i < BiggerOptions.Count; i++)
        {
            int newAmount = BiggerOptions[i].totalBet - BiggerOptions[i].playerBet;
            if (newAmount <= 0) BiggerOptions[i].TotalBetObj.SetActive(false);
            BiggerOptions[i].TotalBetText.text = newAmount.ToString();
            BiggerOptions[i].totalBet = newAmount;
            BiggerOptions[i].MyBetObj.SetActive(false);
            BiggerOptions[i].MyBetText.text = "0";
            BiggerOptions[i].playerBet = 0;


        }
    }
    internal IEnumerator UnduBets(string betId)
    {
        for (int i = PlayerChips.Count - 1; i >= 0; i--)
        {
            var item = PlayerChips[i];

            if (item.betId == betId)
            {
                int amount = item.amount;
                string option = socketManager.BetChipData.payload.betOption; // ⚠ correct option

                // 1. Return chip
                if (item.chip != null)
                {
                    Chip c = item.chip.GetComponent<Chip>();

                    item.chip.transform.DOMove(uiManager.MainPlayers.gameObject.transform.position, 0.3f);

                    yield return new WaitForSeconds(0.5f);
                    ReturnChip(c);
                }

                // 2. Remove from list
                PlayerChips.RemoveAt(i);
                if (item.betoptions != null)
                    // 3. UPDATE UI ⭐
                    UpdateMyBetOnOption(option, -amount, item.betoptions);     // subtract
                UpdateTotalBetOnOption(option, -amount, item.betoptions);  // subtract
            }
        }
    }
    internal void UndoBetsFast(string betId)
    {
        if (PlayerChips == null || PlayerChips.Count == 0)
            return;

        var chipsToUndo = PlayerChips
            .Where(x => x.betId == betId)
            .ToList();   // copy for safety

        foreach (var item in chipsToUndo)
        {
            AnimateAndRemove(item);
        }
    }
    void AnimateAndRemove(ChipData item)
    {
        if (item == null || item.chip == null)
            return;

        int amount = item.amount;
        OptionPrefab option = item.betoptions;

        Chip chipComponent = item.chip.GetComponent<Chip>();

        item.chip.transform
            .DOMove(uiManager.MainPlayers.transform.position, 0.25f)
            .OnComplete(() =>
            {
                // Return to pool
                ReturnChip(chipComponent);

                // Remove from list
                PlayerChips.Remove(item);

                // Update UI
                UpdateMyBetOnOption("", -amount, option);
                UpdateTotalBetOnOption("", -amount, option);
            });
    }
    #endregion



    #region add a card in list
    public void SetHistoryCards(
    List<Sprite> andarSprites,
    List<Sprite> baharSprites)
    {
        PopulateCardList(AndarCardparent, ref AndarIndex, andarSprites);
        PopulateCardList(BaharCardparent, ref BaharIndex, baharSprites);
    }
    private void PopulateCardList(
        List<Image> list,
        ref int index,
        List<Sprite> spritesFromServer)
    {
        ResetCardList(list, ref index);

        int count = Mathf.Min(list.Count, spritesFromServer.Count);

        for (int i = 0; i < count; i++)
        {
            list[i].sprite = spritesFromServer[i];
            list[i].gameObject.SetActive(true);

            var outline = list[i].GetComponent<Outline>();
            if (i == count - 1) outline.enabled = true;
        }

        index = count;
    }

    public void AddAndarCard(Sprite newSprite)
    {
        if (audioManager) audioManager.PlayWLAudio("cards");
        PlayPopAndSwap(AndarCardparent, ref AndarIndex, newSprite);
    }

    public void AddBaharCard(Sprite newSprite)
    {
        if (audioManager) audioManager.PlayWLAudio("cards");
        PlayPopAndSwap(BaharCardparent, ref BaharIndex, newSprite);
    }

    private void PlayPopAndSwap(List<Image> list, ref int index, Sprite newSprite)
    {
        if (list.Count == 0) return;

        // Shift sprites UP (towards 0)
        for (int i = 0; i < list.Count - 1; i++)
        {
            list[i].sprite = list[i + 1].sprite;
            list[i].gameObject.SetActive(list[i + 1].gameObject.activeSelf);
        }

        // New card always goes to LAST index
        Image lastCard = list[list.Count - 1];
        lastCard.sprite = newSprite;
        lastCard.gameObject.SetActive(true);

        // Animate new card only
        lastCard.transform.localScale = Vector3.one;
        lastCard.transform.DOScale(popScale, animTime)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                lastCard.transform.DOScale(1f, animTime * 0.5f);
            });

        // Logical index (if needed)
        index = Mathf.Min(index + 1, list.Count);
    }



    private void ResetCardList(List<Image> list, ref int index)
    {
        foreach (var img in list)
        {
            img.sprite = null;
            // img.gameObject.SetActive(false);
            var outline = img.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
            img.transform.localScale = Vector3.one;
        }
        index = 0;
    }



    #endregion





    #region chip Pool







    GameObject AddChip()
    {
        var go = Instantiate(chipPrefab, poolParent);
        go.SetActive(false);
        pool.Add(go);
        return go;
    }

    // internal Chip GetChip()
    // {
    //     // Try to reuse inactive chip
    //     for (int i = 0; i < pool.Count; i++)
    //     {
    //         if (!pool[i].activeInHierarchy)
    //         {
    //             pool[i].SetActive(true);
    //             return pool[i].GetComponent<Chip>();
    //         }
    //     }

    //     // If all are in use, expand pool BEFORE returning
    //     for (int i = 0; i < 10; i++)
    //         AddChip();

    //     // Guaranteed available now
    //     pool[^1].SetActive(true);
    //     return pool[^1].GetComponent<Chip>();
    // }
    int FreeChipCount()
    {
        int free = 0;
        for (int i = 0; i < pool.Count; i++)
            if (!pool[i].activeInHierarchy)
                free++;

        return free;
    }
    void EnsureFreeChips()
    {
        if (FreeChipCount() >= 5)
            return;

        int need = 5 - FreeChipCount();
        int expandCount = Mathf.Max(need, 10);

        for (int i = 0; i < expandCount; i++)
            AddChip();
    }
    internal Chip GetChip()
    {
        // Make sure free chips exist BEFORE using
        EnsureFreeChips();

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i].GetComponent<Chip>();
            }
        }

        return null; // logically unreachable
    }
    internal void ReturnChip(Chip chip)
    {
        chip.gameObject.SetActive(false);
        chip.transform.SetParent(poolParent);
    }





    #endregion






    #region cardSelection
    internal Sprite CardSet(string suit, string value)
    {

        Sprite tempSprite = null;
        switch (suit.ToUpper())
        {
            case "HEARTS":
                tempSprite = GetCardSprite(HeartSpriteList, value);
                break;
            case "DIAMONDS":
                tempSprite = GetCardSprite(DiamondSpriteList, value);
                break;
            case "CLUBS":
                tempSprite = GetCardSprite(ClubSpriteList, value);
                break;
            case "SPADES":
                tempSprite = GetCardSprite(SpadeSpriteList, value);
                break;
            default:
                Debug.LogError("Invalid Suit: " + suit);
                break;
        }
        //  Debug.Log("#----------------------------------------------------_# ");   // <== ADD
        //   Debug.Log("##SUIT = " + suit);     // <== ADD
        //   Debug.Log("##VALUE = " + value);   // <== ADD

        return tempSprite;
    }

    // Helper function to get the correct sprite from a sprite list based on value
    private Sprite GetCardSprite(List<Sprite> spriteList, string value)
    {
        switch (value.ToUpper())
        {
            case "A": return spriteList[0];
            case "K": return spriteList[12];
            case "Q": return spriteList[11];
            case "J": return spriteList[10];
            default:
                int myval = int.Parse(value);
                //      Debug.Log("##index = " + (myval - 1));   // <== ADD
                //  Debug.Log("#----------------------------------------------------_# ");   // <== ADD
                return spriteList[myval - 1];
        }
    }

    #endregion

    #region Flush Animation

    void PlayFlushAnim(int type, Sprite fCard, Sprite sCard, Sprite tCard)
    {
        FlushImageAnim.StopAnimation();
        FlushImageAnim.textureArray.Clear();
        FlushImageAnim.textureArray.TrimExcess();

        DropImageAnim.StopAnimation();
        DropImageAnim.textureArray.Clear();
        DropImageAnim.textureArray.TrimExcess();

        if (type == 9)
        {
            for (int i = 0; i < StraightFlushSprite.Count; i++)
            {

                FlushImageAnim.textureArray.Add(StraightFlushSprite[i]);
            }
            for (int i = 0; i < yellowDrop.Count; i++)
            {
                DropImageAnim.textureArray.Add(yellowDrop[i]);
            }
        }
        else if (type == 6)
        {
            for (int i = 0; i < FlushSprite.Count; i++)
            {

                FlushImageAnim.textureArray.Add(FlushSprite[i]);
            }
            for (int i = 0; i < purpleDrop.Count; i++)
            {
                DropImageAnim.textureArray.Add(purpleDrop[i]);
            }
        }
        else
        {
            for (int i = 0; i < StraightSprite.Count; i++)
            {

                FlushImageAnim.textureArray.Add(StraightSprite[i]);
            }
            for (int i = 0; i < pinkDrop.Count; i++)
            {
                DropImageAnim.textureArray.Add(pinkDrop[i]);
            }
        }
        FlushCardOne.sprite = fCard;
        FlushCardTwo.sprite = sCard;
        FlushCardThree.sprite = tCard;
        MainFlushObj.SetActive(true);
        FlushImageAnim.StartAnimation();
        DropImageAnim.StartAnimation();
        FlushCard.Play("CardDown");

    }



    #endregion

    #region helper
    List<int> BreakAmountIntoChips(int amount, List<int> chipOptions)
    {
        // ✅ Make a COPY so original list is not modified
        List<int> sortedChips = new List<int>(chipOptions);

        // Sort descending
        sortedChips.Sort((a, b) => b.CompareTo(a));

        List<int> results = new List<int>();

        foreach (int chip in sortedChips)
        {
            while (amount >= chip)
            {
                amount -= chip;
                results.Add(chip);
            }
        }

        return results;
    }


    internal void UpdatePlayerbalance(string balance)
    {
        uiManager.MainPlayers.playerBalence.text = balance;
    }
    Sprite findChipSprite(int amount, List<int> betOptions)
    {
        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] == amount)
            {
                return PlayerChipSprite[i];
            }
        }
        return null;

    }
    Sprite findOtherPlayerChipSprite(int amount, List<int> betOptions)
    {
        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] == amount)
            {
                return otherChipSprite[i];
            }
        }
        return null;

    }
    int findChipindex(int amount, List<int> betOptions)
    {
        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] == amount)
            {
                return i;
            }
        }
        return 0;

    }
    List<int> FindRoom()
    {
        string room = currentRoom;
        List<int> data = null;

        switch (room)
        {
            case "casual":
                data = socketManager.initialData.bets.casual;
                break;

            case "novice":
                data = socketManager.initialData.bets.novice;
                break;

            case "expert":
                data = socketManager.initialData.bets.expert;
                break;

            case "high_roller":
                data = socketManager.initialData.bets.high_roller;
                break;
        }
        return data;
    }

    OptionPrefab FindOption(string opt)
    {
        // Debug.Log("789 _____________" + opt);
        switch (opt)
        {
            case "s_1_5":
                return OptionQTxt;

            case "s_6_10":
                return OptionWTxt;

            case "s_11_15":
                return OptionETxt;

            case "s_16_20":
                return OptionRTxt;

            case "s_21_25":
                return OptionTTxt;

            case "s_26_30":
                return OptionYTxt;

            case "s_31_35":
                return OptionUTxt;

            case "s_36_40":
                return OptionITxt;

            case "s_41_53":
                return OptionOTxt;

            case "andar":
                return AndarTxt;

            case "bahar":
                return BaharTxt;

            case "first_1_andar":
                return FirstAndarTxt;

            case "first_1_bahar":
                return FirstBaharTxt;

            case "first_3":
                return FirstThreeTxt;


            default:
                return FirstAndarTxt;

        }
    }

    internal void PlayPopup(string popupText)
    {
        BlockerText.text = popupText;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PopupRoutine());
    }
    internal void playtheCoin(string winamount)
    {
        coinAddText.text = winamount;

        // Kill previous animation if running
        coinTween?.Kill();

        // Reset state
        coinAddText.rectTransform.localPosition = startPoscoin;
        coinAddText.color = startColor;
        coinAddText.gameObject.SetActive(true);

        // Animate
        coinTween = DOTween.Sequence()
            .Append(coinAddText.rectTransform
                .DOLocalMoveY(startPos.y + moveY, duration))
            .Join(coinAddText
                .DOFade(0f, duration))
            .OnComplete(() =>
            {
                // Reset after animation
                coinAddText.gameObject.SetActive(false);
                coinAddText.rectTransform.localPosition = startPos;
                coinAddText.color = startColor;
            });
    }

    private IEnumerator PopupRoutine()
    {
        BlockerObj.transform.position = popStart.position;
        yield return Move(BlockerObj.transform, popCenter.position, moveDuration);

        yield return new WaitForSeconds(holdDuration);

        yield return Move(BlockerObj.transform, popEnd.position, moveDuration);
    }

    private IEnumerator Move(Transform target, Vector3 toPos, float duration)
    {
        Vector3 fromPos = target.position;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            target.position = Vector3.Lerp(fromPos, toPos, lerp);
            yield return null;
        }

        target.position = toPos;
    }

    internal void UpdateMyBetOnOption(string opt, int amount, OptionPrefab optn = null)
    {
        OptionPrefab option = FindOption(opt);
        if (optn != null) option = optn;
        if (option == null) return;
        option.MyBetObj.SetActive(true);

        // int prev = 0;
        // int.TryParse(option.MyBetText.text, out prev);

        int newAmount = option.playerBet + amount;
        if (newAmount <= 0) option.MyBetObj.SetActive(false);
        option.MyBetText.text = newAmount.ToString();
        option.playerBet = newAmount;
    }
    internal void UpdateTotalBetOnOption(string opt, int amount, OptionPrefab optn = null)
    {
        OptionPrefab option = FindOption(opt);
        if (optn != null) option = optn;
        if (option == null) return;

        option.TotalBetObj.SetActive(true);

        // int prev = 0;
        // int.TryParse(option.TotalBetText.text, out prev);

        int newAmount = option.totalBet + amount;
        if (newAmount <= 0) option.TotalBetObj.SetActive(false);
        option.TotalBetText.text = newAmount.ToString();
        option.totalBet = newAmount;
    }

    internal void ResetBetUI(OptionPrefab option)
    {
        if (option == null) return;

        // Hide My Bet
        option.MyBetObj.SetActive(false);
        option.MyBetText.text = "0";

        // Hide Total Bet
        option.TotalBetObj.SetActive(false);
        option.TotalBetText.text = "0";
    }
    internal void ResetAllBetUI()
    {
        ResetBetUI(OptionQTxt);
        ResetBetUI(OptionWTxt);
        ResetBetUI(OptionETxt);
        ResetBetUI(OptionRTxt);
        ResetBetUI(OptionTTxt);
        ResetBetUI(OptionYTxt);
        ResetBetUI(OptionUTxt);
        ResetBetUI(OptionITxt);
        ResetBetUI(OptionOTxt);

        ResetBetUI(AndarTxt);
        ResetBetUI(BaharTxt);

        ResetBetUI(FirstAndarTxt);
        ResetBetUI(FirstBaharTxt);
        ResetBetUI(FirstThreeTxt);

        foreach (var opt in AllOptions)
        {
            opt.totalBet = 0;
            opt.playerBet = 0;
        }
        foreach (var opt in BiggerOptions)
        {
            opt.totalBet = 0;
            opt.playerBet = 0;
        }
    }


    #endregion
    internal void SetPlayerCountOnReturn(Lobby lobby, double playerbalance)
    {
        homepage.PlayerBalance.text = playerbalance.ToString();
        homepage.CPlayerCount.text = lobby.casual.ToString();
        homepage.NPlayerCount.text = lobby.novice.ToString();
        homepage.EPlayerCount.text = lobby.expert.ToString();
        homepage.HPlayerCount.text = lobby.high_roller.ToString();
        homepage.TotalPlayerCount.text = (lobby.casual + lobby.novice + lobby.expert + lobby.high_roller).ToString();
    }
    public void RoundInfoAnim(int spriteIndex)
    {
        Image img = RoundInfoImage.GetComponent<Image>();
        RectTransform rect = RoundInfoImage.GetComponent<RectTransform>();
        if (spriteIndex < 0 || spriteIndex >= roundInfoSprites.Count)
            return;


        img.sprite = roundInfoSprites[spriteIndex];


        rect.DOKill();
        rect.localScale = Vector3.one;


        rect.DOScale(popScale, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                rect.DOScale(1f, 0.2f)
                    .SetEase(Ease.InOutQuad);
            });
    }

    internal void OnClickNextroom()
    {
        if (nextRoom != currentRoom)
        {
            uiManager.RetractCoins();
            directJump = true;
            //IsMenuPanelOpen = false; 
            socketManager.SendHome();
            //  ResetMenuPanel(false);
            isRepeatbetActive = false;
        }
        else
        {
            uiManager.ClosePopup(uiManager.BetLimitPanel);
        }
    }
}

[System.Serializable]
public class ChipData
{
    public string betId;
    public string username;
    public int amount;
    public OptionPrefab betoptions;
    public GameObject chip;
}