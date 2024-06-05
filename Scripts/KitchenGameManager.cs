using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class KitchenGameManager : NetworkBehaviour
{
    public static KitchenGameManager instance {  get; private set; }

    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChange;

    private enum State {
        waitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state=new NetworkVariable<State>(State.waitingToStart);
    //private float waitingToStartTimer = 1f;
    private NetworkVariable<float> countdownToStartTimer =new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer= new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 300f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool isLocalPlayerReady;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPauseDictionary;
    //ulong���޷��ŵ�64λ�������ͣ���ʾ��Χ��0��18,446,744,073,709,551,615֮������������ķ�Χ�Ǵ�0��2^64-1��
    private bool autoTestGamePausedState;



    private void Awake() {
        instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();


    }
    private void Start() {
        GameInput.instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.instance.OnInteractAction += GameInput_OnInteractAction;
    }
    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach(ulong cliendId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(cliendId,true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        autoTestGamePausedState = true;
        //�������˳�ʱ���ͽ�����ͣ��⣬
        //����˳�����Ҳ����ͣ���Ǹ��ˣ���ô��ʱ������ͣ��⣬������������ڷ���������˶�������ͣ��ȫ����ͣΪfalse
        //�Դ��Զ�ȡ������������ҵ���ͣ
        //1.NetworkManager_OnClientDisconnectCallback
        //2.LateUpdate
        //3.TestGamePausedState
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue) {
        if(isGamePaused.Value) {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this,EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue,State newValue) {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (state.Value == State.waitingToStart) {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChange?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                //This player is NOT ready
                allClientsReady = false;
                break;
            }
        }
        //Debug.Log("allClientsReady:" + allClientsReady);
        //Debug.Log(serverRpcParams.Receive.SenderClientId);

        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        switch(state.Value) {
            case State.waitingToStart:
                //waitingToStartTimer -= Time.deltaTime;
                //if (waitingToStartTimer < 0f) {
                //    state = State.CountdownToStart;
                //    OnStateChanged?.Invoke(this,EventArgs.Empty);
                //}
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f) {
                    state.Value = State.GameOver;
                    //OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:

                break;
        }
        //Debug.Log(state);
    }
    private void LateUpdate() {
        if (autoTestGamePausedState) {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }
    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }
    public bool IsLocalPlayerReady() {
        return isLocalPlayerReady;
    }
    public bool IsCountdownToStartActive() {
        return state.Value == State.CountdownToStart;
    }
    public float GetCountdownToStartTimer() {
        return countdownToStartTimer.Value;
    }
    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }
    public bool IsWaitingToStart() {
        return state.Value ==State.waitingToStart;
    }
    public float GetGamePlayingTimer() {
        return gamePlayingTimer.Value;
    }
    public float GetGamePlayingTimerNormalize() {
        return 1-(gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            PauseGameServerRpc();
            //Time.timeScale = 0f;
            OnLocalGamePaused?.Invoke(this,  EventArgs.Empty);
        }else {
            UnpauseGameServerRpc();
            //Time.timeScale = 1f;
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void PauseGameServerRpc(ServerRpcParams serverrpcParams=default) {
        playerPauseDictionary[serverrpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverrpcParams = default) {
        playerPauseDictionary[serverrpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }
    private void TestGamePausedState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId]) {
                // This player is paused
                isGamePaused.Value = true;
                //��һ�����(playerPauseDictionary)����ͣ����ȫ����Ҷ���ͣ(isGamePaused=true)
                return;
            }
        }
        //All players are unpaused
         isGamePaused.Value = false;
    }
}
