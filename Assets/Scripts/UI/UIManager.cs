using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Linq;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioController;
    [SerializeField]
    private SocketIOManager socketManager;
    [SerializeField] private JSFunctCalls jsFunctCalls;

    [Header("Screens UI")]
    [SerializeField] private GameObject HomeScreen_Object;
    [SerializeField] private GameObject GameScreen_Object;

    [Header("Topbar")]
    [SerializeField]
    internal TMP_Text MinBet;
    [SerializeField]
    internal TMP_Text Username;
    [SerializeField]
    internal TMP_Text Rayid;
    [Header("Andar Bahar Main Buttons")]
    [SerializeField] private GameObject ButtonPanels;
    [SerializeField] private Button HistoryMain_button;
    [SerializeField] internal Button MenuMain_button;
    [SerializeField] private Button CasualGame_button;
    [SerializeField] private Button NoviceGame_button;
    [SerializeField] private Button ExpertGame_button;
    [SerializeField] private Button HighRollerGame_button;

    [Header("Andar Bahar")]
    [SerializeField] internal Button MenuInGame_button;
    [SerializeField] private Button History_button;
    [SerializeField] private Button Info_button;
    [SerializeField] private Button Sound_button;
    [SerializeField] private Button Music_button;
    [SerializeField] private Button SoundMute_button;
    [SerializeField] private Button MusicMute_button;
    [SerializeField] private Button ExpandHome_Button;
    [SerializeField] private Button ShrinkHome_Button;
    [SerializeField] private TMP_Text Expand_Button_Text;
    [SerializeField] private Button Home_button;
    [SerializeField] private Button Exit_Button;
    [SerializeField] private Button YesHome_button;
    [SerializeField] private Button NoHome_button;

    [SerializeField] private GameObject MenuPanel_Object;
    [SerializeField] private GameObject MenuPanelContainer_Object;
    [SerializeField] private GameObject Homebutton_Object;
    [SerializeField] private GameObject ExitButton_Object;

    [SerializeField] private Button HistoryClose_button;

    [SerializeField] private Button InfoClose_button;

    [SerializeField] private Button InfoLeft_button;

    [SerializeField] private Button InfoRight_button;

    [SerializeField] private List<GameObject> InfoPages_Objects;
    [SerializeField] private List<GameObject> InfoActive_Objects;
    private int currentInfoPage = 0;

    private bool IsMenuPanelOpen = false;


    [Header("Game Rules")]
    [SerializeField] private List<TMP_Text> PayoutText;










    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField] private GameObject GameQuitPopup;
    [SerializeField] private GameObject HistoryPopup_Object;
    [SerializeField] private GameObject InfoPopup_Object;



    [Header("Settings Popup")]
    [SerializeField]
    internal GameObject sideMenuePanel;
    [SerializeField]
    private GameObject SettingsPopup_Object;
    [SerializeField]
    private Button SettingsExit_Button;
    [SerializeField]
    private Button Sound_Button;
    [SerializeField]
    private Button Music_Button;

    [SerializeField]
    private GameObject MusicOn_Object;
    [SerializeField]
    private GameObject MusicOff_Object;
    [SerializeField]
    private GameObject SoundOn_Object;
    [SerializeField]
    private GameObject SoundOff_Object;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;
    [Header("History Popup")]
    [SerializeField]
    private GameObject Pageparent;

    [SerializeField] private GameObject HistoryPrefab;
    [SerializeField] private TMP_Text HistoryNav;
    [SerializeField] private int CurrentHistoryPage;
    [SerializeField] private int MaxHistoryPage;
    [SerializeField] private Button HistoryLeft;
    [SerializeField] private Button HistoryRight;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject ExitButton;
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;
    [SerializeField]
    private Button CrossQuit_Button;

    [SerializeField]
    internal GameObject touchDisable;
    [SerializeField]
    private Button Settings_Button;
    [SerializeField]
    private Button Paytable_Button;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private Button GameExit_Button;
    [SerializeField]
    private GameManager gameManager;

    bool isExit;
    bool isMusic;
    bool isSound;
    bool homepopup;

    internal bool isExpanded = false;
    public float duration = 0.5f;



    [Header("stats")]
    [SerializeField] private List<StatsPrefab> LineStats;
    [SerializeField] private GameObject StatsPref;
    [SerializeField] private Transform StatsParent;
    [SerializeField] private Image FillAb;
    [SerializeField] private TMP_Text AndarPercentage;
    [SerializeField] private TMP_Text BaharPercentage;
    [SerializeField] private Image Fillprobability;
    [SerializeField] private TMP_Text AndarProb;
    [SerializeField] private TMP_Text BaharProbab;
    [SerializeField] private TMP_Text cardProb;

    private List<StatsPrefab> gridStats = new List<StatsPrefab>();
    private const int MAX_GRID = 26;


    [Header("coins")]
    [SerializeField] internal GameObject chipPanel;
    [SerializeField] internal Chip coinSelector;
    [SerializeField] internal Button coinSelectorBtn;
    [SerializeField] internal List<Chip> Coins;

    [Header("Chipoptions")]
    [SerializeField] internal GameObject Repeatpanel;
    [SerializeField] internal Button Repeatbtn;
    [SerializeField] internal GameObject chiOptionpanel;
    [SerializeField] internal Button Undubtn;
    [SerializeField] internal Button Canclebtn;
    [SerializeField] internal Button Doublebtn;
    [SerializeField] internal GameObject NetBetPanel;
    [SerializeField] internal TMP_Text NetBet;
    [Header("player data")]
    [SerializeField] internal PlayerData MainPlayers;
    [SerializeField] internal List<PlayerData> RichestPlayers;
    [SerializeField] internal List<PlayerData> WinnerPlayers;
    [SerializeField] internal List<Sprite> UserIcons;
    [Header("SetBetLimit  data")]
    [SerializeField] internal Button BetLimitQuitBtn;
    [SerializeField] internal Button BetLimitBtn;
    [SerializeField] internal Sprite SelectedBtn;
    [SerializeField] internal Sprite NonSelectedBtn;
    [SerializeField] internal GameObject BetLimitPanel;
    [SerializeField] internal Button betBtnQ;
    [SerializeField] internal Button betBtnW;
    [SerializeField] internal Button betBtnE;
    [SerializeField] internal Button betBtnR;
    [SerializeField] internal List<TMP_Text> BetLimitdata;
    [SerializeField] internal Button ConfirmBtn;
    [SerializeField] internal GameObject Disclamer;
    [Header("Intro page")]
    [SerializeField] internal GameObject Intropage;
    [SerializeField] private Button CloseIntroPage;
    [SerializeField] private Button ReadmoreBtn;
    [SerializeField] private Button DontShowBtn;
    [SerializeField] private GameObject tick;

    [Header("Quit Popup Animation ")]
    [SerializeField] private ImageAnimation QuitpopupAnim;
    [SerializeField] private ImageAnimation YesBtnParticles;
    [SerializeField] private ImageAnimation NoBtnParticles;
    [SerializeField] private List<Sprite> QuitStartSprits;
    [SerializeField] private List<Sprite> QuitEndSprits;
    [SerializeField] private Button HomePageHistoryBtn;

    private void Start()
    {
        RetractCoins();
        if (coinSelectorBtn) coinSelectorBtn.onClick.RemoveAllListeners();
        if (coinSelectorBtn) coinSelectorBtn.onClick.AddListener(delegate { ToggleCoins(); });

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); if (audioController) audioController.PlayButtonAudio(); });

        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

        if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
        if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); if (audioController) audioController.PlayButtonAudio(); });

        if (MusicOn_Object) MusicOn_Object.SetActive(true);
        if (MusicOff_Object) MusicOff_Object.SetActive(false);

        if (SoundOn_Object) SoundOn_Object.SetActive(true);
        if (SoundOff_Object) SoundOff_Object.SetActive(false);

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
        {
            // OpenPopup(QuitPopup_Object);
            SetQuitPopupAnimation(true);
            Debug.Log("Quit event: pressed Big_X button");
            if (audioController) audioController.PlayButtonAudio();

        });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                //ClosePopup(QuitPopup_Object);
                StartCoroutine(OnCliqQuitBtn(false));
                Debug.Log("quit event: pressed NO Button ");
                if (audioController) audioController.PlayButtonAudio();
            }
        });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
        {
            if (!isExit)
            {
                ClosePopup(QuitPopup_Object);
                Debug.Log("quit event: pressed Small_X Button ");
                if (audioController) audioController.PlayButtonAudio();
            }
        });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); if (audioController) audioController.PlayButtonAudio(); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
        {
            StartCoroutine(OnCliqQuitBtn(true));
            if (audioController) audioController.PlayButtonAudio();
            // CallOnExitFunction();
            // Debug.Log("quit event: pressed YES Button ");
            // socketManager.ReactNativeCallOnFailedToConnect();
        });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener((delegate { CallOnExitFunction(); socketManager.ReactNativeCallOnFailedToConnect(); }));

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);



        if (audioController) audioController.ToggleMute(false);

        isMusic = true;
        isSound = true;

        if (Sound_Button)
        {
            Sound_Button.onClick.RemoveAllListeners();
            Sound_Button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(Sound_Button, ToggleSound);
            });
        }

        if (Music_Button)
        {
            Music_Button.onClick.RemoveAllListeners();
            Music_Button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(Music_Button, ToggleMusic);
            });
        }


        // Andar Bahar 
        if (HistoryMain_button) HistoryMain_button.onClick.RemoveAllListeners();
        if (HistoryMain_button) HistoryMain_button.onClick.AddListener(delegate { OpenPopup(HistoryPopup_Object); });
        if (HomePageHistoryBtn) HistoryMain_button.onClick.RemoveAllListeners();
        if (HomePageHistoryBtn) HistoryMain_button.onClick.AddListener(delegate { OpenPopup(HistoryPopup_Object); HistorypageOpen(); });

        if (MenuMain_button)
        {
            MenuMain_button.onClick.RemoveAllListeners();
            MenuMain_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(MenuMain_button, () =>
                {
                    ResetMenuPanel(false);
                    ToggleMenuPanel();
                    if (audioController)
                        audioController.PlayButtonAudio();
                });
            });
        }

        if (MenuInGame_button)
        {
            MenuInGame_button.onClick.RemoveAllListeners();
            MenuInGame_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(MenuInGame_button, () =>
                {
                    ResetMenuPanel(true);
                    ToggleMenuPanel();
                    if (audioController)
                        audioController.PlayButtonAudio();
                });
            });
        }

        if (CasualGame_button) CasualGame_button.onClick.RemoveAllListeners();
        if (CasualGame_button) CasualGame_button.onClick.AddListener(delegate { ResetMenuPanel(true); GameScreen_Object.SetActive(true); if (audioController) audioController.PlayButtonAudio(); });

        if (NoviceGame_button) NoviceGame_button.onClick.RemoveAllListeners();
        if (NoviceGame_button) NoviceGame_button.onClick.AddListener(delegate { ResetMenuPanel(true); GameScreen_Object.SetActive(true); if (audioController) audioController.PlayButtonAudio(); });

        if (ExpertGame_button) ExpertGame_button.onClick.RemoveAllListeners();
        if (ExpertGame_button) ExpertGame_button.onClick.AddListener(delegate { ResetMenuPanel(true); GameScreen_Object.SetActive(true); if (audioController) audioController.PlayButtonAudio(); });

        if (HighRollerGame_button) HighRollerGame_button.onClick.RemoveAllListeners();
        if (HighRollerGame_button) HighRollerGame_button.onClick.AddListener(delegate { ResetMenuPanel(true); GameScreen_Object.SetActive(true); if (audioController) audioController.PlayButtonAudio(); });
        if (Info_button)
        {
            Info_button.onClick.RemoveAllListeners();
            Info_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(Info_button, () =>
                {
                    OpenPopup(InfoPopup_Object);
                    MenuPanel_Object.SetActive(false);
                });
            });
        }

        if (History_button)
        {
            History_button.onClick.RemoveAllListeners();
            History_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(History_button, () =>
                {
                    OpenPopup(HistoryPopup_Object);
                    HistorypageOpen();
                    MenuPanel_Object.SetActive(false);
                });
            });
        }

        if (Sound_button)
        {
            Sound_button.onClick.RemoveAllListeners();
            Sound_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(Sound_button, () =>
                {
                    ToggleSound();
                });
            });
        }

        if (SoundMute_button)
        {
            SoundMute_button.onClick.RemoveAllListeners();
            SoundMute_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(SoundMute_button, () =>
                {
                    ToggleSound();
                });
            });
        }

        if (Music_button)
        {
            Music_button.onClick.RemoveAllListeners();
            Music_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(Music_button, () =>
                {
                    ToggleMusic();
                });
            });
        }

        if (MusicMute_button)
        {
            MusicMute_button.onClick.RemoveAllListeners();
            MusicMute_button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(MusicMute_button, () =>
                {
                    ToggleMusic();
                });
            });
        }
        if (ExpandHome_Button)
        {
            ExpandHome_Button.onClick.RemoveAllListeners();
            ExpandHome_Button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(ExpandHome_Button, () =>
                {
                    OnExpand();
                });
            });
        }
        if (ShrinkHome_Button)
        {
            ShrinkHome_Button.onClick.RemoveAllListeners();
            ShrinkHome_Button.onClick.AddListener(() =>
            {
                PlayButtonAnimation(ShrinkHome_Button, () =>
                {
                    OnShrink();
                });
            });
        }


        if (Home_button) Home_button.onClick.RemoveAllListeners();
        if (Home_button) Home_button.onClick.AddListener(delegate { homepopup = true; OpenPopup(QuitPopup_Object); SetQuitPopupAnimation(true); });
        if (Exit_Button) Exit_Button.onClick.RemoveAllListeners();
        if (Exit_Button) Exit_Button.onClick.AddListener(delegate { homepopup = false; OpenPopup(QuitPopup_Object); SetQuitPopupAnimation(true); if (audioController) audioController.PlayButtonAudio(); });

        if (YesHome_button) YesHome_button.onClick.RemoveAllListeners();
        if (YesHome_button) YesHome_button.onClick.AddListener(delegate { homepopup = true; StartCoroutine(OnCliqQuitBtn(true)); if (audioController) audioController.PlayButtonAudio(); });

        if (NoHome_button) NoHome_button.onClick.RemoveAllListeners();
        if (NoHome_button) NoHome_button.onClick.AddListener(delegate { StartCoroutine(OnCliqQuitBtn(false)); if (audioController) audioController.PlayButtonAudio(); });

        if (InfoLeft_button) InfoLeft_button.onClick.RemoveAllListeners();
        if (InfoLeft_button) InfoLeft_button.onClick.AddListener(delegate { GoToPreviousInfoPage(); });

        if (InfoRight_button) InfoRight_button.onClick.RemoveAllListeners();
        if (InfoRight_button) InfoRight_button.onClick.AddListener(delegate { GoToNextInfoPage(); });

        if (InfoClose_button) InfoClose_button.onClick.RemoveAllListeners();
        if (InfoClose_button) InfoClose_button.onClick.AddListener(delegate { PopAndDisable(InfoPopup_Object); IsMenuPanelOpen = false; if (audioController) audioController.PlayButtonAudio(); });

        if (HistoryClose_button) HistoryClose_button.onClick.RemoveAllListeners();
        if (HistoryClose_button) HistoryClose_button.onClick.AddListener(delegate { PopAndDisable(HistoryPopup_Object); IsMenuPanelOpen = false; if (audioController) audioController.PlayButtonAudio(); });


        Repeatbtn.onClick.RemoveAllListeners();
        Repeatbtn.onClick.AddListener(delegate { socketManager.SendRepeat(); Repeatpanel.SetActive(false); });

        Undubtn.onClick.RemoveAllListeners();
        Undubtn.onClick.AddListener(delegate { socketManager.SendUndo(); });

        Canclebtn.onClick.RemoveAllListeners();
        Canclebtn.onClick.AddListener(delegate { socketManager.SendCancle(); });

        Doublebtn.onClick.RemoveAllListeners();
        Doublebtn.onClick.AddListener(delegate { socketManager.SendDouble(); });

        HistoryLeft.onClick.RemoveAllListeners();
        HistoryLeft.onClick.AddListener(delegate { if (CurrentHistoryPage - 1 > 0) socketManager.SendHistory(CurrentHistoryPage - 1); });

        HistoryRight.onClick.RemoveAllListeners();
        HistoryRight.onClick.AddListener(delegate { if (CurrentHistoryPage + 1 <= MaxHistoryPage) socketManager.SendHistory(CurrentHistoryPage + 1); });

        DontShowBtn.onClick.RemoveAllListeners();
        DontShowBtn.onClick.AddListener(delegate { OnClickDontShow(); if (audioController) audioController.PlayButtonAudio(); });

        CloseIntroPage.onClick.RemoveAllListeners();
        CloseIntroPage.onClick.AddListener(delegate { PopAndDisable(Intropage); if (audioController) audioController.PlayButtonAudio(); });

        ReadmoreBtn.onClick.RemoveAllListeners();
        ReadmoreBtn.onClick.AddListener(delegate { ClosePopup(Intropage); OpenPopup(InfoPopup_Object); });

        BetLimitBtn.onClick.RemoveAllListeners();
        BetLimitBtn.onClick.AddListener(delegate
        {
            OnOpenBetLimit();
        });
        betBtnQ.onClick.RemoveAllListeners();
        betBtnQ.onClick.AddListener(delegate { OnChangeLimitClicked(betBtnQ, "casual"); });
        betBtnW.onClick.RemoveAllListeners();
        betBtnW.onClick.AddListener(delegate { OnChangeLimitClicked(betBtnW, "novice"); });
        betBtnE.onClick.RemoveAllListeners();
        betBtnE.onClick.AddListener(delegate { OnChangeLimitClicked(betBtnE, "expert"); });
        betBtnR.onClick.RemoveAllListeners();
        betBtnR.onClick.AddListener(delegate { OnChangeLimitClicked(betBtnR, "high_roller"); });
        ConfirmBtn.onClick.RemoveAllListeners();
        ConfirmBtn.onClick.AddListener(delegate { gameManager.OnClickNextroom(); ClosePopup(BetLimitPanel); });
        BetLimitQuitBtn.onClick.RemoveAllListeners();
        BetLimitQuitBtn.onClick.AddListener(delegate { ClosePopup(BetLimitPanel); });

        // OnClickDontShow();
        ShowIntroPage();
        //  SpawnDummyStats(30);
        RegisterFullscreenListener();
    }


    private void SpawnDummyStats(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string dummyCard = UnityEngine.Random.Range(1, 14).ToString();   // 1–13 cards
            bool isBlue = UnityEngine.Random.value > 0.5f;                   // random color
            string dummyCount = UnityEngine.Random.Range(1, 10).ToString();  // count number

            UpdateStats(dummyCard, isBlue, dummyCount);
        }
    }

    #region Everytheing else

    public void ResetMenuPanel(bool IsGameScreen)
    {
        //     SlideOutAndDisable(() =>
        // {
        //     MenuPanel_Object.SetActive(false);

        //     if (IsGameScreen)
        //     {
        //         Homebutton_Object.SetActive(true);
        //         ExitButton_Object.gameObject.SetActive(false);

        //         MenuPanelContainer_Object.GetComponent<RectTransform>().anchoredPosition
        //             = new Vector2(56, 394);
        //     }
        //     else
        //     {
        //         Homebutton_Object.SetActive(false);
        //         ExitButton_Object.gameObject.SetActive(true);

        //         MenuPanelContainer_Object.GetComponent<RectTransform>().anchoredPosition
        //             = new Vector2(56, 221);
        //     }
        // });

        MenuPanel_Object.SetActive(false);
        if (IsGameScreen)
        {
            Homebutton_Object.SetActive(true);
            ExitButton_Object.gameObject.SetActive(false);

            MenuPanelContainer_Object.GetComponent<RectTransform>().anchoredPosition = new Vector2(56, 394);


        }
        else
        {
            Homebutton_Object.SetActive(false);
            ExitButton_Object.gameObject.SetActive(true);

            MenuPanelContainer_Object.GetComponent<RectTransform>().anchoredPosition = new Vector2(56, 221);

        }
    }
    private Tween slideTween;
    public void SlideOutAndDisable(System.Action onComplete)
    {
        RectTransform rect = MenuPanel_Object.GetComponent<RectTransform>();

        slideTween?.Kill();

        slideTween = rect
            .DOAnchorPosX(800f, 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                onComplete?.Invoke();   // call your ResetMenuPanel logic
            });
    }
    public void ToggleMenuPanel()
    {
        if (IsMenuPanelOpen)
        {
            MenuPanel_Object.SetActive(false);
            IsMenuPanelOpen = false;
        }
        else
        {
            MenuPanel_Object.SetActive(true);
            IsMenuPanelOpen = true;
        }
    }

    internal void SetgameRulePanel()
    {
        float bet = 1;

        PayoutText[0].text = (socketManager.initialData.wagers.op_bets.first_3.payout.flush * bet).ToString();
        PayoutText[1].text = (socketManager.initialData.wagers.op_bets.first_3.payout.straight * bet).ToString();
        PayoutText[2].text = (socketManager.initialData.wagers.op_bets.first_3.payout.straight_flush * bet).ToString();

        PayoutText[3].text = (socketManager.initialData.wagers.op_bets.first_1_andar.payout[0] * bet).ToString();
        PayoutText[4].text = (socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0] * bet).ToString();

        PayoutText[5].text = (socketManager.initialData.wagers.op_bets.first_1_bahar.payout[0] * bet).ToString();
        PayoutText[6].text = (socketManager.initialData.wagers.op_bets.first_1_andar.payout[0] * bet).ToString();

        PayoutText[7].text = (socketManager.initialData.wagers.side_bets.s_1_5.payout * bet).ToString();
        PayoutText[8].text = (socketManager.initialData.wagers.side_bets.s_6_10.payout * bet).ToString();
        PayoutText[9].text = (socketManager.initialData.wagers.side_bets.s_11_15.payout * bet).ToString();
        PayoutText[10].text = (socketManager.initialData.wagers.side_bets.s_16_20.payout * bet).ToString();
        PayoutText[11].text = (socketManager.initialData.wagers.side_bets.s_21_25.payout * bet).ToString();
        PayoutText[12].text = (socketManager.initialData.wagers.side_bets.s_26_30.payout * bet).ToString();
        PayoutText[13].text = (socketManager.initialData.wagers.side_bets.s_31_35.payout * bet).ToString();
        PayoutText[14].text = (socketManager.initialData.wagers.side_bets.s_36_40.payout * bet).ToString();
        PayoutText[15].text = (socketManager.initialData.wagers.side_bets.s_41_53.payout * bet).ToString();

    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            CheckAndClosePopups();
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }



    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }


    private void CallOnExitFunction()
    {
        StartCoroutine(socketManager.CloseSocket());
        isExit = true;
        audioController.PlayButtonAudio();

    }


    internal void HistorypageOpen()
    {
        socketManager.SendHistory(1);
    }

    internal void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    internal void ClosePopup(GameObject Popup)
    {

        if (Popup) Popup.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }
    public void PopAndDisable(GameObject Popup)
    {
        Popup.transform.DOScale(0f, duration)
                 .SetEase(Ease.InBack)
                 .OnComplete(() =>
                 {
                     Popup.SetActive(false);
                     if (MainPopup_Object) MainPopup_Object.SetActive(false);
                 });
    }

    private void ToggleMusic()
    {
        isMusic = !isMusic;
        if (isMusic)
        {
            audioController.PlayButtonAudio();
            Music_button.gameObject.SetActive(true);
            MusicMute_button.gameObject.SetActive(false);
            audioController.ToggleMute(false, "bg");
        }
        else
        {
            audioController.PlayButtonAudio();
            Music_button.gameObject.SetActive(false);
            MusicMute_button.gameObject.SetActive(true);
            audioController.ToggleMute(true, "bg");
        }
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound()
    {
        isSound = !isSound;
        if (isSound)
        {
            Sound_button.gameObject.SetActive(true);
            SoundMute_button.gameObject.SetActive(false);
            if (audioController) audioController.ToggleMute(false, "button");
            if (audioController) audioController.ToggleMute(false, "wl");
            if (audioController) audioController.ToggleMute(false, "win");
            if (audioController) audioController.ToggleMute(false, "bet");

        }
        else
        {
            audioController.PlayButtonAudio();
            Sound_button.gameObject.SetActive(false);
            SoundMute_button.gameObject.SetActive(true);
            if (audioController) audioController.ToggleMute(true, "button");
            if (audioController) audioController.ToggleMute(true, "wl");
            if (audioController) audioController.ToggleMute(true, "win");
            if (audioController) audioController.ToggleMute(true, "bet");
        }
    }

    private void UpdateInfoUI()
    {
        for (int i = 0; i < InfoPages_Objects.Count; i++)
            InfoPages_Objects[i].SetActive(i == currentInfoPage);

        for (int i = 0; i < InfoActive_Objects.Count; i++)
            InfoActive_Objects[i].SetActive(i == currentInfoPage);

    }

    private void GoToPreviousInfoPage()
    {
        if (audioController) audioController.PlayButtonAudio();
        currentInfoPage--;
        if (currentInfoPage < 0)
            currentInfoPage = InfoPages_Objects.Count - 1;

        UpdateInfoUI();
    }

    private void GoToNextInfoPage()
    {
        if (audioController) audioController.PlayButtonAudio();
        currentInfoPage++;
        if (currentInfoPage >= InfoPages_Objects.Count)
            currentInfoPage = 0;

        UpdateInfoUI();
    }

    private void ToggleCoins()
    {
        if (isExpanded)
            RetractCoins();
        else
            ExpandCoins();
    }
    private void ExpandCoins()
    {
        if (audioController)
            audioController.PlayWLAudio("coinSelect");

        SetChipoption(false);
        Repeatpanel.SetActive(false);

        float spacing = 115f;
        Vector3 center = coinSelector.transform.localPosition;

        for (int i = 0; i < Coins.Count; i++)
        {
            var coin = Coins[i];

            coin.transform.DOKill(true); // stop old tween safely

            coin.gameObject.SetActive(true);

            // IMPORTANT: reset to center first
            coin.transform.localPosition = center;

            float offset = (i + 1) * spacing;
            Vector3 targetPos = center + new Vector3(-offset, 0, 0);

            coin.transform
                .DOLocalMove(targetPos, 0.3f)
                .SetEase(Ease.OutBack);
        }

        isExpanded = true;
    }

    // private void ExpandCoins()
    // {
    //     if (audioController) audioController.PlayWLAudio("coinSelect");
    //     SetChipoption(false);
    //     Repeatpanel.SetActive(false);
    //     float spacing = 90f; // distance between coins
    //     Vector3 center = coinSelector.transform.localPosition;

    //     for (int i = 0; i < Coins.Count; i++)
    //     {
    //         var coin = Coins[i];

    //         coin.gameObject.SetActive(true);

    //         // Each coin moves left by (i + 1) * spacing
    //         float offset = (i + 1) * spacing;

    //         Vector3 targetPos = center + new Vector3(-offset, 0, 0);

    //         coin.transform.DOLocalMove(targetPos, duration)
    //             .SetEase(Ease.OutBack);
    //     }

    //     isExpanded = true;
    // }

    internal void RetractCoins()
    {
        Vector3 center = coinSelector.transform.localPosition;
        if (audioController) audioController.PlayWLAudio("coinSelect");
        for (int i = 0; i < Coins.Count; i++)
        {
            var coin = Coins[i];

            coin.transform.DOLocalMove(center, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    coin.gameObject.SetActive(false);
                });
        }

        if (gameManager.currentTotalBet > 0)
        {
            Repeatpanel.SetActive(false);
            SetChipoption(true);

        }
        else
        {
            if (gameManager.isRepeatbetActive) Repeatpanel.SetActive(true);
        }
        isExpanded = false;
    }




    public void OnCoinSelected(Button selectedCoin)
    {
        SetChipoption(false);

        var tempImage = coinSelector.chipImage.sprite;
        coinSelector.chipImage.sprite = selectedCoin.image.sprite;
        selectedCoin.image.sprite = tempImage;

        TMP_Text selectorText = coinSelector.GetComponentInChildren<TMP_Text>();
        TMP_Text selectedText = selectedCoin.GetComponentInChildren<TMP_Text>();

        string tempText = selectorText.text;
        selectorText.text = selectedText.text;
        selectedText.text = tempText;

        Chip selectorChip = coinSelector.GetComponent<Chip>();
        Chip selectedChip = selectedCoin.GetComponent<Chip>();

        int tempIndex = selectorChip.chipIndex;
        selectorChip.chipIndex = selectedChip.chipIndex;
        selectedChip.chipIndex = tempIndex;
        // Debug.Log("mmmmmmmmmmmmmmmmm" + selectorChip.chipIndex);
        RetractCoins();
        SortCoinsByIndex();
        SetgameRulePanel();

    }
    private void SortCoinsByIndex()
    {
        // Sort list based on chipIndex
        Coins = Coins
            .OrderBy(c => c.GetComponent<Chip>().chipIndex)
            .ToList();

        // Update hierarchy order (important for UI rendering order)
        for (int i = 0; i < Coins.Count; i++)
        {
            Coins[i].transform.SetSiblingIndex(i);
        }
    }
    #endregion




    #region Stats Panel





    internal void UpdateStats(string cardNo, bool isBlue, string countNo)
    {
        UpdateLineStats(cardNo, isBlue);
        UpdateGridStats(cardNo, isBlue, countNo);
    }

    private void UpdateLineStats(string cardNo, bool isBlue)
    {
        int activeCount = 0;

        for (int i = 0; i < LineStats.Count; i++)
            if (LineStats[i].gameObject.activeSelf)
                activeCount++;

        if (activeCount < LineStats.Count)
        {
            activeCount++;
            LineStats[activeCount - 1].gameObject.SetActive(true);
        }

        for (int i = activeCount - 1; i > 0; i--)
            LineStats[i].CopyFrom(LineStats[i - 1]);

        LineStats[0].SetData(cardNo, isBlue, true);

        for (int i = 1; i < activeCount; i++)
            LineStats[i].HighBg.SetActive(false);
    }

    internal void InitializeStatsFromServer(List<string> statsList)
    {
        if (statsList == null || statsList.Count == 0)
            return;

        foreach (string statString in statsList)
        {
            StatData data = JsonUtility.FromJson<StatData>(statString);

            if (data == null || data.middleCard == null)
                continue;

            string cardNo = data.middleCard.rank;     // "10", "A", "K"
            bool isBlue = data.matchSide == "bahar";  // bahar = blue
            string countNo = data.cardsDealt.ToString();

            UpdateStats(cardNo, isBlue, countNo);
        }
    }

    private void UpdateGridStats(string cardNo, bool isBlue, string countNo)
    {
        if (gridStats.Count >= MAX_GRID)
        {
            Destroy(gridStats[0].gameObject);
            gridStats.RemoveAt(0);
        }

        StatsPrefab stat = Instantiate(StatsPref, StatsParent).GetComponent<StatsPrefab>();
        stat.SetData(cardNo, isBlue, true, countNo);

        if (gridStats.Count > 0)
        {
            StatsPrefab last = gridStats[gridStats.Count - 1];
            last.SetData(last.cardnumber.text,
                         last.BlueBg.activeSelf,
                         false,
                         last.countNumber.text);
        }

        gridStats.Add(stat);
    }



    internal void CalculateAndShowPercentage()
    {
        var stats = GetStats();
        int total = stats.Count;

        //  Debug.Log($"[Percentage] Total Stats = {total}");
        if (total == 0) return;

        int andarCount = 0;
        int baharCount = 0;

        foreach (var s in stats)
        {
            //  Debug.Log($"[Percentage] Card={s.cardnumber.text}, winner={s.winner}");

            if (s.winner == "andar")
                andarCount++;

            else if (s.winner == "bahar")
                baharCount++;
        }

        //  Debug.Log($"[Percentage] Andar={andarCount}, Bahar={baharCount}");

        float andarPercent = (andarCount * 100f) / total;
        float baharPercent = (baharCount * 100f) / total;

        AndarPercentage.text = andarPercent.ToString("0") + "%";
        BaharPercentage.text = baharPercent.ToString("0") + "%";

        FillAb.fillAmount = andarPercent / 100f;
    }




    internal void CalculateStringProbability(string card)
    {
        //  Debug.Log($"[Probability] Checking for card = {card}");
        cardProb.text = card;

        var stats = GetStats();
        int total = stats.Count;

        if (total == 0)
        {
            //    Debug.Log("[Probability] No stats found.");
            return;
        }

        int andarMatch = 0;
        int baharMatch = 0;

        foreach (var s in stats)
        {
            //  Debug.Log($"[Probability] Card={s.cardnumber.text}, Winner={s.winner}");

            if (s.cardnumber.text == card)
            {
                //    Debug.Log("[Probability] -> Card matched!");

                if (s.winner == "andar")
                    andarMatch++;

                else if (s.winner == "bahar")
                    baharMatch++;
            }
        }

        //   Debug.Log($"[Probability] Matches: Andar={andarMatch}, Bahar={baharMatch}");

        float andarProbVal = (andarMatch * 100f) / total;
        float baharProbVal = (baharMatch * 100f) / total;

        AndarProb.text = andarProbVal.ToString("0") + "%";
        BaharProbab.text = baharProbVal.ToString("0") + "%";

        //  Fillprobability.fillAmount = 1 - ((andarProbVal + baharProbVal) / 100f);
    }


    private List<StatsPrefab> GetStats()
    {
        List<StatsPrefab> list = new List<StatsPrefab>();

        for (int i = 0; i < StatsParent.childCount; i++)
        {
            var s = StatsParent.GetChild(i).GetComponent<StatsPrefab>();
            if (s != null && s.gameObject.activeSelf)
                list.Add(s);
        }

        return list;
    }

    internal void setCoins(bool istrue)
    {
        chipPanel.SetActive(istrue);
    }
    internal void SetNetBetPanel(bool istrue, string totalbet = "-1")
    {
        if (totalbet == "0") return;
        if (totalbet != "-1") NetBet.text = totalbet;
        else NetBet.text = "0";

        NetBetPanel.SetActive(istrue);
        if (istrue) SetChipoption(false);

        RectTransform rect = NetBetPanel.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);


    }
    internal void SetChipoption(bool istrue, bool db = true, bool canc = true, bool undo = true)
    {
        chiOptionpanel.SetActive(istrue);
        Doublebtn.gameObject.SetActive(db);
        Canclebtn.gameObject.SetActive(canc);
        Undubtn.gameObject.SetActive(undo);
    }
    #endregion


    #region History Setup

    internal void SetHistoryPage(Payload payload)
    {
        CurrentHistoryPage = payload.meta.page;
        MaxHistoryPage = payload.meta.pages;
        HistoryNav.text = payload.meta.page.ToString() + "/" + payload.meta.pages.ToString();
        // 1. Remove old items
        foreach (Transform child in Pageparent.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. Spawn new items
        for (int i = 0; i < payload.history.Count; i++)
        {
            GameObject obj = Instantiate(HistoryPrefab, Pageparent.transform);
            HistoryPrefab script = obj.GetComponent<HistoryPrefab>();
            payload.history[i].middleCardParsed = JsonUtility.FromJson<Card>(payload.history[i].middle_card);
            payload.history[i].matchingCardParsed = JsonUtility.FromJson<Card>(payload.history[i].matching_card);



            Sprite middleCard = gameManager.CardSet(payload.history[i].middleCardParsed.suit, payload.history[i].middleCardParsed.rank);
            Sprite sideCard = gameManager.CardSet(payload.history[i].matchingCardParsed.suit, payload.history[i].matchingCardParsed.rank);
            script.SetData(i + 1, payload.history[i], middleCard, sideCard, payload.history[i].cards_dealt);
        }
    }



    #endregion
    #region Intro Page Setup
    void OnClickDontShow()
    {
        int show = PlayerPrefs.GetInt("CanShow");
        if (show == 0)
        {
            show = 1;
            tick.SetActive(true);
        }
        else
        {
            show = 0;
            tick.SetActive(false);

        }
        PlayerPrefs.SetInt("CanShow", show);
    }
    void ShowIntroPage()
    {
        int show = PlayerPrefs.GetInt("CanShow");
        if (show == 0)
        {
            OpenPopup(Intropage);
        }

    }

    #endregion


    void SetQuitPopupAnimation(bool isShowing)
    {
        Debug.Log("_________________OOOOOOOOOOOOO");
        QuitpopupAnim.StopAnimation();
        QuitpopupAnim.textureArray.Clear();
        QuitpopupAnim.textureArray.TrimExcess();
        if (isShowing)
        {
            for (int i = 0; i < QuitStartSprits.Count; i++)
            {
                QuitpopupAnim.textureArray.Add(QuitStartSprits[i]);
            }
        }
        else
        {
            for (int i = 0; i < QuitEndSprits.Count; i++)
            {
                QuitpopupAnim.textureArray.Add(QuitEndSprits[i]);
            }
        }
        QuitpopupAnim.StartAnimation();
        FadeQuitButtons(isShowing);
    }
    void FadeQuitButtons(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float duration = 0.1f;

        // YES button
        YesQuit_Button.image.DOKill();
        YesQuit_Button.image.DOFade(targetAlpha, duration);

        // NO button
        NoQuit_Button.image.DOKill();
        NoQuit_Button.image.DOFade(targetAlpha, 0.1f);

        YesQuit_Button.interactable = fadeIn;
        NoQuit_Button.interactable = fadeIn;
    }
    IEnumerator OnCliqQuitBtn(bool yesBtnClicked)
    {

        if (yesBtnClicked)
        {
            YesBtnParticles.StartAnimation();

        }
        else
        {
            NoBtnParticles.StartAnimation();
            // ClosePopup(QuitPopup_Object);
        }
        YesQuit_Button.interactable = false;
        NoQuit_Button.interactable = false;
        yield return new WaitForSeconds(1f);
        SetQuitPopupAnimation(false);
        yield return new WaitForSeconds(1f);
        QuitpopupAnim.StopAnimation();

        if (yesBtnClicked)
        {
            gameManager.BonusObject.gameObject.SetActive(false);
            YesBtnParticles.StopAnimation();
            if (!homepopup)
            {
                CallOnExitFunction();
                Debug.Log("quit event: pressed YES Button ");
                socketManager.ReactNativeCallOnFailedToConnect();
            }
            else
            {
                gameManager.directJump = false;
                RetractCoins();
                ClosePopup(QuitPopup_Object);
                IsMenuPanelOpen = false; socketManager.SendHome(); ResetMenuPanel(false);
                gameManager.isRepeatbetActive = false;
            }


        }
        else
        {
            NoBtnParticles.StopAnimation();
            ClosePopup(QuitPopup_Object);
        }


    }

    private void PlayButtonAnimation(Button button, Action onComplete)
    {
        RectTransform rect = button.GetComponent<RectTransform>();

        float startScale = 1.2f;
        float endScale = 1f;
        float duration = 0.15f;

        rect.localScale = Vector3.one * startScale;

        rect.DOScale(endScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    void OnOpenBetLimit()
    {
        ChangeButtonLimitData();

        string room = gameManager.currentRoom;
        Button button = null;


        switch (room)
        {
            case "casual":
                button = betBtnQ;
                break;

            case "novice":
                button = betBtnW;
                break;

            case "expert":
                button = betBtnE;
                break;

            case "high_roller":
                button = betBtnR;
                break;
        }
        OnChangeLimitClicked(button, gameManager.currentRoom);
        OpenPopup(BetLimitPanel);
    }
    private void OnChangeLimitClicked(Button btnindex, string room)
    {
        gameManager.nextRoom = room;
        betBtnQ.image.sprite = NonSelectedBtn;
        betBtnW.image.sprite = NonSelectedBtn;
        betBtnE.image.sprite = NonSelectedBtn;
        betBtnR.image.sprite = NonSelectedBtn;
        btnindex.image.sprite = SelectedBtn;

        SetButtonTextColor(betBtnQ, Color.white);
        SetButtonTextColor(betBtnW, Color.white);
        SetButtonTextColor(betBtnE, Color.white);
        SetButtonTextColor(betBtnR, Color.white);

        string fullText = btnindex.GetComponentInChildren<TMP_Text>().text;
        SetButtonTextColor(btnindex, Color.yellow);

        string minBet = fullText.Split('-')[0];

        ChangeLimitData(minBet, room);
        if (gameManager.currentRoom == room) Disclamer.gameObject.SetActive(false);
        else Disclamer.gameObject.SetActive(true);

    }
    void ChangeButtonLimitData()
    {
        betBtnQ.GetComponentInChildren<TMP_Text>().text = socketManager.initialData.levelBetLimit.casual.min_bet_limit.ToString() + "-" + socketManager.initialData.levelBetLimit.casual.max_bet_limit.ToString();
        betBtnW.GetComponentInChildren<TMP_Text>().text = socketManager.initialData.levelBetLimit.novice.min_bet_limit.ToString() + "-" + socketManager.initialData.levelBetLimit.novice.max_bet_limit.ToString();
        betBtnE.GetComponentInChildren<TMP_Text>().text = socketManager.initialData.levelBetLimit.expert.min_bet_limit.ToString() + "-" + socketManager.initialData.levelBetLimit.expert.max_bet_limit.ToString();
        betBtnR.GetComponentInChildren<TMP_Text>().text = socketManager.initialData.levelBetLimit.high_roller.min_bet_limit.ToString() + "-" + socketManager.initialData.levelBetLimit.high_roller.max_bet_limit.ToString();
    }
    void ChangeLimitData(string minBet, string room)
    {
        List<string> data = gameManager.GetAllMaxLimits(room);
        for (int i = 0; i < BetLimitdata.Count; i++)
        {
            BetLimitdata[i].text = minBet + " - " + data[i];
        }

    }
    private void SetButtonTextColor(Button button, Color color)
    {
        TMP_Text txt = button.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.color = color;
    }


    #region Expand / Shrink

    private void InitializeExpandShrink()
    {

        SetExpandShrinkButtons(isExpanded: false);
    }

    private void OnExpand()
    {
        isExpanded = true;
        jsFunctCalls?.RequestExpandGame();
        SetExpandShrinkButtons(isExpanded: true);
    }

    private void OnShrink()
    {
        isExpanded = false;
        jsFunctCalls?.RequestShrinkGame();
        SetExpandShrinkButtons(isExpanded: false);
    }


    private void SetExpandShrinkButtons(bool isExpanded)
    {
        if (ExpandHome_Button) ExpandHome_Button.gameObject.SetActive(!isExpanded);
        if (ShrinkHome_Button) ShrinkHome_Button.gameObject.SetActive(isExpanded);
        if (!isExpanded) Expand_Button_Text.text = "Expand";
        else Expand_Button_Text.text = "Shrink";
        // if (ExpandMenu_Button) ExpandMenu_Button.gameObject.SetActive(!isExpanded);
        // if (ShrinkMenu_Button) ShrinkMenu_Button.gameObject.SetActive(isExpanded);
        // if (ExpandSideMenu_Button)
        // {
        //     RectTransform rect = ExpandSideMenu_Button.GetComponent<RectTransform>();
        //     if (rect != null) rect.anchoredPosition = expandSideMenuOriginalPosition;
        //     ExpandSideMenu_Button.gameObject.SetActive(!isExpanded);
        //     ExpandSideMenu_Button.interactable = !isExpanded;
        // }
        // if (ShrinkSideMenu_Button)
        // {
        //     RectTransform rect = ShrinkSideMenu_Button.GetComponent<RectTransform>();
        //     if (rect != null) rect.anchoredPosition = shrinkSideMenuOriginalPosition;
        //     ShrinkSideMenu_Button.gameObject.SetActive(isExpanded);
        //     ShrinkSideMenu_Button.interactable = isExpanded;
        // }
    }

    private void RegisterFullscreenListener()
    {
        jsFunctCalls?.RegisterFullscreenListener(gameObject.name);
    }

    internal void OnFullscreenChanged(string isFullscreen)
    {
        bool newExpandedState = isFullscreen == "1";
        Debug.Log($"[UI] OnFullscreenChanged callback: isFullscreen={isFullscreen}, newState={newExpandedState}");

        // Only update if state actually changed
        if (isExpanded != newExpandedState)
        {
            isExpanded = newExpandedState;
            SetExpandShrinkButtons(isExpanded);
            Debug.Log($"[UI] Button states synced to fullscreen: {(isExpanded ? "EXPANDED" : "SHRINK")}");
        }
    }
    #endregion
}