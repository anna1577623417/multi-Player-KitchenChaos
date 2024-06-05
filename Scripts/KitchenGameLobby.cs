using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour{

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static KitchenGameLobby instance {  get; private set; }
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float ListLobbiesTimer;

    public event EventHandler OnCreatedLobbyStarted;
    public event EventHandler OnCreatedLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler <OnLobbyListChangedEventArags> OnLobbyListChanged;
    public class OnLobbyListChangedEventArags : EventArgs {
        public List<Lobby> lobbyList;
    }
    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication() {//这是一个私有的异步方法，负责初始化Unity身份验证。
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            //查Unity服务的状态是否已经初始化。只有当服务未初始化时才执行下面的逻辑。
            InitializationOptions initializationOptions = new InitializationOptions();
            //创建一个InitializationOptions对象，用于配置Unity服务的初始化选项。
            //initializationOptions.SetProfile(UnityEngine.Random.Range(0,10000).ToString());
            //使用SetProfile方法设置一个随机生成的数字作为配置文件的值。
            await UnityServices.InitializeAsync(initializationOptions);
            //使用UnityServices类的InitializeAsync方法来异步初始化Unity服务。
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //使用AuthenticationService类的SignInAnonymouslyAsync方法来以匿名方式进行身份验证，
            //即在未提供用户凭据的情况下登录

            //这段代码的目的是通过初始化Unity服务，并以匿名方式进行身份验证，以便后续使用Unity身份验证功能。
        }
    }

    private void Update() {
        HandleHeartBeat();
        HandlePeriodicListLobbies();
    }
    private void HandlePeriodicListLobbies() {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn && 
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()) {

            ListLobbiesTimer -= Time.deltaTime;
            if (ListLobbiesTimer <= 0f) {
                float listLobbiesTimerMax = 3.0f;
                ListLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }
    private void HandleHeartBeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -=Time.deltaTime;
            if (heartbeatTimer <= 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    private bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
            }
            };
            QueryResponse queryReponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArags {
                lobbyList = queryReponse.Results
            });
        }catch(LobbyServiceException e) {
            Debug.Log(e);
        }
        
    }

    private async Task<Allocation> AllocateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }

    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            JoinAllocation joinAllocation=await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }catch(RelayServiceException e) {
            Debug.Log(e);
            return default;
        }

    }

    public async void CreateLobby(string lobbyName,bool isPrivate) {
        OnCreatedLobbyStarted?.Invoke(this, EventArgs.Empty);
        //try 是一个异常处理的关键字，用于编写可处理异常的代码块。
        //try 块包含可能会抛出异常的代码，并指定在异常发生时如何进行处理。
        try {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {KEY_RELAY_JOIN_CODE,new DataObject(DataObject.VisibilityOptions.Member,relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation,"dtls"));

            KitchenGameMultiplayer.instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }   catch(LobbyServiceException e) {
                Debug.Log(e);
                OnCreatedLobbyFailed?.Invoke(this,EventArgs.Empty);
        }
        //使用了 try-catch 块来捕获 LobbyServiceException 异常
    }

    public async void QuickJoin() {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.instance.StartClient();
        } catch(LobbyServiceException e) {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this,EventArgs.Empty);
        }
    }
    public async void JoinWithCode(string lobbyCode) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            //房间密码正确则进入房间
            KitchenGameMultiplayer.instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinWithId(string lobbyId) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            //ID正确则进入房间
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls")); 

            KitchenGameMultiplayer.instance.StartClient();
        } catch(LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this,EventArgs.Empty);
        }
    }

    public async void DeleteLobby() {
        if(joinedLobby != null) {
            try {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }catch(LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public Lobby GetLobby() {
        return joinedLobby;
    }
}
