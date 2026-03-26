using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using System.Runtime.Serialization;
using Best.HTTP.Shared;

public class SocketIOManager : MonoBehaviour
{
    [SerializeField]
    internal GameManager gameManager;

    [SerializeField]
    private UiManager uiManager;
    [SerializeField] private LoadingScreenManager loadingScreenManager;

    internal Root roomData;
    internal Root gameLoopData;
    internal Root gameCashOut;
    internal Root BetChipData;
    internal Root OtherChipData;
    internal Root CashoutData;
    internal Root BonusData;
    internal Root doubleBetData;
    internal Root CancleBetData;
    internal Root HistoryPageData;
    internal Root ReturnHome;
    internal Root TotalPlayerCountData;
    internal Root TimeRemaining;
    internal Root CardDelt;
    internal Root FlushData;
    internal GameData initialData = null;
    // internal Payload resultData = null;
    internal Player playerdata = null;
    [SerializeField]
    // internal List<string> bonusdata = null;
    internal List<double> MultiplierList;
    //WebSocket currentSocket = null;
    internal bool isResultdone = false;
    // protected string nameSpace="game"; //BackendChanges
    protected string nameSpace = "playground-multiplayer"; //BackendChanges
    private Socket gameSocket; //BackendChanges

    private SocketManager manager;

    protected string SocketURI = null;
    protected string TestSocketURI = "https://devrealtime.dingdinghouse.com/";
    // protected string TestSocketURI = "http://localhost:5000/";
    private string savedToken;

    [SerializeField] internal JSFunctCalls JSManager;
    [SerializeField]
    private string testToken;
    protected string gameID = "ml-ab";
    //protected string gameID = "";

    internal bool isLoaded = false;

    internal bool SetInit = false;

    private const int maxReconnectionAttempts = 6;
    private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);
    private bool isConnected = false; //Back2 Start
    private bool hasEverConnected = false;
    private const int MaxReconnectAttempts = 5;
    private const float ReconnectDelaySeconds = 2f;

    private float lastPongTime = 0f;
    private float pingInterval = 2f;
    private float pongTimeout = 3f;
    private bool waitingForPong = false;
    private int missedPongs = 0;
    private const int MaxMissedPongs = 15;
    internal bool loadingPageLoading = false;
    internal bool NormalStart = false;
    internal bool DontDisplayDisconected = false;
    private Coroutine PingRoutine; //Back2 end
    [SerializeField] private GameObject RaycastBlocker;

    private void Awake()
    {
        Application.runInBackground = true;
        isLoaded = false;
        SetInit = false;

    }

    private void Start()
    {
        //OpenWebsocket();
        OpenSocket();
    }
    void CloseGame()
    {
        StartCoroutine(CloseSocket());
    }

    void ReceiveAuthToken(string jsonData)
    {

        // Parse the JSON data
        var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
        SocketURI = data.socketURL;
        myAuth = data.cookie;
        //  nameSpace = data.nameSpace;
        // Proceed with connecting to the server using myAuth and socketURL
    }

    string myAuth = null;

    private void OpenSocket()
    {
        //Create and setup SocketOptions
        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;
        options.Reconnection = false;
        options.Timeout = TimeSpan.FromSeconds(3);
        options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket; //BackendChanges
        RaycastBlocker.SetActive(true);
        loadingScreenManager.ShowLoading(LoadingScreenManager.LoadingType.InitWaiting, null);
        //   Application.ExternalCall("window.parent.postMessage", "authToken", "*");

#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = testToken,
                // gameId = gameID
            };
        };
        options.Auth = authFunction;
        savedToken = testToken;
        // Proceed with connecting to the server
        SetupSocketManager(options);
