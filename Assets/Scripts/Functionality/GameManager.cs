using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


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
    [SerializeField] private TMP_Text LoadingPage_text;
    [SerializeField] private TMP_Text TotalCardsCount_text;
    [SerializeField] private TMP_Text NextRoundCount_text;
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

    [Header("Chip")]
    [SerializeField] private List<Sprite> PlayerChipSprite;
    [SerializeField] private List<Sprite> otherChipSprite;
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private int initialCount = 20;
    [SerializeField] private Transform poolParent;

    private readonly List<GameObject> pool = new List<GameObject>();

    [Header("popup")]
    [SerializeField] private Button BetBlocker;
    [SerializeField] private GameObject BlockerText;
    [SerializeField] private Transform popStart;
    [SerializeField] private Transform popCenter;
    [SerializeField] private Transform popEnd;

    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float holdDuration = 1f;
    private Coroutine animRoutine;

    [Header("Result ")]
    [SerializeField] private GameObject AndarHighLight;
    [SerializeField] private GameObject BaharHighLight;
    [SerializeField] private GameObject AndarbaharBetReset;

    public float popScale = 1.15f;
    public float animTime = 0.15f;

    internal int BetCounter;
    internal int MultiplierCounter;
    internal string currentRoom;
    private double currentTotalBet = 0;
    private double currentBalance;
    private double animationduration = 2f;

    private Coroutine StartGameCorutine;
    private Coroutine EndGameCorutine;

    private Vector3 startPos = new Vector3(0, -138, 0);
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
        Handanimator.Play("MiddleCard");
        BetCounter = 0;
        HomePage.SetActive(true);
        LoadingPage.SetActive(false);
        GamePage.SetActive(false);

        BetBlocker.onClick.AddListener(PlayPopup);

    }


    #region  DataSetup
    internal void SetInitialData()
    {

        homepage.SetInitHomedata(socketManager.initialData);
        SetPlayerData(socketManager.playerdata);
    }

    internal void SetLoadingPage(bool isActive)
    {
        LoadingPage.SetActive(isActive);
        if (isActive) StartCoroutine(ManageloadingPageText());
    }
    IEnumerator ManageloadingPageText()
    {
        for (int i = 0; i < 25; i++)
        {
            if (i < 8) LoadingPage_text.text = "Joining A Room.....";
            else LoadingPage_text.text = "Waition For New Round To Start.....";

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
        AndarTxt.SetData(0, "andar", socketManager.initialData.wagers.main_bets.andar.payout[0].ToString(), "main_bets");
        BaharTxt.SetData(1, "bahar", socketManager.initialData.wagers.main_bets.bahar.payout[0].ToString(), "main_bets");

        FirstAndarTxt.SetData(2, "firstOneAndar", socketManager.initialData.wagers.op_bets.first_1_andar.payout[0].ToString(), "op_bets");
        FirstBaharTxt.SetData(3, "firstOneBahar", socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0].ToString(), "op_bets");
        FirstThreeTxt.SetData(4, "firstThree", socketManager.initialData.wagers.op_bets.first_3.payout.straight.ToString(), "op_bets");
        OptionQTxt.SetData(5, "1-5 Cards", socketManager.initialData.wagers.side_bets.s_1_5.payout.ToString(), "side_bets");
        OptionWTxt.SetData(6, "6-10 Cards", socketManager.initialData.wagers.side_bets.s_6_10.payout.ToString(), "side_bets");
        OptionETxt.SetData(7, "11-15 Cards", socketManager.initialData.wagers.side_bets.s_11_15.payout.ToString(), "side_bets");
        OptionRTxt.SetData(8, "15-20 Cards", socketManager.initialData.wagers.side_bets.s_16_20.payout.ToString(), "side_bets");
        OptionTTxt.SetData(9, "21-25 Cards", socketManager.initialData.wagers.side_bets.s_21_25.payout.ToString(), "side_bets");
        OptionYTxt.SetData(10, "26-30 Cards", socketManager.initialData.wagers.side_bets.s_26_30.payout.ToString(), "side_bets");
        OptionUTxt.SetData(11, "31-35 Cards", socketManager.initialData.wagers.side_bets.s_31_35.payout.ToString(), "side_bets");
        OptionITxt.SetData(12, "36-40 Cards", socketManager.initialData.wagers.side_bets.s_36_40.payout.ToString(), "side_bets");
        OptionOTxt.SetData(13, "41-45 Cards", socketManager.initialData.wagers.side_bets.s_41_53.payout.ToString(), "side_bets");

    }

    internal void SetCoinData()
    {
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

        for (int i = 0; i < uiManager.Coins.Count; i++)
            uiManager.Coins[i].Chiptext.text = data[i + 1].ToString();
    }


    void SetPlayerData(Player player)
    {
        homepage.setPlayerData(socketManager.playerdata);
        uiManager.MainPlayers.SetData(player.username, player.balance.ToString(), uiManager.UserIcons[0]);
    }
    internal void SetOtherplayerData(Leaderboards leaderboard)
    {
        if (leaderboard == null)
        {
            Debug.LogError("Leaderboards is NULL");
            return;
        }


        if (leaderboard.richest == null || leaderboard.richest.Count == 0)
        {
            Debug.Log("No richest players");
            foreach (var item in uiManager.RichestPlayers)
                item.gameObject.SetActive(false);

            return;
        }

        foreach (var item in uiManager.RichestPlayers)
            item.gameObject.SetActive(false);

        int richestCount = Mathf.Min(leaderboard.richest.Count, uiManager.RichestPlayers.Count);

        for (int i = 0; i < richestCount; i++)
        {
            int r = UnityEngine.Random.Range(0, uiManager.UserIcons.Count);
            Richest rich = leaderboard.richest[i];

            uiManager.RichestPlayers[i].SetData(
                rich.username,
                rich.balance.ToString(),
                uiManager.UserIcons[r]
            );
            uiManager.RichestPlayers[i].gameObject.SetActive(true);
        }


        if (leaderboard.winners == null || leaderboard.winners.Count == 0)
        {
            Debug.Log("No winners players");
            foreach (var item in uiManager.WinnerPlayers)
                item.gameObject.SetActive(false);

            return;
        }

        foreach (var item in uiManager.WinnerPlayers)
            item.gameObject.SetActive(false);

        int winnersCount = Mathf.Min(leaderboard.winners.Count, uiManager.WinnerPlayers.Count);

        for (int i = 0; i < winnersCount; i++)
        {
            int r = UnityEngine.Random.Range(0, uiManager.UserIcons.Count);
            Winner win = leaderboard.winners[i];

            uiManager.WinnerPlayers[i].SetData(
                win.username,
                win.totalWins.ToString(),
                uiManager.UserIcons[r]
            );
            uiManager.WinnerPlayers[i].gameObject.SetActive(true);
        }
    }



    #endregion




    #region GamePlay
    internal void SetMainCard()
    {
        if (EndGameCorutine != null)
        {
            EndGameCorutine = null;
        }
        StartGameCorutine = StartCoroutine(StartCountdown());
    }
    internal void StartGame()
    {
        if (StartGameCorutine != null)
        {
            StartGameCorutine = null;
        }
        EndGameCorutine = StartCoroutine(GameLoop());
    }
    internal void StartGameMidway()
    {
        if (StartGameCorutine != null)
        {
            StartGameCorutine = null;
        }
        animHand.MiddleSprite = CardSet(socketManager.gameLoopData.middleCard.suit, socketManager.gameLoopData.middleCard.rank);
        animHand.MiddleCard.gameObject.SetActive(true);
        EndGameCorutine = StartCoroutine(GameLoop());
    }
    IEnumerator StartCountdown()
    {
        audioManager.PlayGirlAudio("placeyourbet");
        BetBlocker.gameObject.SetActive(false);
        uiManager.setCoins(true);
        uiManager.SetChipoption(true);
        uiManager.SetNetBetPanel(false);


        PlayMiddleCardAnim();
        yield return new WaitForSeconds(2f);
        for (int i = 25; i > 0; i--)
        {
            TotalCardsCount_text.text = "Next Round\n" + i.ToString();
            yield return new WaitForSeconds(1f);
            if (i == 6) audioManager.PlayGirlAudio("timeisrunning");
        }
        BetBlocker.gameObject.SetActive(true);
        audioManager.PlayGirlAudio("nomorebets");

    }
    IEnumerator GameLoop()
    {
        int a = socketManager.gameLoopData.andarCards.Count;
        int b = socketManager.gameLoopData.baharCards.Count;

        bool startWithAndar = socketManager.gameLoopData.middleCard.color == "blue";

        int max = Mathf.Max(a, b);
        int delivered = 0;

        for (int i = 0; i < max; i++)
        {
            if (startWithAndar)
            {
                if (i < a) { PlayAndarCardAnim(i); delivered++; ManageCardCounts(delivered); yield return new WaitForSeconds(0.5f); AddAndarCard(animHand.LeftSprite); yield return new WaitForSeconds(0.5f); }
                if (i < b) { PlayBagarCardAnim(i); delivered++; ManageCardCounts(delivered); yield return new WaitForSeconds(0.5f); AddBaharCard(animHand.RightSprite); yield return new WaitForSeconds(0.5f); }
            }
            else
            {
                if (i < b) { PlayBagarCardAnim(i); delivered++; ManageCardCounts(delivered); yield return new WaitForSeconds(0.5f); AddBaharCard(animHand.RightSprite); yield return new WaitForSeconds(0.5f); }
                if (i < a) { PlayAndarCardAnim(i); delivered++; ManageCardCounts(delivered); yield return new WaitForSeconds(0.5f); AddAndarCard(animHand.LeftSprite); yield return new WaitForSeconds(0.5f); }
            }
        }
        if (socketManager.gameLoopData.matchSide == "andar") uiManager.UpdateStats(socketManager.gameLoopData.middleCard.rank, true, delivered.ToString());
        else uiManager.UpdateStats(socketManager.gameLoopData.middleCard.rank, false, delivered.ToString());
        uiManager.CalculateAndShowPercentage();
        PlayWinAnimations();

        yield return new WaitForSeconds(1f);
        StartCoroutine(ManagePayout());

        yield return new WaitForSeconds(1f);


        Handanimator.Play("RemoveAllCard");
        ResetCardHistory();
        AndarHighLight.SetActive(false);
        BaharHighLight.SetActive(false);
        yield return new WaitForSeconds(1f);
        PlayResetAnimation();
    }

    IEnumerator ManagePayout()
    {
        foreach (var item in AllOptions)
        {
            item.BG.SetActive(true);
            item.HighlightedBG.SetActive(false);
        }
        MoveAllChipstohome();
        yield return new WaitForSeconds(1f);
        UpdatePlayerbalance(socketManager.gameLoopData.amount.ToString());
        DistributeAllPayout();
        yield return null;
    }
    void MoveAllChipstohome()
    {
        foreach (var item in PlayerChips)
        {
            MoveChip(item.chip.transform, item.chip.transform, TotalCardsCount_text.transform, true);
        }
        foreach (var item in OtherPlayerChips)
        {
            MoveChip(item.chip.transform, item.chip.transform, TotalCardsCount_text.transform, true);
        }
    }
    void DistributeAllPayout()
    {
        DistributePayouts(socketManager.CashoutData.payouts);
    }















    void ManageCardCounts(int count)
    {
        NextRoundCount_text.text = count + "\nCards";


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
    void PlayAndarCardAnim(int index)
    {
        AndarHighLight.SetActive(true);
        BaharHighLight.SetActive(false);
        animHand.LeftSprite = CardSet(socketManager.gameLoopData.andarCards[index].suit, socketManager.gameLoopData.andarCards[index].rank);
        Handanimator.Play("LeftCard");
    }
    void PlayBagarCardAnim(int index)
    {
        AndarHighLight.SetActive(false);
        BaharHighLight.SetActive(true);
        animHand.RightSprite = CardSet(socketManager.gameLoopData.baharCards[index].suit, socketManager.gameLoopData.baharCards[index].rank);
        Handanimator.Play("RightCard");
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
        socketManager.BetPlaced(index, optionprefab.VaridontWant, socketManager.initialData.betOptions[optionprefab.Optionindex]);


    }
    internal void ManageBrodcastBetsPlayer()
    {
        if (socketManager == null)
        {
            Debug.LogError("socketManager is NULL");
            return;
        }

        if (socketManager.BetChipData == null)
        {
            Debug.LogError("BetChipData is NULL (server not sent or not deserialized)");
            return;
        }

        if (socketManager.BetChipData.payload.betOption == null)
        {
            Debug.LogError("payload is NULL (JSON missing payload field)");
            return;
        }
        ChipData data = new ChipData();
        data.betId = socketManager.BetChipData.payload.betId;
        data.amount = socketManager.BetChipData.payload.amount;

        string val = uiManager.coinSelector.Chiptext.text;
        int index = uiManager.coinSelector.chipIndex;
        data.chip = SpawnChip(uiManager.coinSelector.chipImage.sprite, val, index, uiManager.coinSelector.transform, FindOption(socketManager.BetChipData.payload.betOption));

        PlayerChips.Add(data);

    }
    internal void ManageBrodcastBetsOtherPlayers(Root chipdata)
    {
        bool existsAnywhere =
    PlayerChips.Exists(x => x.betId == chipdata.betId) ||
    OtherPlayerChips.Exists(x => x.betId == chipdata.betId);

        if (existsAnywhere)
        {
            return;
        }

        ChipData data = new ChipData();
        data.betId = chipdata.betId;
        data.amount = chipdata.amount;

        string val = FindRoom()[chipdata.amount].ToString(); ;
        int index = chipdata.amount;
        data.chip = SpawnChip(findChipSprite(chipdata.amount, FindRoom()), val, index, TotalPlayer_text.transform, FindOption(chipdata.betOption));

        OtherPlayerChips.Add(data);

    }



    GameObject SpawnChip(Sprite sprite, string amount, int chipindex, Transform startPoint, OptionPrefab op, float moveTime = 0.4f)
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

        chipRT.position = startPoint.position;
        chipRT.DOMove(op.chiparea.TransformPoint(randomPos), moveTime);
        return chip.gameObject;
    }








    #endregion


    #region Manage Result and reset
    void PlayWinAnimations()
    {
        if (socketManager.gameLoopData.matchSide == "andar") AndarTxt.winAnimation.StartAnimation();
        else BaharTxt.winAnimation.StartAnimation();

        foreach (var item in AllOptions)
        {
            if (item.HighlightedBG.activeInHierarchy)
            {
                item.winAnimation.StartAnimation();
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
        float targetX = -1252f;

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPosX(targetX, 0.8f).SetEase(Ease.OutBounce))

           // It stops here
           .AppendInterval(0.5f)

           .AppendCallback(() =>
           {
               NextRoundCount_text.text = "";
               TotalCardsCount_text.text = "";
           })


           .Append(rt.DOAnchorPosX(startX, 0.6f).SetEase(Ease.InOutSine));
    }


    private Chip SpawnChipFromPool()
    {
        Chip chip = GetChip();   // your pool code
        chip.transform.SetParent(poolParent);
        chip.transform.position = TotalCardsCount_text.transform.position;
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
        if (uiManager.MainPlayers.PlayerId == playerId)
            return uiManager.MainPlayers.transform;

        foreach (var p in uiManager.RichestPlayers)
            if (p.PlayerId == playerId)
                return p.transform;

        foreach (var p in uiManager.WinnerPlayers)
            if (p.PlayerId == playerId)
                return p.transform;

        return null;
    }

    public void DistributePayouts(List<Payout> payouts)
    {
        foreach (var payout in payouts)
        {
            Transform target = FindPlayerTransform(payout.userId);

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

    internal void DoubleBets(List<Bet> bets)
    {
        foreach (var bet in bets)
        {
            ChipData data = new ChipData();
            data.betId = bet.betId;
            data.amount = bet.oldAmount;

            string val = bet.oldAmount.ToString();
            int index = findChipindex(bet.oldAmount, FindRoom());
            data.chip = SpawnChip(findChipSprite(bet.oldAmount, FindRoom()), val, index, uiManager.coinSelector.transform, FindOption(bet.betOption));

            PlayerChips.Add(data);
        }

    }
    internal void CancleBets()
    {
        foreach (var chips in PlayerChips)
        {
            Chip c = chips.chip.GetComponent<Chip>();
            if (chips != null)
                ReturnChip(c);
        }
        PlayerChips.Clear();
    }
    internal void UnduBets(string betId)
    {
        // Debug.Log("sdhfjhdfijhdijfhjkadshfjkdhkfhakshfkahsdfkhsdkfhksaj");
        for (int i = PlayerChips.Count - 1; i >= 0; i--)
        {
            var item = PlayerChips[i];

            if (item.betId == betId)
            {
                Chip c = item.chip.GetComponent<Chip>();
                ReturnChip(c);
                PlayerChips.RemoveAt(i);
            }
        }
    }

    #endregion



    #region add a card in list

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






    #endregion





    #region chip Pool







    GameObject AddChip()
    {
        var go = Instantiate(chipPrefab, poolParent);
        go.SetActive(false);
        pool.Add(go);
        return go;
    }

    internal Chip GetChip()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i].GetComponent<Chip>();
            }
        }

        return AddChip().GetComponent<Chip>();
    }

    internal void ReturnChip(Chip chip)
    {
        chip.gameObject.SetActive(false);
        chip.transform.SetParent(poolParent);
    }





    #endregion






    #region cardSelection
    private Sprite CardSet(string suit, string value)
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



    #region helper

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
        //  Debug.Log("789 _____________" + opt);
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

    private void PlayPopup()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        BlockerText.transform.position = popStart.position;
        yield return Move(BlockerText.transform, popCenter.position, moveDuration);

        yield return new WaitForSeconds(holdDuration);

        yield return Move(BlockerText.transform, popEnd.position, moveDuration);
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


    #endregion


}

[System.Serializable]
public class ChipData
{
    public string betId;
    public string username;
    public int amount;

    public GameObject chip;
}