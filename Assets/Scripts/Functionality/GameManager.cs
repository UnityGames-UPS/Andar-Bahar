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
    [SerializeField] internal GameObject HomePage;
    [SerializeField] internal GameObject GamePage;

    [Header("ScriptRef")]
    [SerializeField] private Homepage homepage;
    [SerializeField] SocketIOManager socketManager;
    [SerializeField] internal UiManager uiManager;
    [SerializeField] AudioManager audioManager;


    [Header("Texts")]
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
    [SerializeField] private List<Sprite> HeartSpriteListDisplay;
    [SerializeField] private List<Sprite> DiamondSpriteListDisplay;
    [SerializeField] private List<Sprite> ClubSpriteListDisplay;
    [SerializeField] private List<Sprite> SpadeSpriteListDisplay;
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
    // Level-specific chip sprites (6 sprites per level)
    [Header("Player Chip Sprites (6 sprites each level)")]
    [SerializeField] private List<Sprite> PlayerChipSprite_Casual;
    [SerializeField] private List<Sprite> PlayerChipSprite_Novice;
    [SerializeField] private List<Sprite> PlayerChipSprite_Expert;
    [SerializeField] private List<Sprite> PlayerChipSprite_HighRoller;
    [Header(" Chip SelectionSprites (6 sprites each level)")]
    [SerializeField] private List<Sprite> HighlightedChipSprite_Casual;
    [SerializeField] private List<Sprite> HighlightedChipSprite_Novice;
    [SerializeField] private List<Sprite> HighlightedChipSprite_Expert;
    [SerializeField] private List<Sprite> HighlightedChipSprite_HighRoller;

    [Header("Other Player Chip Sprites (6 sprites each level)")]
    [SerializeField] private List<Sprite> OtherChipSprite_Casual;
    [SerializeField] private List<Sprite> OtherChipSprite_Novice;
    [SerializeField] private List<Sprite> OtherChipSprite_Expert;
    [SerializeField] private List<Sprite> OtherChipSprite_HighRoller;
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
    [SerializeField] private Image coinImage;
    [SerializeField] private float moveY = 30f;
    [SerializeField] private float duration = 0.5f;

    private Vector3 startPos;
    private Color startColor;
    private Tween coinTween;
    private Tween popTween;
    private OptionPrefab ResultOption;
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
    [SerializeField] private ImageAnimation SparkAnim;
    [SerializeField] private Sprite HandResetSprite;
    [Header("Round Info ")]
    [SerializeField] private GameObject RoundInfoImage;
    [SerializeField] private TMP_Text pulseText;
    [SerializeField] private List<Sprite> roundInfoSprites;
    [Header("Bonus  ")]
    [SerializeField] internal GameObject BonusObject;

    internal List<OptionPrefab> resultsOptions = new List<OptionPrefab>();

    private float popScale = 1.20f;
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
    private Coroutine repeatPanelCoroutine; // controls the 5-sec repeat panel window

    // Card animation queue — ensures zero gap between consecutive dealt-card animations
    private Queue<System.Action> cardAnimQueue = new Queue<System.Action>();
    private Coroutine cardAnimCoroutine = null;

    private Vector3 startPoscoin;
    private Vector3 endPos = new Vector3(0, -10, 0);

    private int previousNextRoundTimerValue = -1; // Track previous next round timer to detect actual end vs join mid-round


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
        GamePage.SetActive(false);
        uiManager.MenuInGame_button.gameObject.SetActive(false);
        BetBlocker.onClick.RemoveAllListeners();
        BetBlocker.onClick.AddListener(() => PlayPopup("This Round is already closed.\nPlease wait for next round."));
        startPoscoin = coinAddText.rectTransform.localPosition;
        startColor = coinAddText.color;
        coinAddText.gameObject.SetActive(false);
        // animHand.MiddleCard.gameObject.SetActive(false);

    }


    #region  DataSetup
    internal void SetInitialData()
    {
        uiManager.SetgameRulePanel();
        homepage.SetInitHomedata(socketManager.initialData);
        SetPlayerData(socketManager.playerdata);
    }



    internal void OnGameLoaded()
    {
        // ✅ FIX: Ensure home screen is hidden when game screen activates
        // This handles cases where game events arrive before room join completes
        if (HomePage.activeSelf)
        {
            HomePage.SetActive(false);
        }

        GamePage.SetActive(true);



    }

    /// <summary>
    /// ✅ FIX: Ensures smooth transition from Home → Game screen before chip distribution
    /// Prevents chips from appearing on home screen when joining mid-round
    /// </summary>
    internal void TransitionToGameScreen(RoundState roundState, List<Bet> bets, System.Action onComplete)
    {
        StartCoroutine(TransitionToGameScreenCoroutine(roundState, bets, onComplete));
    }

    private IEnumerator TransitionToGameScreenCoroutine(RoundState roundState, List<Bet> bets, System.Action onComplete)
    {
        // Step 1: Hide home screen
        HomePage.SetActive(false);

        // Step 2: Activate game screen
        GamePage.SetActive(true);

        // Step 3: Wait for Canvas to rebuild layout (critical for chip positions)
        yield return null;  // Wait 1 frame for Canvas layout
        yield return null;  // Wait 1 more frame to ensure RectTransforms are ready

        // Step 4: Now apply mid-round state (if joining mid-round)
        if (roundState != null)
        {
            ApplyMidRoundState(roundState, bets);
        }

        // Step 5: Wait one more frame to ensure chips have correct positions
        yield return null;

        // Step 6: Now it's safe to hide loading screen
        onComplete?.Invoke();
    }


    internal void SetOptionData()
    {
        AndarTxt.SetData(0, "andar", "", "main_bets");
        BaharTxt.SetData(1, "bahar", "", "main_bets");

        // Payout index for first_1_andar/bahar is set per-round based on middle card color in SetMainCard
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
    internal void SetBetLimit(string room)
    {
        int min = 0;
        int max = 0;

        switch (room)
        {
            case "casual":
                min = socketManager.initialData.levelBetLimit.casual.min_bet_limit;
                max = socketManager.initialData.levelBetLimit.casual.max_bet_limit;
                break;

            case "novice":
                min = socketManager.initialData.levelBetLimit.novice.min_bet_limit;
                max = socketManager.initialData.levelBetLimit.novice.max_bet_limit;
                break;

            case "expert":
                min = socketManager.initialData.levelBetLimit.expert.min_bet_limit;
                max = socketManager.initialData.levelBetLimit.expert.max_bet_limit;
                break;

            case "high_roller":
                min = socketManager.initialData.levelBetLimit.high_roller.min_bet_limit;
                max = socketManager.initialData.levelBetLimit.high_roller.max_bet_limit;
                break;
        }
        minBet_text.text = FormatHelper.FormatAmount(min);
        uiManager.MinBet.text = FormatHelper.FormatAmount(min);
        maxBet_text.text = FormatHelper.FormatAmount(max);

    }
    internal void SetCoinData()
    {
        // ResetCoinsToDefault();
        TotalPlayer_text.text = socketManager.roomData.playerCount.ToString();
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

        uiManager.coinSelector.Chiptext.text = FormatHelper.FormatChipAmount(data[0]);
        uiManager.coinSelector.chipIndex = 0;
        minBet_text.text = FormatHelper.FormatAmount(data[0]);
        uiManager.MinBet.text = FormatHelper.FormatAmount(data[0]);
        maxBet_text.text = FormatHelper.FormatAmount(data[uiManager.Coins.Count]);

        for (int i = 0; i < uiManager.Coins.Count; i++)
        {
            uiManager.Coins[i].Chiptext.text = FormatHelper.FormatChipAmount(data[i + 1]);
            uiManager.Coins[i].chipIndex = i + 1;
        }

        // Update chip sprites to match current level
        UpdateChipSprites();
    }
    public void ResetCoinsToDefault()
    {
        // Use level-specific sprites
        List<Sprite> currentChips = GetCurrentPlayerChipSprites();

        if (currentChips == null || currentChips.Count == 0)
        {
            return;
        }

        // Force select 0 index
        if (currentChips.Count > 0)
        {
            uiManager.coinSelector.chipImage.sprite = currentChips[0];
        }

        // Reset all coins visuals
        for (int i = 0; i < uiManager.Coins.Count; i++)
        {
            int spriteIndex = i + 1;
            if (spriteIndex < currentChips.Count)
            {
                uiManager.Coins[i].chipImage.sprite = currentChips[spriteIndex];
            }
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

        if (leaderboard == null)
        {
            return;
        }

        string mainPlayerName = uiManager.MainPlayers.playername.text;
        Sprite mainPlayerIcon = uiManager.MainPlayers.PlayerIcon.sprite;

        // ------------------- RICHEST -------------------
        if (leaderboard.richest == null || leaderboard.richest.Count == 0)
        {
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

        foreach (var item in AllOptions)
        {
            item.BG.SetActive(true);
            item.HighlightedBG.SetActive(false);
            item.DisableWinRatioText();
        }
        currentTotalBet = 0;

        // FIX 1: Keep bet LOCKED at round start. Bet unlock happens only when
        // the betting_timer broadcast fires (SetBetTimer). Do NOT unlock here.
        BetBlocker.gameObject.SetActive(true);
        uiManager.setCoins(false);
        uiManager.SetChipoption(false);
        CancelRepeatPanel();
        uiManager.SetNetBetPanel(false);
        MainFlushObj.SetActive(false);
        ResetAllBetUI();
        FirstThreeTxt.HighlightedBG.SetActive(false);
        FirstAndarTxt.HighlightedBG.SetActive(false);
        FirstBaharTxt.HighlightedBG.SetActive(false);
        RoundInfo_Text.gameObject.SetActive(false);
        pulseText.text = "";
        animHand.LeftCard.gameObject.SetActive(false);
        animHand.RightCard.gameObject.SetActive(false);

        AndarHighLight.SetActive(false);
        BaharHighLight.SetActive(false);
        AndarBtnHighLight.SetActive(false);
        BaharBtnHighLight.SetActive(false);
        firstTime = true;
        // Clear any stale card animations from the previous round
        cardAnimQueue.Clear();
        if (cardAnimCoroutine != null) { StopCoroutine(cardAnimCoroutine); cardAnimCoroutine = null; }
        PlayMiddleCardAnim();
        // yield return new WaitForSeconds(2f);

        // Black middle card: Andar deals first -> Andar payout[0], Bahar payout[1]
        // Red middle card: Bahar deals first -> Bahar payout[0], Andar payout[1]
        bool middleCardIsBlack = socketManager.gameLoopData.middleCard.color == "black";
        if (middleCardIsBlack)
        {
            AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[0].ToString(), "main_bets");
            BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[1].ToString(), "main_bets");
            FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[0].ToString(), "op_bets");
            FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[1].ToString(), "op_bets");
        }
        else
        {
            AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[1].ToString(), "main_bets");
            BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[0].ToString(), "main_bets");
            FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[1].ToString(), "op_bets");
            FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0].ToString(), "op_bets");
        }

    }

    internal void SetBetTimer()
    {
        ResetCardHistory();
        BonusObject.gameObject.SetActive(false);
        // Always apply correct win ratios from the current middle card color.
        // This handles mid-round joins where SetMainCard was never called.
        if (socketManager.TimeRemaining.middleCard != null)
        {
            bool isBlack = socketManager.TimeRemaining.middleCard.color == "black";
            if (isBlack)
            {
                AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[0].ToString(), "main_bets");
                BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[1].ToString(), "main_bets");
                FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[0].ToString(), "op_bets");
                FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[1].ToString(), "op_bets");
            }
            else
            {
                AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[1].ToString(), "main_bets");
                BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[0].ToString(), "main_bets");
                FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[1].ToString(), "op_bets");
                FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0].ToString(), "op_bets");
            }
        }
        // FIX 1: Unlock bets here - this fires when betting timer starts
        BetBlocker.gameObject.SetActive(false);
        uiManager.setCoins(true);
        // Repeat panel is NOT shown here - it only appears at time==25 for 5 sec
        RoundInfo_Text.gameObject.SetActive(true);
        animHand.MiddleCard.sprite = CardSet(socketManager.TimeRemaining.middleCard.suit, socketManager.TimeRemaining.middleCard.rank);
        animHand.MiddleCard.gameObject.SetActive(true);
        animHand.LeftCard.gameObject.SetActive(false);
        animHand.RightCard.gameObject.SetActive(false);
        CardCount_Text.text = "";
        AndarHighLight.SetActive(false);
        BaharHighLight.SetActive(false);
        AndarBtnHighLight.SetActive(false);
        BaharBtnHighLight.SetActive(false);
        foreach (var item in AllOptions)
        {
            item.BG.SetActive(true);
            item.HighlightedBG.SetActive(false);
        }

        int time = socketManager.TimeRemaining.timeRemaining;
        if (time > 5)
        {
            // RoundInfo_Text.text = "<size=30>Place bet Now</size>\n " + "<size=50><color=yellow>" + time + "</color></size>";
            RoundInfo_Text.text = "<size=30>Place Bet Now</size>\n ";
            pulseText.text = "<color=yellow>" + time.ToString() + "</color>";
            pulseText.gameObject.SetActive(true);
        }
        else
        {
            RoundInfo_Text.text = "<size=30>Place Bet Now</size>\n ";
            pulseText.text = "<color=yellow>" + time.ToString() + "</color>";
            pulseText.gameObject.SetActive(true);
            PopTMP(pulseText);

        }
        if (time == 5)
        {
            RoundInfoAnim(2);
            audioManager.PlayGirlAudio("timeisrunning");
        }
        else if (time == 1)
        {
            StartCoroutine(WaitForBetLocked(1f));
        }
        else if (time == 25)
        {
            EnableAllWinRatioTexts();
            audioManager.PlayGirlAudio("placeyourbet");
            // Show Repeat panel for 5 seconds if player has a previous bet
            if (isRepeatbetActive && !uiManager.isExpanded)
            {
                if (repeatPanelCoroutine != null) StopCoroutine(repeatPanelCoroutine);
                repeatPanelCoroutine = StartCoroutine(ShowRepeatPanelBriefly());
            }
        }
        // If joining mid-round during betting phase, ratios may not be enabled yet
        // EnableAllWinRatioTexts is safe to call multiple times (idempotent)
        if (time > 1 && time < 25)
        {
            EnableAllWinRatioTexts();
        }
        if (time % 5 == 4)
        {
            Handanimator.Play("NoHand");

        }
    }

    IEnumerator WaitForBetLocked(float delay)
    {
        yield return new WaitForSeconds(delay);
        pulseText.gameObject.SetActive(false);
        RoundInfoAnim(1);
        BetBlocker.gameObject.SetActive(true);
        audioManager.PlayGirlAudio("nomorebets");
        //RoundInfo_Text.gameObject.SetActive(false);
        uiManager.SetChipoption(false);
        uiManager.setCoins(false);
        CancelRepeatPanel();

        RoundInfo_Text.text = "<size=30>BET LOCKED!</size>";
        BetBlocker.gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the Repeat panel at betting start (time==25) for 5 seconds then hides it.
    /// Cancelled automatically if the player places a bet or bets lock.
    /// </summary>
    IEnumerator ShowRepeatPanelBriefly()
    {
        uiManager.Repeatpanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        uiManager.Repeatpanel.SetActive(false);
        repeatPanelCoroutine = null;
    }

    /// <summary>
    /// Call this whenever the repeat panel must be force-hidden (bet placed, locked, etc.)
    /// </summary>
    void CancelRepeatPanel()
    {
        if (repeatPanelCoroutine != null)
        {
            StopCoroutine(repeatPanelCoroutine);
            repeatPanelCoroutine = null;
        }
        uiManager.Repeatpanel.SetActive(false);
    }
    bool firstTime = true;
    internal void SetNewRoundTimer(int i)
    {
        // int value = i >= 1000 ? i / 1000 : 0;
        int value = i;

        // Only disable win ratio when timer actually ends (transitions from >0 to 0)
        // Don't disable if player joins mid-round with timer already at 0
        if (value <= 0)
        {
            if (previousNextRoundTimerValue > 0)
            {
                // Timer actually ended (was running and reached 0)
                DisableAllWinRatioTexts();
            }
            // Reset tracking for next round
            previousNextRoundTimerValue = -1;
            return;
        }

        // Track the current value for next iteration
        previousNextRoundTimerValue = value;

        if (firstTime) RoundInfoAnim(3);
        firstTime = false;
        pulseText.gameObject.SetActive(true);
        RoundInfo_Text.text = "<size=30>Next Round</size>\n ";

        pulseText.text = $"<color=#00AB15>{value}</color>";

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
        CancelRepeatPanel();
        uiManager.SetNetBetPanel(true, currentTotalBet.ToString());
        RoundInfo_Text.text = "<size=30>BET LOCKED!</size>";

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
                CardSetHIS(cardDelt.andarCards[i].suit, cardDelt.andarCards[i].rank)
            );
        }

        // BAHAR history
        for (int i = 0; i < baharCount; i++)
        {
            baharSpriteList.Add(
                CardSetHIS(cardDelt.baharCards[i].suit, cardDelt.baharCards[i].rank)
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
        CancelRepeatPanel();
        RoundInfo_Text.text = "<size=30>BET LOCKED!</size>";
        if (StartGameCorutine != null)
        {
            StopCoroutine(StartGameCorutine);
            StartGameCorutine = null;
        }
        int delivered = socketManager.CardDelt.cardsDealt;

        // ✅ ADD WIN SOUNDS HERE:
        if (socketManager.gameLoopData.matchSide == "andar")
        {
            uiManager.UpdateStats(socketManager.gameLoopData.middleCard.rank, true, delivered.ToString());

            // ✅ NEW: Play Andar win sound
            if (audioManager != null)
            {
                audioManager.PlayWinSound("andar");
            }

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

            // ✅ NEW: Play Bahar win sound
            if (audioManager != null)
            {
                audioManager.PlayWinSound("bahar");
            }

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

        yield return new WaitForSeconds(3f);

        // PlayResetAnimation is triggered in RestRoundText on game:round_start
        if (currentWin >= 1)
        {
            int winInt = Mathf.FloorToInt((float)currentWin);
            playtheCoin("+" + winInt);
        }

        currentWin = 0;
        ResetAllBetUI();
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
        // StartCoroutine(NextRoundtext());
    }
    internal void RestRoundText()
    {
        RoundInfoAnim(0);
        RoundInfo_Text.text = "  ";
        pulseText.text = " ";
        // Stop new round animation when the next round actually begins
        NewRoundAnim.StopAnimation();
        // Slide in the Andar/Bahar reset panel — fires exactly on round_start event
        PlayResetAnimation();
    }
    IEnumerator ManagePayout()
    {

        // MoveAllChipstohome();
        // yield return new WaitForSeconds(1f);

        // DistributeAllPayout();
        // SetOtherplayerData(socketManager.CashoutData.leaderboards);
        // resultsOptions.Clear();
        // yield return null;
        resultsOptions.Add(ResultOption);
        MoveAllChipstohomeNew();
        ResetOptionPrefabs();
        yield return new WaitForSeconds(3f);

        // 2️⃣ Spawn payout chips on winning options
        SpawnPayoutOnWinningOptions(socketManager.CashoutData.payouts);
        // FIX 3: Only reset non-winning bet areas here.
        // Winning options must keep TotalBetObj visible so the win amount
        // shows while chips are animating in.
        ResetNonWinningBetUI();

        yield return new WaitForSeconds(1.5f);

        // 3️⃣ Move winning chips to players
        MoveWinningChipsToPlayers(socketManager.CashoutData.payouts);

        // 4️⃣ Update leaderboard
        SetOtherplayerData(socketManager.CashoutData.leaderboards);
        yield return new WaitForSeconds(1.5f);
        ResetAllChips();
        // Extra wait so TotalBetObj stays visible after chips land before hiding
        yield return new WaitForSeconds(0.75f);
        // FIX 3: Now that chips have fully animated to players, hide winning areas
        foreach (var item in resultsOptions)
        {
            if (item != null) item.DontShowText();
        }
        yield return new WaitForSeconds(1f);
        // FIX 3: Full reset only after everything is done
        ResetAllBetUI();
        resultsOptions.Clear();


    }
    void ResetOptionPrefabs()
    {
        var payouts = socketManager.CashoutData.payouts;

        foreach (var item in AllOptions)
        {
            if (!resultsOptions.Contains(item))
            {
                // Non-winning: hide all bet text
                item.DontShowText();
            }
            else
            {
                // FIX 3: Show the existing totalBet (chips already on this option)
                // so TotalBetObj displays the bet amount while win chips fly in.
                // We keep whatever totalBet was accumulated during the round.
                item.TotalBetObj.SetActive(item.totalBet > 0);
                item.TotalBetText.text = FormatHelper.FormatAmount(item.totalBet);
                item.MyBetObj.SetActive(false);
            }
        }

        // Also handle BiggerOptions (Andar/Bahar live here in some setups)
        foreach (var item in BiggerOptions)
        {
            if (!resultsOptions.Contains(item))
            {
                item.DontShowText();
            }
            else
            {
                item.TotalBetObj.SetActive(item.totalBet > 0);
                item.TotalBetText.text = FormatHelper.FormatAmount(item.totalBet);
                item.MyBetObj.SetActive(false);
            }
        }
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

    void ResetAllChips()
    {
        // Return main player chips to pool
        foreach (var chip in PlayerChips)
        {
            if (chip.chip != null)
            {
                // If you have pooling system
                chip.chip.SetActive(false);
            }
        }

        // Return other player chips to pool
        foreach (var chip in OtherPlayerChips)
        {
            if (chip.chip != null)
            {
                chip.chip.SetActive(false);
            }
        }

        PlayerChips.Clear();
        OtherPlayerChips.Clear();
    }


    void MoveAllChipstohomeNew()
    {
        foreach (var item in PlayerChips)
        {
            if (!resultsOptions.Contains(item.betoptions))
            {
                MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true, 1f);
            }
        }
        foreach (var item in OtherPlayerChips)
        {
            if (!resultsOptions.Contains(item.betoptions))
            {
                MoveChip(item.chip.transform, item.chip.transform, RoundInfo_Text.transform, true, 1f);
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
                socketManager.playerdata.balance = payout.balance;
            }
            if (target == null)
                target = TotalPlayer_text.transform;

            SpawnPayoutChips(payout.win, target);
        }
    }

    void SpawnPayoutOnWinningOptions(List<Payout> payouts)
    {
        List<OptionPrefab> validOptions = GetValidWinningOptions();

        if (validOptions.Count == 0)
        {
            validOptions.Add(resultsOptions[0]);
        }

        int totalCount = validOptions.Count;
        int resultIndex = 0;

        // FIX 3: Tally the total win amount per winning option so we can
        // display it in TotalBetText as win chips land there.
        Dictionary<OptionPrefab, double> winPerOption = new Dictionary<OptionPrefab, double>();

        foreach (var payout in payouts)
        {
            if (payout.win <= 0)
                continue;

            List<int> roomChips = FindRoom();
            List<int> chipPieces =
                BreakAmountIntoMinChips((int)payout.win, roomChips);

            foreach (int piece in chipPieces)
            {
                OptionPrefab winningOption = validOptions[resultIndex];

                int index = findChipindex(piece, roomChips);

                ChipData data = new ChipData();
                data.betId = payout.username;
                data.amount = piece;
                data.betoptions = winningOption;

                data.chip = SpawnChip(
                    findChipSprite(piece, roomChips),
                    FormatHelper.FormatChipAmount(piece),
                    index,
                    RoundInfo_Text.transform,
                    winningOption,
                    false,
                    1f
                );

                if (uiManager.MainPlayers.playername.text == payout.username)
                {
                    PlayerChips.Add(data);
                    double oldBalance = double.Parse(uiManager.MainPlayers.playerBalence.text);
                    double newBalance = payout.balance;

                    currentWin = newBalance - oldBalance;
                }
                else
                    OtherPlayerChips.Add(data);

                // FIX 3: Accumulate win amount per winning option
                if (!winPerOption.ContainsKey(winningOption))
                    winPerOption[winningOption] = 0;
                winPerOption[winningOption] += piece;

                // rotate safely
                resultIndex++;
                if (resultIndex >= totalCount)
                    resultIndex = 0;
            }
        }

        // FIX 3: Update TotalBetText on each winning option to show
        // existing bet amount + incoming win amount, keeping TotalBetObj visible
        foreach (var kvp in winPerOption)
        {
            OptionPrefab opt = kvp.Key;
            double winAmount = kvp.Value;
            double displayTotal = opt.totalBet + winAmount;
            opt.TotalBetObj.SetActive(true);
            opt.TotalBetText.text = FormatHelper.FormatAmount(displayTotal);
        }
    }
    List<OptionPrefab> GetValidWinningOptions()
    {
        if (resultsOptions == null)
            return new List<OptionPrefab>();

        List<OptionPrefab> validOptions = new List<OptionPrefab>();

        foreach (var option in resultsOptions)
        {
            bool foundInPlayer = PlayerChips.Any(x =>
                x.betoptions != null && x.betoptions.name == option.name);

            bool foundInOther = OtherPlayerChips.Any(x =>
                x.betoptions != null && x.betoptions.name == option.name);

            if (foundInPlayer || foundInOther)
            {
                validOptions.Add(option);
            }
        }

        return validOptions;
    }

    // List<OptionPrefab> GetValidWinningOptions()
    // {

    //     if (resultsOptions == null)
    //     {
    //         return new List<OptionPrefab>();
    //     }


    //     List<OptionPrefab> validOptions = new List<OptionPrefab>();

    //     for (int i = 0; i < resultsOptions.Count; i++)
    //     {
    //         OptionPrefab option = resultsOptions[i];


    //         bool foundInPlayer = false;
    //         bool foundInOther = false;

    //         // 🔹 Check PlayerChips
    //         for (int j = 0; j < PlayerChips.Count; j++)
    //         {

    //             if (PlayerChips[j].betoptions == option)
    //             {
    //                 foundInPlayer = true;
    //                 break;
    //             }
    //         }

    //         // 🔹 Check OtherPlayerChips
    //         for (int k = 0; k < OtherPlayerChips.Count; k++)
    //         {

    //             if (OtherPlayerChips[k].betoptions == option)
    //             {
    //                 foundInOther = true;
    //                 break;
    //             }
    //         }

    //         if (foundInPlayer || foundInOther)
    //         {
    //             validOptions.Add(option);
    //         }
    //         else
    //         {
    //         }
    //     }


    //     return validOptions;
    // }
    void MoveWinningChipsToPlayers(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            Transform target = FindPlayerTransform(payout.username);
            if (target == null)
                target = TotalPlayer_text.transform;

            // 🔹 Move Main Player Chips
            if (uiManager.MainPlayers.playername.text == payout.username)
            {
                var chipsToMove = PlayerChips
                    .Where(x => resultsOptions.Contains(x.betoptions))
                    .ToList();

                foreach (var chipData in chipsToMove)
                {
                    MoveChip(chipData.chip.GetComponent<Chip>(), target, true);
                }

                uiManager.MainPlayers.playerBalence.text = payout.balance.ToString();
                socketManager.playerdata.balance = payout.balance;
            }
            else
            {
                // 🔹 Move Other Player Chips
                var chipsToMove = OtherPlayerChips
                    .Where(x => x.betId == payout.username &&
                                resultsOptions.Contains(x.betoptions))
                    .ToList();

                foreach (var chipData in chipsToMove)
                {
                    MoveChip(chipData.chip.GetComponent<Chip>(), target, true);
                }
            }

            if (uiManager.MainPlayers.playername.text == payout.username)
            {
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


        HighlightOption(optionIndex);
        ResultOption = AllOptions[optionIndex];
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
        // animHand.MiddleCard.gameObject.SetActive(false);
        Handanimator.Play("MiddleCard");
        uiManager.CalculateStringProbability(socketManager.gameLoopData.middleCard.rank);
    }
    // ─── Card animation queue helpers ─────────────────────────────────────────
    // Each dealt card is pushed onto the queue; the drain coroutine plays them
    // back-to-back with zero extra delay between sequential cards.

    void EnqueueCardAnim(bool isAndar, int cardCount, Sprite cardSprite)
    {
        cardAnimQueue.Enqueue(() => ExecuteCardAnim(isAndar, cardCount, cardSprite));
        if (cardAnimCoroutine == null)
            cardAnimCoroutine = StartCoroutine(DrainCardAnimQueue());
    }

    IEnumerator DrainCardAnimQueue()
    {
        while (cardAnimQueue.Count > 0)
        {
            System.Action next = cardAnimQueue.Dequeue();
            next?.Invoke();

            // Wait exactly as long as the Animator clip takes (0.3 s deal + small buffer)
            // so the next card starts the instant the current flip finishes.
            yield return new WaitForSeconds(0.35f);
        }
        cardAnimCoroutine = null;
    }

    void ExecuteCardAnim(bool isAndar, int cardCount, Sprite cardSprite)
    {
        if (isAndar)
        {
            if (cardCount > 2)
            {
                AndarHighLight.SetActive(true);
                BaharHighLight.SetActive(false);
                AndarBtnHighLight.SetActive(true);
                BaharBtnHighLight.SetActive(false);
            }
            else StartCoroutine(delayedactive(true, false));
            animHand.LeftSprite = cardSprite;
            Handanimator.Play("LeftCard");
        }
        else
        {
            if (cardCount > 2)
            {
                AndarHighLight.SetActive(false);
                BaharHighLight.SetActive(true);
                AndarBtnHighLight.SetActive(false);
                BaharBtnHighLight.SetActive(true);
            }
            else StartCoroutine(delayedactive(false, true));
            animHand.RightSprite = cardSprite;
            Handanimator.Play("RightCard");
        }
    }

    void PlayAndarCardAnim(int cardCount)
    {
        Sprite s = CardSet(socketManager.CardDelt.card.suit, socketManager.CardDelt.card.rank);
        EnqueueCardAnim(true, cardCount, s);
    }
    void PlayBagarCardAnim(int cardCount)
    {
        Sprite s = CardSet(socketManager.CardDelt.card.suit, socketManager.CardDelt.card.rank);
        EnqueueCardAnim(false, cardCount, s);
    }
    IEnumerator delayedactive(bool andar, bool bahar)
    {
        yield return new WaitForSeconds(1f);
        AndarHighLight.SetActive(andar);
        BaharHighLight.SetActive(bahar);
        AndarBtnHighLight.SetActive(andar);
        BaharBtnHighLight.SetActive(bahar);
    }
    private void MoveChip(Transform chip, Transform startPos, Transform endPos, bool disableOnEnd, float duration = 1f)
    {
        if (chip == null || startPos == null || endPos == null)
        {
            return;
        }

        chip.position = startPos.position;
        chip.DOMove(endPos.position, duration)
            .SetEase(Ease.InOutExpo)
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

                return;
            }
        }
        socketManager.BetPlaced(index, optionprefab.VaridontWant, socketManager.initialData.betOptions[optionprefab.Optionindex]);
        uiManager.RetractCoins();
        isRepeatbetActive = true;
        CancelRepeatPanel(); // hide repeat panel once player is actively placing bets

    }
    internal void ManageBrodcastBetsPlayer()
    {
        uiManager.SetChipoption(true);
        CancelRepeatPanel();
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
            string val = FormatHelper.FormatChipAmount(chipAmount);


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
    // internal void ManageBrodcastBetsOtherPlayers(Root chipdata)
    // {
    //     if (chipdata.amount < 0)
    //     {
    //         ClearOtherPlayerbets(chipdata);

    //         return;
    //     }
    //     // Do not show own chip here
    //     if (chipdata.username == uiManager.MainPlayers.playername.text)
    //         return;

    //     List<int> roomChips = FindRoom();
    //     int totalAmount = chipdata.amount;

    //     // Break large amount into individual chips
    //     List<int> chipPieces = BreakAmountIntoChips(totalAmount, roomChips);

    //     foreach (int piece in chipPieces)
    //     {
    //         int index = findChipindex(piece, roomChips);

    //         string val = piece.ToString();

    //         ChipData data = new ChipData();
    //         data.betId = chipdata.betId;
    //         data.amount = piece;
    //         data.betoptions = FindOption(chipdata.payload.betOption);

    //         data.chip = SpawnChip(
    //             findOtherPlayerChipSprite(piece, roomChips),
    //             val,
    //             index,
    //             TotalPlayer_text.transform,
    //             FindOption(chipdata.betOption)
    //         );
    //         data.chip.transform.SetParent(OtherPlayerChipPoolParent);
    //         OtherPlayerChips.Add(data);
    //         UpdateTotalBetOnOption(chipdata.betOption, piece);
    //     }
    // }
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

        // 🔹 Find player position (leaderboard / winner / richest)
        Transform spawnFrom = FindPlayerTransform(chipdata.username);
        if (spawnFrom == null)
            spawnFrom = TotalPlayer_text.transform;

        // Break large amount into individual chips
        List<int> chipPieces = BreakAmountIntoChips(totalAmount, roomChips);

        foreach (int piece in chipPieces)
        {
            int index = findChipindex(piece, roomChips);
            string val = FormatHelper.FormatChipAmount(piece);

            ChipData data = new ChipData();
            data.betId = chipdata.betId;
            data.amount = piece;
            data.betoptions = FindOption(chipdata.payload.betOption);

            data.chip = SpawnChip(
                findOtherPlayerChipSprite(piece, roomChips),
                val,
                index,
                spawnFrom, // 🔹 spawn from leaderboard player
                FindOption(chipdata.betOption)
            );

            data.chip.transform.SetParent(OtherPlayerChipPoolParent);

            OtherPlayerChips.Add(data);

            UpdateTotalBetOnOption(chipdata.betOption, piece);
        }

        audioManager.PlayWLAudio("double");
    }
    /// <summary>
    /// Called from OnRoomEnter when joining mid-round.
    /// Sets correct win ratios based on middle card color, enables win ratio texts,
    /// and defers chip spawning by one frame so UI layout is ready.
    /// </summary>
    internal void ApplyMidRoundState(RoundState roundState, List<Bet> bets)
    {
        // --- Fix win ratios based on middle card color ---
        if (roundState.middleCard != null)
        {
            bool isBlack = roundState.middleCard.color == "black";
            if (isBlack)
            {
                AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[0].ToString(), "main_bets");
                BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[1].ToString(), "main_bets");
                FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[0].ToString(), "op_bets");
                FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[1].ToString(), "op_bets");
            }
            else
            {
                AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[1].ToString(), "main_bets");
                BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[0].ToString(), "main_bets");
                FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[1].ToString(), "op_bets");
                FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0].ToString(), "op_bets");
            }

            // Always show middle card regardless of phase
            animHand.MiddleCard.sprite = CardSet(roundState.middleCard.suit, roundState.middleCard.rank);
            animHand.MiddleCard.gameObject.SetActive(true);
        }

        // ── Phase-specific UI ──────────────────────────────────────────────────
        if (roundState.phase == "betting")
        {
            // Bet open — show timer text, correct sprite, unlock UI
            EnableAllWinRatioTexts();
            BetBlocker.gameObject.SetActive(false);
            uiManager.setCoins(true);
            animHand.LeftCard.gameObject.SetActive(false);
            animHand.RightCard.gameObject.SetActive(false);

            int time = roundState.timeRemaining;

            RoundInfo_Text.gameObject.SetActive(true);
            RoundInfo_Text.text = "<size=30>Place Bet Now</size>\n ";
            pulseText.text = "<color=yellow>" + time + "</color>";
            pulseText.gameObject.SetActive(true);

            if (time <= 5)
            {
                RoundInfoAnim(2);
                PopTMP(pulseText);
            }
            else
            {
                RoundInfoAnim(0);   // "Place Bet" sprite
            }

            // Show repeat panel briefly if the player already has a saved bet
            if (isRepeatbetActive && !uiManager.isExpanded)
            {
                if (repeatPanelCoroutine != null) StopCoroutine(repeatPanelCoroutine);
                repeatPanelCoroutine = StartCoroutine(ShowRepeatPanelBriefly());
            }
        }
        else if (roundState.phase == "dealing")
        {
            // Cards are being dealt — bets locked, hide coins
            BetBlocker.gameObject.SetActive(true);
            uiManager.setCoins(false);
            uiManager.SetChipoption(false);
            CancelRepeatPanel();

            RoundInfo_Text.gameObject.SetActive(true);
            RoundInfo_Text.text = "<size=30>BET LOCKED!</size>";
            pulseText.gameObject.SetActive(false);
            RoundInfoAnim(1);   // "Bet Locked" sprite
        }
        else
        {
            // bonus / cashout / any other post-deal phase — fully locked
            BetBlocker.gameObject.SetActive(true);
            uiManager.setCoins(false);
            uiManager.SetChipoption(false);
            CancelRepeatPanel();

            RoundInfo_Text.gameObject.SetActive(true);
            RoundInfo_Text.text = "<size=30>BET LOCKED!</size>";
            pulseText.gameObject.SetActive(false);
            RoundInfoAnim(1);
        }

        // --- Defer chip spawning by one frame so RectTransform layout is ready ---
        if (bets != null && bets.Count > 0 && (roundState.phase == "betting" || roundState.phase == "dealing"))
        {
            StartCoroutine(SpawnMidRoundChipsNextFrame(bets));
        }
    }

    IEnumerator SpawnMidRoundChipsNextFrame(List<Bet> bets)
    {
        // Wait two frames: one for GamePage activation, one for Canvas layout rebuild
        yield return null;
        yield return null;

        string mainPlayerName = uiManager.MainPlayers.playername.text;
        List<int> roomChips = FindRoom();

        foreach (var bet in bets)
        {
            if (string.IsNullOrEmpty(bet.username) || bet.username == mainPlayerName)
                continue;

            Transform spawnFrom = FindPlayerTransform(bet.username);
            if (spawnFrom == null)
                spawnFrom = TotalPlayer_text.transform;

            int totalAmount = bet.amount;
            List<int> chipPieces = BreakAmountIntoChips(totalAmount, roomChips);

            foreach (int piece in chipPieces)
            {
                int index = findChipindex(piece, roomChips);
                string val = FormatHelper.FormatChipAmount(piece);
                OptionPrefab targetOption = FindOption(bet.betOption);

                ChipData data = new ChipData();
                data.betId = bet.betId;
                data.amount = piece;
                data.betoptions = targetOption;

                data.chip = SpawnChip(
                    findOtherPlayerChipSprite(piece, roomChips),
                    val,
                    index,
                    spawnFrom,
                    targetOption
                );

                data.chip.transform.SetParent(OtherPlayerChipPoolParent);
                OtherPlayerChips.Add(data);
                UpdateTotalBetOnOption(bet.betOption, piece);
            }
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
                    // break;
                }
            }
        }
        // PlayerChips.Clear();
        //  ResetAllBetUI();

    }

    GameObject SpawnChip(Sprite sprite, string amount, int chipindex, Transform startPoint, OptionPrefab op, bool isPlayerbet = false, float moveTime = 1f)
    {
        Chip chip = GetChip();
        chip.SetData(sprite, amount, chipindex);

        RectTransform chipRT = chip.GetComponent<RectTransform>();
        chipRT.SetParent(poolParent);
        chipRT.localScale = Vector3.one;

        Vector2 size = op.chiparea.rect.size;
        Vector2 randomPos = new Vector2(
            UnityEngine.Random.Range(-size.x * 0.40f, size.x * 0.40f),
            UnityEngine.Random.Range(-size.y * 0.40f, size.y * 0.40f)
        );

        Vector3 worldRandomPos = op.chiparea.TransformPoint(randomPos);

        if (!isPlayerbet && startPoint != null)
        {
            chipRT.position = startPoint.position;
            chipRT.DOMove(worldRandomPos, moveTime);
        }
        else
        {
            chipRT.position = worldRandomPos;
        }

        return chip.gameObject;
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

        // 1️⃣ Slide IN slower (0.8f → 0.9f for smoother animation)
        seq.Append(rt.DOAnchorPosX(targetX, 1.2f).SetEase(Ease.OutBounce))

           .AppendInterval(0.01f)
           .AppendCallback(() =>
           {
               // Stop any running animations before starting new ones
               NewRoundAnim.StopAnimation();
               SparkAnim.StopAnimation();
           })

           .AppendInterval(1.5f)
           .AppendCallback(() =>
           {
               // Clear all text and UI elements BEFORE starting animations
               CardCount_Text.text = "";
               RoundInfo_Text.text = "";
               AndarTxt.DontShowText();
               BaharTxt.DontShowText();
               AndarTxt.winAnimation.StopAnimation();
               BaharTxt.winAnimation.StopAnimation();

               // Start NewRound animation but NOT spark yet
               // Spark will start AFTER slide-out completes
               NewRoundAnim.StartAnimation();
           })

           // 2️⃣ Slide OUT slower (0.6f → 0.8f for better visual flow)
           .Append(rt.DOAnchorPosX(startX, 0.8f).SetEase(Ease.InOutSine))

           // 3️⃣ CRITICAL FIX: Start spark animation ONLY AFTER slide-out completes
           // This prevents the spark from appearing too early during the slide-out
           .AppendCallback(() =>
           {
               SparkAnim.StartAnimation();
           });

        // Reset all option backgrounds
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
        chip.transform.DOMove(target.position, 1.2f)
            .SetEase(Ease.InOutExpo)
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

        // --- Check Main Player ---
        if (uiManager.MainPlayers != null)
        {
            if (uiManager.MainPlayers.playername.text == playerId)
            {
                return uiManager.MainPlayers.transform;
            }
        }
        else
        {
        }

        // --- Check Richest Players ---
        foreach (var p in uiManager.RichestPlayers)
        {
            if (p == null)
            {
                continue;
            }

            if (p.PlayerId == playerId)
            {
                return p.transform;
            }
        }

        // --- Check Winner Players ---
        foreach (var p in uiManager.WinnerPlayers)
        {
            if (p == null)
            {
                continue;
            }

            if (p.PlayerId == playerId)
            {
                return p.transform;
            }
        }

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

                string val = FormatHelper.FormatChipAmount(piece);
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
                int amount = bet.delta;

                List<int> chipPieces = BreakAmountIntoChips(amount, roomChips);

                foreach (int piece in chipPieces)
                {
                    ChipData data = new ChipData();
                    data.betId = bet.betId;
                    data.amount = piece;
                    data.betoptions = FindOption(bet.betOption);

                    string val = FormatHelper.FormatChipAmount(piece);
                    int index = findChipindex(piece, roomChips);

                    if (index <= 5)
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
            AllOptions[i].TotalBetText.text = FormatHelper.FormatAmount(newAmount);
            AllOptions[i].totalBet = newAmount;
            AllOptions[i].MyBetObj.SetActive(false);
            AllOptions[i].MyBetText.text = FormatHelper.FormatAmount(0);
            AllOptions[i].playerBet = 0;


        }
        for (int i = 0; i < BiggerOptions.Count; i++)
        {
            int newAmount = BiggerOptions[i].totalBet - BiggerOptions[i].playerBet;
            if (newAmount <= 0) BiggerOptions[i].TotalBetObj.SetActive(false);
            BiggerOptions[i].TotalBetText.text = FormatHelper.FormatAmount(newAmount);
            BiggerOptions[i].totalBet = newAmount;
            BiggerOptions[i].MyBetObj.SetActive(false);
            BiggerOptions[i].MyBetText.text = FormatHelper.FormatAmount(0);
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
                break;
        }

        return tempSprite;
    }

    internal Sprite CardSetHIS(string suit, string value)
    {

        Sprite tempSprite = null;
        switch (suit.ToUpper())
        {
            case "HEARTS":
                tempSprite = GetCardSprite(HeartSpriteListDisplay, value);
                break;
            case "DIAMONDS":
                tempSprite = GetCardSprite(DiamondSpriteListDisplay, value);
                break;
            case "CLUBS":
                tempSprite = GetCardSprite(ClubSpriteListDisplay, value);
                break;
            case "SPADES":
                tempSprite = GetCardSprite(SpadeSpriteListDisplay, value);
                break;
            default:
                break;
        }

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
                return spriteList[myval - 1];
        }
    }

    #endregion

    #region Level-Specific Chip Sprite Helpers

    /// <summary>
    /// Get the player chip sprite list for the current room/level
    /// </summary>
    private List<Sprite> GetCurrentPlayerChipSprites(bool highlighted = true)
    {
        if (highlighted)
        {
            switch (currentRoom)
            {
                case "casual":
                    return PlayerChipSprite_Casual;
                case "novice":
                    return PlayerChipSprite_Novice;
                case "expert":
                    return PlayerChipSprite_Expert;
                case "high_roller":
                    return PlayerChipSprite_HighRoller;
                default:
                    return PlayerChipSprite_Casual;
            }
        }
        else
        {
            switch (currentRoom)
            {
                case "casual":
                    return HighlightedChipSprite_Casual;
                case "novice":
                    return HighlightedChipSprite_Novice;
                case "expert":
                    return HighlightedChipSprite_Expert;
                case "high_roller":
                    return HighlightedChipSprite_HighRoller;
                default:
                    return HighlightedChipSprite_Casual;
            }
        }
    }

    /// <summary>
    /// Get the other player chip sprite list for the current room/level
    /// </summary>
    private List<Sprite> GetCurrentOtherChipSprites()
    {
        switch (currentRoom)
        {
            case "casual":
                return OtherChipSprite_Casual;
            case "novice":
                return OtherChipSprite_Novice;
            case "expert":
                return OtherChipSprite_Expert;
            case "high_roller":
                return OtherChipSprite_HighRoller;
            default:
                return OtherChipSprite_Casual;
        }
    }

    /// <summary>
    /// Update all chip selector sprites to match the current level
    /// </summary>
    private void UpdateChipSprites()
    {
        List<Sprite> currentChips = GetCurrentPlayerChipSprites(false);

        if (currentChips == null || currentChips.Count == 0)
        {
            return;
        }

        // Update main coin selector sprite (index 0)
        if (currentChips.Count > 0)
        {
            uiManager.coinSelector.chipImage.sprite = currentChips[0];
        }

        // Update all coin option sprites (indices 1-5)
        for (int i = 0; i < uiManager.Coins.Count; i++)
        {
            int spriteIndex = i + 1;
            if (spriteIndex < currentChips.Count)
            {
                uiManager.Coins[i].chipImage.sprite = currentChips[spriteIndex];
            }
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
    // List<int> BreakAmountIntoChips(int amount, List<int> chipOptions)
    // {
    //     // ✅ Make a COPY so original list is not modified
    //     List<int> sortedChips = new List<int>(chipOptions);

    //     // Sort descending
    //     sortedChips.Sort((a, b) => b.CompareTo(a));

    //     List<int> results = new List<int>();

    //     foreach (int chip in sortedChips)
    //     {
    //         while (amount >= chip)
    //         {
    //             amount -= chip;
    //             results.Add(chip);
    //         }
    //     }

    //     return results;
    // }
    List<int> BreakAmountIntoChips(int amount, List<int> chipOptions)
    {
        List<int> results = new List<int>();

        while (amount > 0)
        {
            int bestValue = int.MinValue;

            // Find largest chip <= remaining amount
            for (int i = 0; i < chipOptions.Count; i++)
            {
                if (chipOptions[i] <= amount && chipOptions[i] > bestValue)
                {
                    bestValue = chipOptions[i];
                }
            }

            // If no valid chip found → just add remaining amount and stop
            if (bestValue == int.MinValue)
            {
                results.Add(amount);
                break;
            }

            results.Add(bestValue);
            amount -= bestValue;
        }

        return results;
    }
    List<int> BreakAmountIntoMinChips(int amount, List<int> chipOptions)
    {
        List<int> results = new List<int>();

        if (amount <= 0)
            return results;

        int minChip = chipOptions[0];   // ✅ always smallest chip

        int count = amount / minChip;

        for (int i = 0; i < count; i++)
        {
            results.Add(minChip);
        }

        return results;
    }

    internal void UpdatePlayerbalance(string balance)
    {
        uiManager.MainPlayers.playerBalence.text = balance;
    }
    Sprite findChipSprite(int amount, List<int> betOptions)
    {
        // Use level-specific sprites
        List<Sprite> sprites = GetCurrentPlayerChipSprites();

        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] == amount)
            {
                // Safety check
                if (i < sprites.Count)
                    return sprites[i];

            }
        }

        // Return first sprite as fallback
        return sprites.Count > 0 ? sprites[0] : null;

    }
    Sprite findOtherPlayerChipSprite(int amount, List<int> betOptions)
    {
        // Use level-specific sprites
        List<Sprite> sprites = GetCurrentOtherChipSprites();

        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] == amount)
            {
                // Safety check
                if (i < sprites.Count)
                    return sprites[i];

            }
        }

        // Return first sprite as fallback
        return sprites.Count > 0 ? sprites[0] : null;

    }
    int findChipindex(int amount, List<int> betOptions)
    {
        int bestIndex = 0;
        int bestValue = int.MinValue;

        for (int i = 0; i < betOptions.Count; i++)
        {
            if (betOptions[i] <= amount && betOptions[i] > bestValue)
            {
                bestValue = betOptions[i];
                bestIndex = i;
            }
        }

        return bestIndex; // returns -1 if no valid chip found
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

        coinTween?.Kill();

        coinAddText.rectTransform.localPosition = startPoscoin;
        coinAddText.transform.localScale = Vector3.one;
        coinAddText.color = startColor;
        coinAddText.gameObject.SetActive(true);

        float moveDuration = 1.35f;
        float scaleDuration = 0.75f;

        coinTween = DOTween.Sequence()

            .Append(
                coinAddText.rectTransform
                    .DOLocalMoveY(startPoscoin.y + moveY, moveDuration)
                    .SetEase(Ease.Linear)
            )

            .Append(
                DOTween.Sequence()
                    .Join(
                        coinAddText.transform
                            .DOScale(1.15f, scaleDuration)
                    ).Join(
                        coinImage.transform
                            .DOScale(1.15f, scaleDuration)
                    )
                    .Join(
                        coinAddText
                            .DOFade(0f, scaleDuration)
                    )
                    .Join(
                        coinImage
                            .DOFade(0f, scaleDuration)
                    )

            )

            .OnComplete(() =>
            {
                coinAddText.gameObject.SetActive(false);
                coinAddText.rectTransform.localPosition = startPoscoin;
                coinAddText.transform.localScale = Vector3.one;
                coinAddText.color = startColor;
            });
    }


    private IEnumerator PopupRoutine()
    {
        BlockerObj.transform.position = popStart.position;
        BlockerObj.transform.localScale = Vector3.one;

        yield return Move(BlockerObj.transform, popCenter.position, moveDuration);

        BlockerObj.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

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

        int newAmount = option.playerBet + amount;
        if (newAmount <= 0) option.MyBetObj.SetActive(false);
        option.MyBetText.text = FormatHelper.FormatAmount(newAmount);
        option.playerBet = newAmount;
    }
    internal void UpdateTotalBetOnOption(string opt, int amount, OptionPrefab optn = null)
    {
        OptionPrefab option = FindOption(opt);
        if (optn != null) option = optn;
        if (option == null) return;

        option.TotalBetObj.SetActive(true);

        int newAmount = option.totalBet + amount;
        if (newAmount <= 0) option.TotalBetObj.SetActive(false);
        option.TotalBetText.text = FormatHelper.FormatAmount(newAmount);
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

    /// <summary>
    /// FIX 3: Resets bet UI only for non-winning options.
    /// Winning options (in resultsOptions) are skipped so TotalBetObj stays
    /// visible with the win amount while payout chips are animating in.
    /// </summary>
    void ResetNonWinningBetUI()
    {
        // Build a flat list of every option prefab
        var allOpts = new List<OptionPrefab>();
        allOpts.AddRange(AllOptions);
        allOpts.AddRange(BiggerOptions);
        allOpts.Add(AndarTxt);
        allOpts.Add(BaharTxt);
        allOpts.Add(FirstAndarTxt);
        allOpts.Add(FirstBaharTxt);
        allOpts.Add(FirstThreeTxt);

        foreach (var opt in allOpts)
        {
            if (opt == null) continue;
            // Skip winning options — they need to keep showing the win total
            if (resultsOptions.Contains(opt)) continue;
            ResetBetUI(opt);
            opt.totalBet = 0;
            opt.playerBet = 0;
        }
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

    internal void DisableAllWinRatioTexts()
    {
        foreach (var opt in AllOptions)
        {
            opt.DisableWinRatioText();
        }
        foreach (var opt in BiggerOptions)
        {
            opt.DisableWinRatioText();
        }
        FirstThreeTxt.DisableWinRatioText();
    }

    internal void EnableAllWinRatioTexts()
    {
        foreach (var opt in AllOptions)
        {
            opt.EnableWinRatioText();
        }
        foreach (var opt in BiggerOptions)
        {
            opt.EnableWinRatioText();
        }
        // Always keep First 3 disabled
        FirstThreeTxt.DisableWinRatioText();
    }

    #endregion
    internal void UpdateHomeScreenPlayerCount(Lobby lobby, int totalCount)
    {
        if (homepage == null) return;
        if (lobby == null) return;

        homepage.CPlayerCount.text = lobby.casual.ToString();
        homepage.NPlayerCount.text = lobby.novice.ToString();
        homepage.EPlayerCount.text = lobby.expert.ToString();
        homepage.HPlayerCount.text = lobby.high_roller.ToString();
        homepage.TotalPlayerCount.text = totalCount.ToString();

        // Disable all hot game objects
        if (homepage.CHotGame) homepage.CHotGame.SetActive(false);
        if (homepage.NHotGame) homepage.NHotGame.SetActive(false);
        if (homepage.EHotGame) homepage.EHotGame.SetActive(false);
        if (homepage.HHotGame) homepage.HHotGame.SetActive(false);

        // Check total players
        int totalPlayers = lobby.casual + lobby.novice + lobby.expert + lobby.high_roller;

        if (totalPlayers == 0)
        {
            // No players anywhere - disable all parent objects
            homepage.CPlayerCountParent.gameObject.SetActive(false);
            homepage.NPlayerCountParent.gameObject.SetActive(false);
            homepage.EPlayerCountParent.gameObject.SetActive(false);
            homepage.HPlayerCountParent.gameObject.SetActive(false);
        }
        else
        {
            // Players exist - enable parents where count > 0
            homepage.CPlayerCountParent.gameObject.SetActive(lobby.casual > 0);
            homepage.NPlayerCountParent.gameObject.SetActive(lobby.novice > 0);
            homepage.EPlayerCountParent.gameObject.SetActive(lobby.expert > 0);
            homepage.HPlayerCountParent.gameObject.SetActive(lobby.high_roller > 0);

            // Find room with highest player count
            int maxPlayers = Mathf.Max(lobby.casual, lobby.novice, lobby.expert, lobby.high_roller);

            // Count how many rooms have the max player count
            int roomsWithMax = 0;
            if (lobby.casual == maxPlayers && lobby.casual > 0) roomsWithMax++;
            if (lobby.novice == maxPlayers && lobby.novice > 0) roomsWithMax++;
            if (lobby.expert == maxPlayers && lobby.expert > 0) roomsWithMax++;
            if (lobby.high_roller == maxPlayers && lobby.high_roller > 0) roomsWithMax++;

            // Only show hot if one room uniquely has the highest count
            bool hasHot = roomsWithMax == 1;

            if (hasHot)
            {
                // One room is hot (red), others are normal (green)
                if (lobby.casual == maxPlayers && homepage.CHotGame) homepage.CHotGame.SetActive(true);
                if (lobby.casual == maxPlayers && homepage.CPlayerCountImage) homepage.CPlayerCountImage.sprite = homepage.HotGameRedSprite;
                else if (lobby.casual > 0 && homepage.CPlayerCountImage) homepage.CPlayerCountImage.sprite = homepage.NormalGameGreenSprite;

                if (lobby.novice == maxPlayers && homepage.NHotGame) homepage.NHotGame.SetActive(true);
                if (lobby.novice == maxPlayers && homepage.NPlayerCountImage) homepage.NPlayerCountImage.sprite = homepage.HotGameRedSprite;
                else if (lobby.novice > 0 && homepage.NPlayerCountImage) homepage.NPlayerCountImage.sprite = homepage.NormalGameGreenSprite;

                if (lobby.expert == maxPlayers && homepage.EHotGame) homepage.EHotGame.SetActive(true);
                if (lobby.expert == maxPlayers && homepage.EPlayerCountImage) homepage.EPlayerCountImage.sprite = homepage.HotGameRedSprite;
                else if (lobby.expert > 0 && homepage.EPlayerCountImage) homepage.EPlayerCountImage.sprite = homepage.NormalGameGreenSprite;

                if (lobby.high_roller == maxPlayers && homepage.HHotGame) homepage.HHotGame.SetActive(true);
                if (lobby.high_roller == maxPlayers && homepage.HPlayerCountImage) homepage.HPlayerCountImage.sprite = homepage.HotGameRedSprite;
                else if (lobby.high_roller > 0 && homepage.HPlayerCountImage) homepage.HPlayerCountImage.sprite = homepage.NormalGameGreenSprite;
            }
            else
            {
                // No hot (tie) - all green
                if (lobby.casual > 0 && homepage.CPlayerCountImage) homepage.CPlayerCountImage.sprite = homepage.NormalGameGreenSprite;
                if (lobby.novice > 0 && homepage.NPlayerCountImage) homepage.NPlayerCountImage.sprite = homepage.NormalGameGreenSprite;
                if (lobby.expert > 0 && homepage.EPlayerCountImage) homepage.EPlayerCountImage.sprite = homepage.NormalGameGreenSprite;
                if (lobby.high_roller > 0 && homepage.HPlayerCountImage) homepage.HPlayerCountImage.sprite = homepage.NormalGameGreenSprite;
            }
        }
    }
    internal void UpdateGamePlayerCount(int playerCount)
    {
        if (TotalPlayer_text != null)
        {
            TotalPlayer_text.text = playerCount.ToString();
        }
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
            BonusObject.gameObject.SetActive(false);
            animHand.MiddleCard.gameObject.SetActive(false);
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