#endif
    }

    private IEnumerator WaitForAuthToken(SocketOptions options)
    {
        // Wait until myAuth is not null
        while (myAuth == null)
        {
            yield return null;
        }
        while (SocketURI == null)
        {
            yield return null;
        }
        // Once myAuth is set, configure the authFunction
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = myAuth,
                // gameId = gameID
            };
        };
        options.Auth = authFunction;
        savedToken = myAuth;

        // Proceed with connecting to the server
        SetupSocketManager(options);
        yield return null;
    }

    private void SetupSocketManager(SocketOptions options)
    {
        // Create and setup SocketManager
#if UNITY_EDITOR
        this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
        this.manager = new SocketManager(new Uri(SocketURI), options);
#endif
        if (string.IsNullOrEmpty(nameSpace))
        {  //BackendChanges Start
            gameSocket = this.manager.Socket;
        }
        else
        {
            print("nameSpace: " + nameSpace);
            gameSocket = this.manager.GetSocket("/" + nameSpace);
        }
        // Set subscriptions
        gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
        gameSocket.On<Error>(SocketIOEventTypes.Error, OnError);
        //gameSocket.On<string>("message", OnListenEvent);
        gameSocket.On<string>("game:init", ManageInitData);
        gameSocket.On<string>("game:bet_placed", ManageOtherPlayerbets);
        gameSocket.On<string>("game:round_start", OnGameLoopStarted);
        gameSocket.On<string>("game:cashout_timer", OnCashoutTimer);
        gameSocket.On<string>("game:round_end", OnGameLoopEnd);
        gameSocket.On<string>("game:bonus", OnGameBonus);
        gameSocket.On<string>("game:cashout", OnCashout);
        gameSocket.On<string>("game:lobby_count", OnLobbyCount);
        gameSocket.On<string>("game:leaderboard_update", OnLeaderboardUpdate);
        gameSocket.On<string>("game:betting_timer", OnListenTimeEvent);
        gameSocket.On<string>("game:flush_result", OnListenFlush);
        gameSocket.On<string>("game:card_dealt", OnListenCardEvent);
        gameSocket.On<bool>("socketState", OnSocketState);
        gameSocket.On<string>("internalError", OnSocketError);
        gameSocket.On<string>("alert", OnSocketAlert);
        gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice);
        gameSocket.On<string>("pong", OnPongReceived);
        manager.Open();
    }

    // Connected event handler implementation
    void OnConnected(ConnectResponse resp) //Back2 Start
    {
        Debug.Log("[CONNECT] Socket connected to server.");

        if (hasEverConnected)
        {
            uiManager.CheckAndClosePopups();
        }

        isConnected = true;
        hasEverConnected = true;
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        SendPing();
    } //Back2 end

    private void OnPongReceived(string data) //Back2 Start
    {
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;

        if (loadingScreenManager.IsLoading())
        {
            loadingScreenManager.HideLoading();
        }
    } //Back2 end

    private void OnDisconnected() //Back2 Start
    {
        Debug.LogWarning("[DISCONNECT] Socket disconnected from server.");
        isConnected = false;
        uiManager.DisconnectionPopup();
        RaycastBlocker.SetActive(true);
        ResetPingRoutine();
    } //Back2 end
    private void OnError(Error err)
    {
        Debug.LogError("[ERROR] Socket error: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
    }
    private void OnListenTimeEvent(string data)
    {
        Debug.Log("[BROADCAST] game:betting_timer : " + data);

        // Always store the latest timer data so it's ready when transition completes
        TimeRemaining = JsonUtility.FromJson<Root>(data);

        // If loading screen is still up, we're mid-transition — don't apply yet.
        // TransitionToGameScreen's onComplete will have already set state via ApplyMidRoundState.
        // The *next* betting_timer broadcast after loading hides will apply cleanly.
        if (loadingScreenManager.IsLoading())
            return;

        gameManager.OnGameLoaded();
        gameManager.SetBetTimer();
    }
    private void OnListenCardEvent(string data)
    {
        Debug.Log("[BROADCAST] game:card_dealt : " + data);
        gameManager.OnGameLoaded();
        //  ParseResponse(data);
        CardDelt = JsonUtility.FromJson<Root>(data);
        gameManager.ManageCardDelt(CardDelt);
    }
    private void OnListenFlush(string data)
    {
        FlushData = JsonUtility.FromJson<Root>(data);
        StartCoroutine(gameManager.ManageFlushAnimation());
        //  ParseResponse(data);
    }

    private void OnSocketState(bool state)
    {
        if (state)
        {
            Debug.Log("[BROADCAST] socketState : " + state);
        }
        else
        {

        }
    }
    private void OnSocketError(string data)
    {
        Debug.Log("[BROADCAST] internalError : " + data);
    }
    private void OnSocketAlert(string data)
    {
    }
    private bool isFocused = true;
    private Coroutine focusCheckCoroutine;
    private bool disconnectionShown = false;   // <- NEW

    void OnApplicationFocus(bool focus)
    {
        isFocused = focus;

        if (!focus)
        {
            // Start checking after losing focus
            if (focusCheckCoroutine == null && !disconnectionShown)
                focusCheckCoroutine = StartCoroutine(IsNotInFocus());
        }
        else
        {
            // If popup already shown, do NOT cancel anything
            if (disconnectionShown) return;

            // Otherwise cancel coroutine when focus returns
            if (focusCheckCoroutine != null)
            {
                StopCoroutine(focusCheckCoroutine);
                focusCheckCoroutine = null;
            }
        }
    }

    IEnumerator IsNotInFocus()
    {
        yield return new WaitForSeconds(120f); // 2 seconds, change as required

        // If still not focused AND popup not shown
        if (!isFocused && !disconnectionShown)
        {
            // disconnectionShown = true;  // Prevent future runs
            //  uiManager.DisconnectionPopup();
        }

        focusCheckCoroutine = null;
    }

    private void OnSocketOtherDevice(string data)
    {
        Debug.Log("[BROADCAST] AnotherDevice : " + data);
        uiManager.ADfunction();
    }

    private void SendPing() //Back2 Start
    {
        ResetPingRoutine();
        PingRoutine = StartCoroutine(PingCheck());
    }

    void ResetPingRoutine()
    {
        if (PingRoutine != null)
        {
            StopCoroutine(PingRoutine);
        }
        PingRoutine = null;
    }

    private IEnumerator PingCheck()
    {
        while (true)
        {
            if (missedPongs == 0)
            {
                uiManager.CheckAndClosePopups();
            }

            if (waitingForPong)
            {
                if (missedPongs == 2)
                {
                    // REPLACE: uiManager.ReconnectionPopup();
                    // WITH:
                    loadingScreenManager.ShowLoading(
                        LoadingScreenManager.LoadingType.Connecting,
                        null
                    );
                }
                missedPongs++;

                if (missedPongs >= MaxMissedPongs)
                {
                    isConnected = false;
                    loadingScreenManager.ForceHideLoading(); // ADD THIS LINE
                    uiManager.DisconnectionPopup();
                    yield break;
                }
            }

            waitingForPong = true;
            lastPongTime = Time.time;
            SendDataWithNamespace("ping");
            yield return new WaitForSeconds(pingInterval);
        }
    }

    private void AliveRequest()
    {
        SendDataWithNamespace("YES I AM ALIVE");
    }

    private void SendDataWithNamespace(string eventName, string json = null)
    {
        // Send the message
        if (gameSocket != null && gameSocket.IsOpen) //BackendChanges
        {
            if (json != null)
            {
                gameSocket.Emit(eventName, json);
                Debug.Log("[EMIT] " + eventName + " : " + json);
            }
            else
            {
                gameSocket.Emit(eventName);
            }
        }
        else
        {
            Debug.LogWarning("[EMIT] Failed — socket not connected.");
        }
    }

    internal void ReactNativeCallOnFailedToConnect() //BackendChanges
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("onExit");
#endif
    }

    internal IEnumerator CloseSocket() //Back2 Start
    {
        RaycastBlocker.SetActive(true);
        ResetPingRoutine();

        Debug.Log("[CONNECT] Closing socket...");

        manager?.Close();
        manager = null;



        yield return new WaitForSeconds(0.5f);

        Debug.Log("[CONNECT] Socket closed.");
        RaycastBlocker.SetActive(true);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
    }
    public void Reconnect()
    {
        Debug.Log("[CONNECT] Reconnecting with saved token...");

        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;
        options.Reconnection = false;
        options.Timeout = TimeSpan.FromSeconds(3);
        options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;

        // Use saved token here
        options.Auth = (manager, socket) =>
        {
            return new { token = savedToken };
        };
        SetupSocketManager(options);
        //         // Close old manager if any
        //         manager?.Close();

        // #if UNITY_EDITOR
        //         manager = new SocketManager(new Uri(TestSocketURI), options);
        // #else
        //     manager = new SocketManager(new Uri(SocketURI), options);
        // #endif

        //         // Get correct namespace
        //         if (string.IsNullOrEmpty(nameSpace))
        //             gameSocket = manager.Socket;
        //         else
        //             gameSocket = manager.GetSocket("/" + nameSpace);

        //         manager.Open();
        //         DontDisplayDisconected = false;
    }

    void ManageInitData(string jsonObject)
    {
        Root myData = null;
        try
        {
            myData = JsonConvert.DeserializeObject<Root>(jsonObject);
        }
        catch (Exception ex)
        {
            Debug.LogError("[BROADCAST] game:init parse error: " + ex.Message);
            return;
        }

        if (myData == null)
        {
            Debug.LogError("[BROADCAST] game:init : null payload received.");
            return;
        }
        Debug.Log("[BROADCAST] game:init : " + jsonObject);

        string id = myData.id;
        initialData = myData.gameData;
        playerdata = myData.player;

        setInitialData();
        // if (initialData.bets != null)
        // else
        RaycastBlocker.SetActive(false);
        loadingScreenManager.HideLoading();
#if UNITY_WEBGL && !UNITY_EDITOR
            JSManager.SendCustomMessage("OnEnter");
#endif
    }

    private void setInitialData()
    {
        isLoaded = true;
        gameManager.SetInitialData();
        RaycastBlocker.SetActive(false);
        Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
#if UNITY_WEBGL && !UNITY_EDITOR //BackendChanges
            Application.ExternalEval(@"
            if(window.ReactNativeWebView){
            window.ReactNativeWebView.postMessage('OnEnter');
            }
            ");
#endif
    }

    internal void SendRoomSelection(string Room)
    {
        Debug.Log("[EMIT] request : JOIN_LEVEL => " + Room);
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "JOIN_LEVEL";
        message.payload.level = Room;
        string json = JsonUtility.ToJson(message);

        loadingScreenManager.ShowLoading(
            LoadingScreenManager.LoadingType.JoiningTable,
            () => gameSocket.ExpectAcknowledgement<string>(OnRoomEnter).Emit("request", json)
        );
    }
    internal void BetPlaced(int amountIndex, string betType, string betOption)
    {
        double chipValue;

        if (double.TryParse(uiManager.coinSelector.Chiptext.text, out chipValue))
        {
            if (chipValue > playerdata.balance)
            {
                gameManager.PlayPopup("Low Balance");
                // Low balance logic here

                return;
            }
        }
        BetMessage message = new BetMessage();
        message.type = "PLACE_BET";
        message.payload = new BetPayload();

        message.payload.amountIndex = amountIndex;
        message.payload.betType = betType;
        message.payload.betOption = betOption;

        string json = JsonUtility.ToJson(message);
        Debug.Log("[EMIT] request : PLACE_BET => " + json);

        gameSocket.ExpectAcknowledgement<string>(OnBetAcknowledged).Emit("request", json);
    }
    internal void SendUndo()
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "UNDO_BET";
        // message.payload.level = Room;

        string json = JsonUtility.ToJson(message);

        SendDataWithNamespace("Undo Send", json);
        gameSocket.ExpectAcknowledgement<string>(OnUndo).Emit("request", json);
    }
    internal void SendRepeat()
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "REPEAT_BET";
        // message.payload.level = Room;

        string json = JsonUtility.ToJson(message);

        //  SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnRepeat).Emit("request", json);
    }
    internal void SendCancle()
    {

        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "CANCEL_BET";
        // message.payload.level = Room;

        string json = JsonUtility.ToJson(message);

        //  SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnCancle).Emit("request", json);
    }
    internal void SendDouble()
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "DOUBLE_BET";
        // message.payload.level = Room;

        string json = JsonUtility.ToJson(message);

        // SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnDouble).Emit("request", json);
    }
    internal void SendHome()
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "HOME";
        loadingPageLoading = false;
        NormalStart = false;
        DontDisplayDisconected = true;
        string json = JsonUtility.ToJson(message);
        Debug.Log("[EMIT] request : HOME => " + json);

        loadingScreenManager.ShowLoading(
            LoadingScreenManager.LoadingType.LeavingTable,
            () => gameSocket.ExpectAcknowledgement<string>(OnHome).Emit("request", json)
        );
    }

    internal void SendHistory(int Pages)
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "BET_HISTORY";
        message.payload.page = Pages;
        string json = JsonUtility.ToJson(message);
        Debug.Log("[EMIT] request : BET_HISTORY => " + json);

        loadingScreenManager.ShowLoading(
            LoadingScreenManager.LoadingType.LoadingHistory,
            () => gameSocket.ExpectAcknowledgement<string>(OnHistory).Emit("request", json)
        );
    }
    void OnHome(string json)
    {
        gameManager.ClearAllBets();
        Debug.Log("[ACK] HOME : " + json);
        ReturnHome = JsonUtility.FromJson<Root>(json);
        playerdata.balance = ReturnHome.payload.balance;

        if (gameManager.directJump)
        {
            // Level-change flow: set currentRoom NOW so OnRoomEnter uses the correct room,
            // keep the loading screen visible the whole time, skip showing home screen entirely
            gameManager.directJump = false;
            gameManager.currentRoom = gameManager.nextRoom;
            StartCoroutine(JoinNextRoomDirectly());
        }
        else
        {
            // Normal home flow: show home screen as usual
            loadingScreenManager.HideLoading();
            gameManager.GamePage.SetActive(false);
            gameManager.HomePage.SetActive(true);
            uiManager.MenuMain_button.gameObject.SetActive(true);
            uiManager.MenuInGame_button.gameObject.SetActive(false);
            uiManager.sideMenuePanel.transform.position = new Vector3(
                uiManager.sideMenuePanel.transform.position.x, 451f,
                uiManager.sideMenuePanel.transform.position.z);
            gameManager.SetPlayerData(playerdata);
        }
    }

    IEnumerator JoinNextRoomDirectly()
    {
        // Keep loading screen up, wait one frame for HOME cleanup to settle, then join new room
        yield return null;
        SendRoomSelection(gameManager.nextRoom);
    }

    void OnHistory(string json)
    {
        Debug.Log("[ACK] BET_HISTORY : " + json);
        loadingScreenManager.HideLoading();
        HistoryPageData = JsonUtility.FromJson<Root>(json);
        uiManager.SetHistoryPage(HistoryPageData.payload);
    }

    void OnDouble(string json)
    {
        Debug.Log("[ACK] DOUBLE_BET : " + json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        if (doubleBetData.success)
        {
            gameManager.DoubleBets(doubleBetData.payload.bets);
            gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
            playerdata.balance = doubleBetData.payload.balance;
            gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        }
        else
        {
            gameManager.PlayPopup(doubleBetData.payload.message);
        }
    }
    void OnRepeat(string json)
    {
        Debug.Log("[ACK] REPEAT_BET : " + json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        if (doubleBetData.success)
        {
            gameManager.RepeAtBet(doubleBetData.payload.bets);
            gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
            playerdata.balance = doubleBetData.payload.balance;
            gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        }
        else
        {
            gameManager.PlayPopup(doubleBetData.payload.message);
        }
    }
    void OnCancle(string json)
    {
        Debug.Log("[ACK] CANCEL_BET : " + json);
        CancleBetData = JsonUtility.FromJson<Root>(json);
        if (CancleBetData.success == true)
        {
            StartCoroutine(gameManager.CancleBets());
            gameManager.UpdatePlayerbalance(CancleBetData.payload.balance.ToString());
            playerdata.balance = CancleBetData.payload.balance;
            gameManager.currentTotalBet = 0;
            uiManager.SetChipoption(false);
        }
    }
    void OnUndo(string json)
    {
        Debug.Log("[ACK] UNDO_BET : " + json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        // StartCoroutine(gameManager.UnduBets(doubleBetData.payload.bet.betId));
        gameManager.UndoBetsFast(doubleBetData.payload.bet.betId);
        gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
        playerdata.balance = doubleBetData.payload.balance;
        gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        if (doubleBetData.payload.totalBet == 0) uiManager.SetChipoption(false);
    }
    void OnRoomEnter(string json)
    {
        Debug.Log("[ACK] JOIN_LEVEL : " + json);

        roomData = JsonUtility.FromJson<Root>(json);
        if (roomData.success == false)
        {
            gameManager.HomePage.SetActive(true);
            gameManager.GamePage.SetActive(false);
            loadingScreenManager.HideLoading();
            return;

        }
        gameManager.SetCoinData();
        gameManager.SetBetLimit(gameManager.currentRoom);
        gameManager.SetOptionData();
        gameManager.UpdateGamePlayerCount(roomData.payload.playerCount);
        gameManager.SetOtherplayerData(roomData.payload.leaderboards);
        uiManager.MenuMain_button.gameObject.SetActive(false);
        uiManager.MenuInGame_button.gameObject.SetActive(true);
        uiManager.sideMenuePanel.transform.position = new Vector3(uiManager.sideMenuePanel.transform.position.x, 271f, uiManager.sideMenuePanel.transform.position.z);
        uiManager.Rayid.text = "R.ID: " + roomData.payload.roomId;
        uiManager.InitializeStatsFromServer(roomData.payload.stats);


        // ✅ FIX: Keep loading screen visible until game screen transition is complete
        // This prevents chips from appearing on home screen during mid-round join
        gameManager.TransitionToGameScreen(
            roomData.payload.roundState,
            roomData.payload.bets,
            () => loadingScreenManager.HideLoading()  // Hide loading only after transition complete
        );
    }

    void ManageOtherPlayerbets(string data)
    {
        Debug.Log("[BROADCAST] game:bet_placed : " + data);
        OtherChipData = JsonUtility.FromJson<Root>(data);
        gameManager.ManageBrodcastBetsOtherPlayers(OtherChipData);

    }
    void OnCashoutTimer(string json)
    {
        Debug.Log("[BROADCAST] game:cashout_timer : " + json);
        TimeRemaining = JsonUtility.FromJson<Root>(json);
        gameManager.SetNewRoundTimer(TimeRemaining.timeRemaining);
    }
    void OnGameLoopStarted(string json)
    {
        if (!loadingPageLoading)
        {
            loadingPageLoading = true;
            gameManager.OnGameLoaded();
        }
        NormalStart = true;
        Debug.Log("[BROADCAST] game:round_start : " + json);
        gameLoopData = JsonUtility.FromJson<Root>(json);
        gameManager.SetMainCard();
        gameManager.RestRoundText();

    }
    void OnGameLoopEnd(string data)
    {
        gameLoopData = JsonUtility.FromJson<Root>(data);

        Debug.Log("[BROADCAST] game:round_end : " + data);

        // if (!NormalStart)
        // {
        //     gameManager.GamePage.SetActive(true);
        //     gameManager.SetLoadingPage(false);
        //     gameManager.HomePage.SetActive(false);
        //     gameManager.StartGameMidway();
        // }

        gameManager.EndLoop();

        // Only start game AFTER validating
    }
    void OnCashout(string data)
    {
        Debug.Log("[BROADCAST] game:cashout : " + data);
        CashoutData = JsonUtility.FromJson<Root>(data);

        gameManager.ManagePayouts();

    }
    void OnGameBonus(string data)
    {
        Debug.Log("[BROADCAST] game:bonus : " + data);
        BonusData = JsonUtility.FromJson<Root>(data);

        gameManager.ManageBonus();

    }
    void OnLobbyCount(string data)
    {
        Debug.Log("[BROADCAST] game:lobby_count : " + data);

        TotalPlayerCountData = JsonUtility.FromJson<Root>(data);

        if (TotalPlayerCountData == null)
        {
            Debug.LogError("[BROADCAST] game:lobby_count parse error.");
            return;
        }

        if (TotalPlayerCountData.lobby != null)
        {

        }
        else
        {
            Debug.LogWarning("[BROADCAST] game:lobby_count : lobby object is null.");
        }

        if (gameManager != null)
        {

            gameManager.UpdateHomeScreenPlayerCount(TotalPlayerCountData.lobby, TotalPlayerCountData.totalCount);
        }
        else
        {
            Debug.LogError("[BROADCAST] game:lobby_count : GameManager reference is null.");
        }
    }
    void OnLeaderboardUpdate(string data)
    {
        Debug.Log("[BROADCAST] game:leaderboard_update : " + data);

        Root leaderboardData = JsonUtility.FromJson<Root>(data);

        if (leaderboardData == null)
        {
            Debug.LogError("[BROADCAST] game:leaderboard_update parse error.");
            return;
        }



        if (leaderboardData.leaderboards != null)
        {

        }
        else
        {
            Debug.LogWarning("[BROADCAST] game:leaderboard_update : leaderboards object is null.");
        }

        if (gameManager != null)
        {

            gameManager.UpdateGamePlayerCount(leaderboardData.playerCount);


            gameManager.SetOtherplayerData(leaderboardData.leaderboards);
        }
        else
        {
            Debug.LogError("[BROADCAST] game:leaderboard_update : GameManager reference is null.");
        }
    }
    private void OnBetAcknowledged(string data)
    {

        Debug.Log("[ACK] PLACE_BET : " + data);
        BetChipData = JsonUtility.FromJson<Root>(data);
        if (BetChipData.success)
        {
            gameManager.ManageBrodcastBetsPlayer();
            gameManager.UpdatePlayerbalance(BetChipData.payload.balance.ToString());
            gameManager.currentTotalBet = BetChipData.payload.totalBet;
            playerdata.balance = BetChipData.payload.balance;
        }
        else
        {
            gameManager.PlayPopup(BetChipData.payload.message);
        }

    }

}

[Serializable]
public class SendHistory
{
    public int page;
}

[Serializable]
public class SendRoom
{
    public string type;
    public Payload payload;
}

[Serializable]
public class RoundState
{
    public string roundId;
    public MiddleCard middleCard;
    public List<AndarCard> andarCards;
    public List<BaharCard> baharCards;
    public int cardsDealt;
    public string phase;
    public int timeRemaining;   // seconds left in betting window at join time
}

[Serializable]
public class Payload
{
    public string level;

    public string roomId;

    public int playerCount;
    public Leaderboards leaderboards;

    public string betId;
    public string betOption;
    public string message;
    public int amount;
    public int totalBet;

    public double balance;
    public List<Bet> bets;

    public int refundAmount;
    public Bet bet;

    public int page;

    public List<History> history;
    public Meta meta;

    public Lobby lobby;
    public List<string> stats;

    public RoundState roundState;
}
[System.Serializable]
public class Bet
{
    public int oldAmount;
    public int newAmount;
    public string betId;
    public string betType;
    public string betOption;
    public int delta;
    public int amount;
    public string username; // populated in JOIN_LEVEL ACK bets array
}

[System.Serializable]
public class BetPayload
{
    public int amountIndex;
    public string betType;
    public string betOption;
}

[System.Serializable]
public class BetMessage
{
    public string type;
    public BetPayload payload;
}
[System.Serializable]
public class Richest
{
    public string username;
    public double balance;
    public int rank;
}
[System.Serializable]
public class Winner
{
    public string username;
    public double totalWins;
    public int rank;
}
[System.Serializable]
public class Leaderboards
{
    public List<Richest> richest;
    public List<Winner> winners;
}
[System.Serializable]
public class S1115
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S15
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S1620
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S2125
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S2630
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S3135
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S3640
{
    public int payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S4153
{
    public int payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class S610
{
    public double payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class Andar
{
    public List<double> payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class Bahar
{
    public List<double> payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class Bets
{
    public List<int> casual;
    public List<int> novice;
    public List<int> expert;
    public List<int> high_roller;

    public string username;
    public int amount;
    public string betId;
    public string betType;
    public string level;
    public string userId;
    public string betOption;
}
[System.Serializable]
public class First1Andar
{
    public List<double> payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class First1Bahar
{
    public List<double> payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class First3
{
    public Payout payout;
    public MaxBetLimit max_bet_limit;
}
[System.Serializable]
public class GameData
{
    public List<string> betOptions;
    public int roundInterval;
    public int cashoutInterval;
    public int middleCardLimit;
    public int statsLimit;
    public Bets bets;
    public List<string> levels;
    public Wagers wagers;
    public Lobby lobby;
    public HandCode handCode;
    public Leaderboards leaderboards;
    public List<object> stats;
    public LevelBetLimit levelBetLimit;
}
[System.Serializable]
public class HandCode
{
    [JsonProperty("1")]
    public string _1;

    [JsonProperty("2")]
    public string _2;

    [JsonProperty("3")]
    public string _3;

    [JsonProperty("4")]
    public string _4;

    [JsonProperty("5")]
    public string _5;

    [JsonProperty("6")]
    public string _6;

    [JsonProperty("7")]
    public string _7;

    [JsonProperty("8")]
    public string _8;

    [JsonProperty("9")]
    public string _9;

    [JsonProperty("10")]
    public string _10;
    public int HIGH_CARD;
    public int PAIR;
    public int TWO_PAIR;
    public int THREE_OF_A_KIND;
    public int STRAIGHT;
    public int FLUSH;
    public int FULL_HOUSE;
    public int FOUR_OF_A_KIND;
    public int STRAIGHT_FLUSH;
    public int ROYAL_FLUSH;
}

[Serializable]
public class Lobby
{
    public int casual;
    public int novice;
    public int expert;
    public int high_roller;
}
[Serializable]
public class MainBets
{
    public Andar andar;
    public Bahar bahar;
}
[Serializable]
public class MaxBetLimit
{
    public int casual;
    public int novice;
    public int expert;
    public int high_roller;
}
[Serializable]
public class OpBets
{
    public First1Andar first_1_andar;
    public First1Bahar first_1_bahar;
    public First3 first_3;
}
[Serializable]
public class Payout
{
    public int straight_flush;
    public int straight;
    public int flush;

    public int win;
    public double balance;
    public string username;
    public string userId;
}

[Serializable]
public class Player
{
    public double balance;
    public string username;
}
[Serializable]
public class Root
{
    public string id;
    public GameData gameData;
    public Player player;

    public string roundId;
    public string matchSide;
    public MiddleCard middleCard;
    public List<AndarCard> andarCards;
    public List<BaharCard> baharCards;
    public int firstThreeResult;
    // public Leaderboards leaderboards;

    public bool success;
    public Payload payload;
    public int playerCount;

    public long startedAt;

    public string username;
    public string betId;
    public string betType;
    public string betOption;
    public int amount;
    public List<Payout> payouts;

    public Lobby lobby;

    //new

    public long serverTime;
    public long bettingEndTime;
    public int timeRemaining;

    public Card card;
    public string side;
    public int cardsDealt;

    public List<Card> cards;
    public Leaderboards leaderboards;

    public string bonus;
    public int totalCount;

}
[Serializable]
public class SideBets
{
    public S15 s_1_5;
    public S610 s_6_10;
    public S1115 s_11_15;
    public S1620 s_16_20;
    public S2125 s_21_25;
    public S2630 s_26_30;
    public S3135 s_31_35;
    public S3640 s_36_40;
    public S4153 s_41_53;
}
[Serializable]
public class Wagers
{
    public MainBets main_bets;
    public OpBets op_bets;
    public SideBets side_bets;
}

[Serializable]
public class AuthTokenData
{
    public string cookie;
    public string socketURL;
    public string nameSpace;
}
[Serializable]
public class AndarCard
{
    public string suit;
    public string rank;
    public string color;
}
[Serializable]
public class BaharCard
{
    public string suit;
    public string rank;
    public string color;
}
[Serializable]
public class MiddleCard
{
    public string suit;
    public string rank;
    public string color;
}
[Serializable]
public class History
{
    public string user_id;
    public int bet_amount;
    public double win_amount;
    public string bet_type;
    public string bet_option;
    public string round_id;
    public string bet_id;
    public string level;
    public string middle_card;
    public int cards_dealt;
    public string match_side;
    public string matching_card;

    // CHANGED: From DateTime to string to fix parsing issue
    public string created_at;  // Was: public DateTime created_at;

    // Parsed objects
    public Card middleCardParsed;
    public Card matchingCardParsed;

    // NEW: Helper property to get parsed DateTime
    public DateTime CreatedAtDateTime
    {
        get
        {
            if (string.IsNullOrEmpty(created_at))
                return DateTime.MinValue;

            // Try parsing ISO 8601 format
            if (DateTime.TryParse(created_at, out DateTime result))
                return result;

            return DateTime.MinValue;
        }
    }
}

[Serializable]
public class Meta
{
    public int total;
    public int page;
    public int limit;
    public int pages;
}
[System.Serializable]
public class Card
{
    public string color;
    public string suit;
    public string rank;
}
[System.Serializable]
public class StatData
{
    public MiddleCard middleCard;
    public string matchSide;
    public int cardsDealt;
}
[System.Serializable]
public class LevelBetLimit
{
    public Casual casual;
    public Novice novice;
    public Expert expert;
    public HighRoller high_roller;
}

[System.Serializable]
public class Casual
{
    public int min_bet_limit;
    public int max_bet_limit;
}

[System.Serializable]
public class Novice
{
    public int min_bet_limit;
    public int max_bet_limit;
}

[System.Serializable]
public class Expert
{
    public int min_bet_limit;
    public int max_bet_limit;
}

[System.Serializable]
public class HighRoller
{
    public int min_bet_limit;
    public int max_bet_limit;
}