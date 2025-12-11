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

    internal Root roomData;
    internal Root gameLoopData;
    internal Root gameCashOut;
    internal Root BetChipData;
    internal Root OtherChipData;
    internal Root CashoutData;
    internal Root doubleBetData;
    internal Root HistoryPageData;
    internal Root ReturnHome;
    internal Root TotalPlayerCountData;
    internal GameData initialData = null;
    // internal Payload resultData = null;
    internal Player playerdata = null;
    [SerializeField]
    internal List<string> bonusdata = null;
    internal List<double> MultiplierList;
    //WebSocket currentSocket = null;
    internal bool isResultdone = false;
    // protected string nameSpace="game"; //BackendChanges
    protected string nameSpace = "playground-multiplayer"; //BackendChanges
    private Socket gameSocket; //BackendChanges


    private SocketManager manager;


    protected string SocketURI = null;
    // protected string TestSocketURI = "https://game-crm-rtp-backend.onrender.com/";
    protected string TestSocketURI = "http://localhost:5000/";
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
    private const int MaxMissedPongs = 5;
    internal bool loadingPageLoading = false;
    internal bool NormalStart = false;
    internal bool DontDisplayDisconected = false;
    private Coroutine PingRoutine; //Back2 end
    [SerializeField] private GameObject RaycastBlocker;

    private void Awake()
    {
        Application.runInBackground = true;
        //Debug.unityLogger.logEnabled = false;
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
        Debug.Log("Unity: Closing Game");
        StartCoroutine(CloseSocket());
    }


    void ReceiveAuthToken(string jsonData)
    {
        Debug.Log("Received data: " + jsonData);

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
            Debug.Log("My Auth is null");
            yield return null;
        }
        while (SocketURI == null)
        {
            Debug.Log("My Socket is null");
            yield return null;
        }
        Debug.Log("My Auth is not null");
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
        Debug.Log("Auth function configured with token: " + myAuth);

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
        gameSocket.On<string>("game:round_end", OnGameLoopEnd);
        gameSocket.On<string>("game:cashout", OnCashout);
        gameSocket.On<string>("game:lobby_count", OnLobbyCount);
        gameSocket.On<string>("result", OnListenEvent);
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
        Debug.Log("✅ Connected to server.");

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
        Debug.Log("✅ Received pong from server.");
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
        Debug.Log($"📦 Pong payload: {data}");
    } //Back2 end

    private void OnDisconnected() //Back2 Start
    {
        Debug.LogWarning("⚠️ Disconnected from server.");
        isConnected = false;
        uiManager.DisconnectionPopup();
        ResetPingRoutine();
    } //Back2 end
    private void OnError(Error err)
    {
        Debug.LogError("Socket Error Message: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
    }
    private void OnListenEvent(string data)
    {
        // Debug.Log("Received some_event with data: " + data);
        ParseResponse(data);
    }

    private void OnSocketState(bool state)
    {
        if (state)
        {
            Debug.Log("my state is " + state);
        }
        else
        {

        }
    }
    private void OnSocketError(string data)
    {
        Debug.Log("Received error with data: " + data);
    }
    private void OnSocketAlert(string data)
    {
        //        Debug.Log("Received alert with data: " + data);
    }
    private bool isFocused = true;
    private Coroutine focusCheckCoroutine;

    void OnApplicationFocus(bool focus)
    {
        Debug.Log("Focus: " + focus);
        isFocused = focus;

        if (!focus)
        {
            // Start checking after losing focus
            if (focusCheckCoroutine == null)
                focusCheckCoroutine = StartCoroutine(IsNotInFocus());
        }
        else
        {
            // Cancel coroutine when focus returns
            if (focusCheckCoroutine != null)
            {
                StopCoroutine(focusCheckCoroutine);
                focusCheckCoroutine = null;
            }
        }
    }

    IEnumerator IsNotInFocus()
    {
        yield return new WaitForSeconds(120f);


        // If still not focused after 2 seconds
        if (!isFocused)
        {
            uiManager.DisconnectionPopup();
            Debug.Log("Disconnected: No Focus for 2 seconds");
        }

        focusCheckCoroutine = null;
    }
    private void OnSocketOtherDevice(string data)
    {
        Debug.Log("Received Device Error with data: " + data);
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
            Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

            if (missedPongs == 0)
            {
                uiManager.CheckAndClosePopups();
            }

            // If waiting for pong, and timeout passed
            if (waitingForPong)
            {
                if (missedPongs == 2)
                {
                    uiManager.ReconnectionPopup();
                }
                missedPongs++;
                Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

                if (missedPongs >= MaxMissedPongs)
                {
                    Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
                    isConnected = false;
                    uiManager.DisconnectionPopup();
                    yield break;
                }
            }

            // Send next ping
            waitingForPong = true;
            lastPongTime = Time.time;
            Debug.Log("📤 Sending ping...");
            SendDataWithNamespace("ping");
            yield return new WaitForSeconds(pingInterval);
        }
    } //Back2 end
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
                Debug.Log("JSON data sent: " + json);
            }
            else
            {
                gameSocket.Emit(eventName);
            }
        }
        else
        {
            Debug.LogWarning("Socket is not connected.");
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

        Debug.Log("Closing Socket");

        manager?.Close();
        manager = null;

        Debug.Log("Waiting for socket to close");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
    }
    public void Reconnect()
    {
        Debug.Log("Reconnecting using saved token...");

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
            Debug.LogError("Failed to deserialize JSON. Exception: " + ex.Message + "\nJSON: " + jsonObject);
            return;
        }

        if (myData == null)
        {
            Debug.LogError("ParseResponse: myData is null. JSON = " + jsonObject);
            return;
        }
        Debug.Log("ParseResponse: " + jsonObject);

        string id = myData.id;
        initialData = myData.gameData;
        playerdata = myData.player;

        setInitialData();
        // if (initialData.bets != null)
        // else
        //     Debug.LogWarning("initData: bets list is null.");

#if UNITY_WEBGL && !UNITY_EDITOR
            JSManager.SendCustomMessage("OnEnter");
#endif
    }

    private void ParseResponse(string jsonObject)
    {
        Debug.Log("ParseResponse JSON: " + jsonObject);

        Root myData = null;
        try
        {
            myData = JsonConvert.DeserializeObject<Root>(jsonObject);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to deserialize JSON. Exception: " + ex.Message + "\nJSON: " + jsonObject);
            return;
        }

        if (myData == null)
        {
            Debug.LogError("ParseResponse: myData is null. JSON = " + jsonObject);
            return;
        }

        string id = myData.id;

        switch (id)
        {
            case "initData":
                {
                    //  gameManager.uiManager.touchDisable.SetActive(false);

                    if (myData.gameData == null)
                    {
                        Debug.LogError("initData missing gameData. JSON = " + jsonObject);
                        return;
                    }

                    if (myData.player == null)
                    {
                        Debug.LogWarning("initData missing player data. JSON = " + jsonObject);
                    }

                    initialData = myData.gameData;
                    playerdata = myData.player;

                    setInitialData();
                    // if (initialData.bets != null)
                    // else
                    //     Debug.LogWarning("initData: bets list is null.");

#if UNITY_WEBGL && !UNITY_EDITOR
            JSManager.SendCustomMessage("OnEnter");
#endif

                    break;
                }

            case "ResultData":
                {
                    playerdata = myData.player;


                    isResultdone = true;
                    break;
                }

            case "ExitUser":
                {
                    if (gameSocket != null)
                    {
                        Debug.Log("Dispose my Socket");
                        this.manager.Close();
                    }

                    Application.ExternalCall("window.parent.postMessage", "onExit", "*");
#if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval(@"
              if(window.ReactNativeWebView){
                window.ReactNativeWebView.postMessage('onExit');
              }
            ");
#endif
                    break;
                }

            default:
                Debug.LogWarning("Unknown id in JSON: " + id);
                break;
        }
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
        Debug.Log("*** send Data ***" + Room);
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "JOIN_LEVEL";
        message.payload.level = Room;

        string json = JsonUtility.ToJson(message);
        Debug.Log("*** send Data ***" + json);
        // SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnRoomEnter).Emit("request", json);
    }
    internal void BetPlaced(int amountIndex, string betType, string betOption)
    {
        BetMessage message = new BetMessage();
        message.type = "PLACE_BET";
        message.payload = new BetPayload();

        message.payload.amountIndex = amountIndex;
        message.payload.betType = betType;
        message.payload.betOption = betOption;

        string json = JsonUtility.ToJson(message);
        Debug.Log("Bet JSON: " + json);

        gameSocket.ExpectAcknowledgement<string>(OnBetAcknowledged).Emit("request", json);
    }
    internal void SendUndo()
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "UNDO_BET";
        // message.payload.level = Room;

        string json = JsonUtility.ToJson(message);

        //  SendDataWithNamespace("request", json);
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
        // message.payload.level = Room;
        loadingPageLoading = false;
        NormalStart = false;
        DontDisplayDisconected = true;
        string json = JsonUtility.ToJson(message);
        Debug.Log("Return home: " + json);
        // SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnHome).Emit("request", json);
    }

    internal void SendHistory(int Pages)
    {
        SendRoom message = new SendRoom();
        message.payload = new Payload();
        message.type = "BET_HISTORY";
        message.payload.page = Pages;

        string json = JsonUtility.ToJson(message);
        Debug.Log("Hestory sent: " + json);

        // SendDataWithNamespace("request", json);
        gameSocket.ExpectAcknowledgement<string>(OnHistory).Emit("request", json);
    }

    void OnHome(string json)
    {
        gameManager.ClearAllBets();
        Debug.Log("Home Receved: " + json);
        ReturnHome = JsonUtility.FromJson<Root>(json);
        gameManager.SetPlayerCountOnReturn(ReturnHome.payload.lobby);
        //  Invoke(nameof(Reconnect), 0.2f);

    }
    void OnHistory(string json)
    {
        Debug.Log("**History Receved**" + json);
        HistoryPageData = JsonUtility.FromJson<Root>(json);
        uiManager.SetHistoryPage(HistoryPageData.payload);
    }

    void OnDouble(string json)
    {
        Debug.Log(json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        if (doubleBetData.success)
        {
            gameManager.DoubleBets(doubleBetData.payload.bets);
            gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
            gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        }
        else
        {
            gameManager.PlayPopup(doubleBetData.payload.message);
        }
    }
    void OnRepeat(string json)
    {
        Debug.Log("RepeatBet" + json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        if (doubleBetData.success)
        {
            gameManager.RepeAtBet(doubleBetData.payload.bets);
            gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
            gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        }
        else
        {
            gameManager.PlayPopup(doubleBetData.payload.message);
        }
    }
    void OnCancle(string json)
    {
        doubleBetData = JsonUtility.FromJson<Root>(json);
        gameManager.CancleBets();
        gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
        gameManager.currentTotalBet = 0;
        uiManager.SetChipoption(false);
    }
    void OnUndo(string json)
    {
        Debug.Log("undo :" + json);
        Debug.Log(json);
        doubleBetData = JsonUtility.FromJson<Root>(json);
        gameManager.UnduBets(doubleBetData.payload.bet.betId);
        gameManager.UpdatePlayerbalance(doubleBetData.payload.balance.ToString());
        gameManager.currentTotalBet = doubleBetData.payload.totalBet;
        if (doubleBetData.payload.totalBet == 0) uiManager.SetChipoption(false);
    }
    void OnRoomEnter(string json)
    {
        Debug.Log(json);
        roomData = JsonUtility.FromJson<Root>(json);
        gameManager.SetCoinData();
        gameManager.SetOptionData();
        gameManager.SetOtherplayerData(roomData.payload.leaderboards);
    }

    void ManageOtherPlayerbets(string data)
    {
        // Debug.Log("Bet Placed $$$$$$$$$$$$$$\n" + data);
        OtherChipData = JsonUtility.FromJson<Root>(data);
        gameManager.ManageBrodcastBetsOtherPlayers(OtherChipData);

    }
    void OnGameLoopStarted(string json)
    {
        if (!loadingPageLoading)
        {
            loadingPageLoading = true;
            gameManager.OnGameLoaded();
        }
        NormalStart = true;
        Debug.Log("Loop Started\n" + json);
        gameLoopData = JsonUtility.FromJson<Root>(json);
        gameManager.SetMainCard();
    }
    void OnGameLoopEnd(string data)
    {
        gameLoopData = JsonUtility.FromJson<Root>(data);

        Debug.Log("Loop end\n" + data);


        if (!NormalStart)
        {
            gameManager.GamePage.SetActive(true);
            gameManager.SetLoadingPage(false);
            gameManager.HomePage.SetActive(false);
            gameManager.StartGameMidway();
        }
        else
        {
            gameManager.StartGame();

        }

        // Only start game AFTER validating
    }
    void OnCashout(string data)
    {
        Debug.Log("CashOut\n" + data);
        CashoutData = JsonUtility.FromJson<Root>(data);


    }
    void OnLobbyCount(string data)
    {
        Debug.Log("playerount\n" + data);
        TotalPlayerCountData = JsonUtility.FromJson<Root>(data);
        gameManager.SetPlayerCountOnReturn(TotalPlayerCountData.lobby);

    }
    private void OnBetAcknowledged(string data)
    {

        Debug.Log("Bet Acknowledgement: " + data);
        BetChipData = JsonUtility.FromJson<Root>(data);
        if (BetChipData.success)
        {
            gameManager.ManageBrodcastBetsPlayer();
            gameManager.UpdatePlayerbalance(BetChipData.payload.balance.ToString());
            gameManager.currentTotalBet = BetChipData.payload.totalBet;
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


    public int balance;
    public List<Bet> bets;


    public int refundAmount;
    public Bet bet;

    public int page;

    public List<History> history;
    public Meta meta;

    public Lobby lobby;


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

public class S1115
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S15
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S1620
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S2125
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S2630
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S3135
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S3640
{
    public int payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S4153
{
    public int payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class S610
{
    public double payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}
public class Andar
{
    public List<double> payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class Bahar
{
    public List<double> payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class Bets
{
    public List<int> casual { get; set; }
    public List<int> novice { get; set; }
    public List<int> expert { get; set; }
    public List<int> high_roller { get; set; }

    public string username { get; set; }
    public int amount { get; set; }
    public string betId { get; set; }
    public string betType { get; set; }
    public string level { get; set; }
    public string userId { get; set; }
    public string betOption { get; set; }
}

public class First1Andar
{
    public List<double> payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class First1Bahar
{
    public List<double> payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class First3
{
    public Payout payout { get; set; }
    public MaxBetLimit max_bet_limit { get; set; }
}

public class GameData
{
    public List<string> betOptions { get; set; }
    public int roundInterval { get; set; }
    public int cashoutInterval { get; set; }
    public int middleCardLimit { get; set; }
    public int statsLimit { get; set; }
    public Bets bets { get; set; }
    public List<string> levels { get; set; }
    public Wagers wagers { get; set; }
    public Lobby lobby { get; set; }
    public HandCode handCode { get; set; }
    public Leaderboards leaderboards { get; set; }
    public List<object> stats { get; set; }
}

public class HandCode
{
    [JsonProperty("1")]
    public string _1 { get; set; }

    [JsonProperty("2")]
    public string _2 { get; set; }

    [JsonProperty("3")]
    public string _3 { get; set; }

    [JsonProperty("4")]
    public string _4 { get; set; }

    [JsonProperty("5")]
    public string _5 { get; set; }

    [JsonProperty("6")]
    public string _6 { get; set; }

    [JsonProperty("7")]
    public string _7 { get; set; }

    [JsonProperty("8")]
    public string _8 { get; set; }

    [JsonProperty("9")]
    public string _9 { get; set; }

    [JsonProperty("10")]
    public string _10 { get; set; }
    public int HIGH_CARD { get; set; }
    public int PAIR { get; set; }
    public int TWO_PAIR { get; set; }
    public int THREE_OF_A_KIND { get; set; }
    public int STRAIGHT { get; set; }
    public int FLUSH { get; set; }
    public int FULL_HOUSE { get; set; }
    public int FOUR_OF_A_KIND { get; set; }
    public int STRAIGHT_FLUSH { get; set; }
    public int ROYAL_FLUSH { get; set; }
}


[Serializable]
public class Lobby
{
    public int casual;
    public int novice;
    public int expert;
    public int high_roller;
}

public class MainBets
{
    public Andar andar { get; set; }
    public Bahar bahar { get; set; }
}

public class MaxBetLimit
{
    public int casual { get; set; }
    public int novice { get; set; }
    public int expert { get; set; }
    public int high_roller { get; set; }
}

public class OpBets
{
    public First1Andar first_1_andar { get; set; }
    public First1Bahar first_1_bahar { get; set; }
    public First3 first_3 { get; set; }
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


}

public class SideBets
{
    public S15 s_1_5 { get; set; }
    public S610 s_6_10 { get; set; }
    public S1115 s_11_15 { get; set; }
    public S1620 s_16_20 { get; set; }
    public S2125 s_21_25 { get; set; }
    public S2630 s_26_30 { get; set; }
    public S3135 s_31_35 { get; set; }
    public S3640 s_36_40 { get; set; }
    public S4153 s_41_53 { get; set; }
}

public class Wagers
{
    public MainBets main_bets { get; set; }
    public OpBets op_bets { get; set; }
    public SideBets side_bets { get; set; }
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
    public string bet_amount;
    public string win_amount;
    public string bet_type;
    public string bet_option;
    public string round_id;
    public string bet_id;
    public string level;
    public string middle_card;
    public int cards_dealt;
    public string match_side;
    public string matching_card;
    public DateTime created_at;

    // Parsed objects
    public Card middleCardParsed;
    public Card matchingCardParsed;
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
